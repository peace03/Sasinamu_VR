using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void ChangeSubwayScene()
    {
        SceneManager.LoadScene("SubwayScene_Y");
    }
}
