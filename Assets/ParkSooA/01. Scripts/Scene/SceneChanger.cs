using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("플레이어 레이어")]
    [SerializeField] private LayerMask playerLayer;

    [Header("이동할 씬 종류")]
    [SerializeField] private SceneType sceneType;

    [Header("화면 전환")]
    [SerializeField] private ScreenFader screenFader;

    private void OnTriggerEnter(Collider other)
    {
        // 들어온 물체가 플레이어가 아니라면
        if ((1 << other.gameObject.layer & playerLayer.value) == 0)
            // 종료
            return;
        
        // 화면 전환이 비어있지 않다면
        if (screenFader != null)
        {
            // 화면 전환 활성화
            screenFader.gameObject.SetActive(true);
            // 화면 전환 시작
            screenFader.ScreenFadeHandler(true);
        }

        // 씬 전환 시작
        StartCoroutine(SceneChangeCoroutine());
    }

    private void OnTriggerExit(Collider other)
    {
        // 나간 물체가 플레이어가 아니라면
        if ((1 << other.gameObject.layer & playerLayer.value) == 0)
            // 종료
            return;

        // 화면 전환이 비어있지 않다면
        if (screenFader != null)
        {
            // 화면 전환 중지
            screenFader.StopScreenFader();
            // 화면 전환 비활성화
            screenFader.gameObject.SetActive(false);
        }

        // 씬 전환 중지
        StopCoroutine(SceneChangeCoroutine());
    }

    // 씬 전환 코루틴
    private IEnumerator SceneChangeCoroutine()
    {
        // 화면 전환이 비어있지 않다면
        if (screenFader != null)
            // 화면 전환 기다리기
            yield return new WaitForSeconds(screenFader.Duration);

        // 이동할 씬 종류에 해당하는 씬 이름이 있다면
        if (SceneRegistry.GetSceneName(sceneType, out string sceneName))
            // 씬 이동
            SceneManager.LoadScene(sceneName);
        // 없다면
        else
            // 알려주기
            Debug.Log($"[Error] {sceneType}에 맞는 씬이 등록되어 있지 않습니다.");
    }
}