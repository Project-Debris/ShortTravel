using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("SandBox_Jasper");
    }
}
