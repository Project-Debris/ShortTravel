using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTileFillerWindow : EditorWindow
{
    const string DefaultDataDir = "Assets/_Project/Data/Level";
    const string DefaultWallTile = "Assets/_Project/Art/Sprites/Greybox/Tiles/Tile_Platform.asset";
    const string DefaultThinTile = "Assets/_Project/Art/Sprites/Greybox/Tiles/Tile_PlatformThin.asset";
    const string DefaultDisappearTile = "Assets/_Project/Art/Sprites/Greybox/Tiles/Tile_PlatformDisappear.asset";

    const string WallSuffix = "_wall_ceiling.csv";
    const string ThinSuffix = "_platform_thin.csv";
    const string DisappearSuffix = "_platform_disappear.csv";

    const string WallTilemapName = "Platform_Ceiling";
    const string ThinTilemapName = "Platform_Thin";
    const string DisappearGroupName = "Platform_Disappearing";

    string dataDir = DefaultDataDir;
    TileBase wallTile;
    TileBase thinTile;
    TileBase disappearTile;

    Grid targetGrid;
    string[] roomIds = new string[0];
    int roomIndex;

    [MenuItem("Tools/Room Tile Filler")]
    static void Open() => GetWindow<RoomTileFillerWindow>("Room Tile Filler");

    void OnEnable()
    {
        if (wallTile == null) wallTile = AssetDatabase.LoadAssetAtPath<TileBase>(DefaultWallTile);
        if (thinTile == null) thinTile = AssetDatabase.LoadAssetAtPath<TileBase>(DefaultThinTile);
        if (disappearTile == null) disappearTile = AssetDatabase.LoadAssetAtPath<TileBase>(DefaultDisappearTile);
        RefreshRooms();
    }

    void RefreshRooms()
    {
        if (!Directory.Exists(dataDir))
        {
            roomIds = new string[0];
            return;
        }

        roomIds = Directory.GetFiles(dataDir, "*" + WallSuffix)
            .Select(Path.GetFileName)
            .Select(n => n.Substring(0, n.Length - WallSuffix.Length))
            .OrderBy(n => n)
            .ToArray();

        roomIndex = Mathf.Clamp(roomIndex, 0, Mathf.Max(0, roomIds.Length - 1));
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("데이터", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            dataDir = EditorGUILayout.TextField("CSV 폴더", dataDir);
            if (GUILayout.Button("새로고침", GUILayout.Width(70))) RefreshRooms();
        }

        if (roomIds.Length == 0)
            EditorGUILayout.HelpBox($"'{dataDir}'에서 *{WallSuffix} 파일을 찾지 못했습니다.", MessageType.Warning);
        else
            roomIndex = EditorGUILayout.Popup("방", roomIndex, roomIds);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("타일", EditorStyles.boldLabel);
        wallTile = (TileBase)EditorGUILayout.ObjectField("벽/천장 타일", wallTile, typeof(TileBase), false);
        thinTile = (TileBase)EditorGUILayout.ObjectField("얇은 플랫폼 타일", thinTile, typeof(TileBase), false);
        disappearTile = (TileBase)EditorGUILayout.ObjectField("사라지는 발판 타일", disappearTile, typeof(TileBase), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("대상", EditorStyles.boldLabel);
        targetGrid = (Grid)EditorGUILayout.ObjectField("Grid", targetGrid, typeof(Grid), true);
        if (GUILayout.Button("Hierarchy에서 선택한 오브젝트의 Grid 사용"))
        {
            var go = Selection.activeGameObject;
            if (go != null) targetGrid = go.GetComponentInParent<Grid>();
        }

        EditorGUILayout.Space();
        bool ready = roomIds.Length > 0 && wallTile != null && thinTile != null && targetGrid != null;
        using (new EditorGUI.DisabledScope(!ready))
        {
            if (GUILayout.Button("채우기", GUILayout.Height(32)))
                Fill(roomIds[roomIndex]);
        }
    }

    void Fill(string roomId)
    {
        Selection.activeGameObject = targetGrid.gameObject;
        GUIUtility.hotControl = 0;
        GUIUtility.keyboardControl = 0;

        Undo.RegisterFullObjectHierarchyUndo(targetGrid.gameObject, "Fill Room Tiles");

        int wall = FillLayer(Path.Combine(dataDir, roomId + WallSuffix), WallTilemapName, wallTile);
        int thin = FillLayer(Path.Combine(dataDir, roomId + ThinSuffix), ThinTilemapName, thinTile);
        int disappear = FillDisappearLayer(Path.Combine(dataDir, roomId + DisappearSuffix), DisappearGroupName, disappearTile);
        Debug.Log($"[{roomId}] → {targetGrid.name}: 벽/천장 {wall}칸, 얇은 플랫폼 {thin}칸, 사라지는 발판 {disappear}개 배치 완료");
    }

    int FillLayer(string csvPath, string tilemapName, TileBase tile)
    {
        var tilemap = GetOrCreateTilemap(tilemapName);

        Undo.RegisterCompleteObjectUndo(tilemap, "Fill " + tilemapName);
        tilemap.ClearAllTiles();

        if (!File.Exists(csvPath))
        {
            Debug.LogWarning($"CSV가 없어 '{tilemapName}'은 비워 두었습니다: {csvPath}");
            return 0;
        }

        int count = 0;
        var lines = File.ReadAllLines(csvPath);
        for (int i = 1; i < lines.Length; i++)
        {
            var p = lines[i].Split(',');
            if (p.Length < 2) continue;
            if (!int.TryParse(p[0], out int row) || !int.TryParse(p[1], out int col)) continue;

            tilemap.SetTile(new Vector3Int(col, -(row - 1), 0), tile);
            count++;
        }

        EditorUtility.SetDirty(tilemap);
        return count;
    }

    int FillDisappearLayer(string csvPath, string groupName, TileBase tile)
    {
        var old = targetGrid.transform.Find(groupName);
        if (old != null)
        {
            Selection.objects = Array.Empty<UnityEngine.Object>();
            Undo.DestroyObjectImmediate(old.gameObject);
        }

        var sprite = (tile as Tile)?.sprite;
        if (sprite == null || !File.Exists(csvPath)) return 0;

        var group = new GameObject(groupName);
        Undo.RegisterCreatedObjectUndo(group, "Create Disappear Group");
        group.transform.SetParent(targetGrid.transform, false);

        var cells = File.ReadLines(csvPath).Skip(1)
            .Select(line => line.Split(','))
            .Where(p => p.Length >= 2 && int.TryParse(p[0], out _) && int.TryParse(p[1], out _))
            .Select(p => (row: int.Parse(p[0]), col: int.Parse(p[1])))
            .OrderBy(c => c.row).ThenBy(c => c.col);

        Vector3 cell = targetGrid.cellSize;
        int count = 0;
        int prevRow = int.MinValue, prevCol = int.MinValue;
        int startCol = 0, length = 0;

        void Spawn()
        {
            if (length == 0) return;

            float width = cell.x * length;

            var go = new GameObject($"Disappearing_{prevRow}_{startCol}");
            Undo.RegisterCreatedObjectUndo(go, "Create Disappear Run");
            go.transform.SetParent(group.transform, false);
            go.layer = LayerMask.NameToLayer("Ground");
            go.transform.position = targetGrid.CellToWorld(new Vector3Int(startCol, -(prevRow - 1), 0))
                                  + new Vector3(width * 0.5f, cell.y * 0.5f, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = new Vector2(width, sprite.bounds.size.y);

            var box = go.AddComponent<BoxCollider2D>();
            box.size = sr.size;

            go.AddComponent<DisappearingPlatform>();
            count++;
        }

        foreach (var (row, col) in cells)
        {
            if (row != prevRow || col != prevCol + 1)
            {
                Spawn();
                startCol = col;
                length = 0;
            }

            length++;
            prevRow = row;
            prevCol = col;
        }
        Spawn();

        return count;
    }

    Tilemap GetOrCreateTilemap(string tilemapName)
    {
        var child = targetGrid.transform.Find(tilemapName);
        if (child != null && child.TryGetComponent(out Tilemap existing))
        {
            EnsurePhysics(existing.gameObject);
            return existing;
        }

        var go = new GameObject(tilemapName, typeof(Tilemap), typeof(TilemapRenderer));
        Undo.RegisterCreatedObjectUndo(go, "Create " + tilemapName);
        go.transform.SetParent(targetGrid.transform, false);
        EnsurePhysics(go);
        return go.GetComponent<Tilemap>();
    }

    static void EnsurePhysics(GameObject go)
    {
        if (!go.TryGetComponent(out Rigidbody2D rb)) rb = Undo.AddComponent<Rigidbody2D>(go);
        if (!go.TryGetComponent(out TilemapCollider2D tc)) tc = Undo.AddComponent<TilemapCollider2D>(go);
        if (!go.TryGetComponent(out CompositeCollider2D cc)) cc = Undo.AddComponent<CompositeCollider2D>(go);

        Undo.RecordObjects(new UnityEngine.Object[] { rb, tc, cc }, "Setup Tilemap Physics");
        rb.bodyType = RigidbodyType2D.Static;
        cc.geometryType = CompositeCollider2D.GeometryType.Polygons;
#if UNITY_6000_0_OR_NEWER
        tc.compositeOperation = Collider2D.CompositeOperation.Merge;
#else
        tc.usedByComposite = true;
#endif
    }
}