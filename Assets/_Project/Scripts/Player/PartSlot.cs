using UnityEngine;

public enum PartType { None, Pin, Discharge }

public class PartSlot : MonoBehaviour
{
    public PartType CurrentPart { get; private set; }
    public int RemainingUses { get; private set; }

    public void Equip(PartType part, int maxUses)
    {
        CurrentPart = part;
        RemainingUses = maxUses;
        Debug.Log($"{part} 장착 ({maxUses}회)");
    }
}