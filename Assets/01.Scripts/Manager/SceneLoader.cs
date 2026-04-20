using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public enum LoadScene
    {
        StartScene,
        SubwayScene,
        EndScene
    }

    [Header("이동할 씬 설정")]
    [SerializeField] private LoadScene loadScene;

    public void ChangeScene()
    {
        switch (loadScene)
        {
            case LoadScene.StartScene:
                SceneManager.LoadScene("StartRoom_Y");
                break;
            case LoadScene.SubwayScene:
                SceneManager.LoadScene("SubwayScene_Y");
                break;
            case LoadScene.EndScene:
                SceneManager.LoadScene("EndScene_Y");
                break;
            default:
                break;
        }
    }
}
