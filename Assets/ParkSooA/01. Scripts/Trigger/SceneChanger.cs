using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR    // 유니티 에디터에서만
using UnityEditor;
#endif

public class SceneChanger : ArrivalTrigger
{
    [Header("이동할 씬")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif

    [Header("화면 전환 UI")]
    [SerializeField] private SceneChangeUI sceneChangeUI;

    private Coroutine sceneChangeCoroutine;     // 씬 전환 코루틴

    private string sceneName;                   // 이동할 씬 이름

    private void OnValidate()
    {
#if UNITY_EDITOR
        // 이동할 씬이 비어있지 않다면
        if (sceneAsset != null)
            // 이동할 씬 이름 저장
            sceneName = sceneAsset.name;
#endif
    }

    protected override void OnArrival()
    {
        // 씬 전환 중이라면
        if (sceneChangeCoroutine != null)
            // 종료
            return;

        // 화면 전환 UI가 비어있지 않다면
        if (sceneChangeUI != null)
        {
            // 화면 전환 UI 열기
            sceneChangeUI.gameObject.SetActive(true);
            // 화면 전환(페이드 아웃) 시작
            sceneChangeUI.SetSceneChange(true);
        }

        // 씬 전환 시작
        sceneChangeCoroutine = StartCoroutine(SceneChangeRoutine());
    }

    protected override void OnExited()
    {
        // 화면 전환 UI가 비어있지 않다면
        if (sceneChangeUI != null)
        {
            // 화면 전환(페이드 아웃) 중지
            sceneChangeUI.StopSceneChange();
            // 화면 전환 UI 닫기
            sceneChangeUI.gameObject.SetActive(false);
        }

        // 씬 전환 중이라면
        if (sceneChangeCoroutine != null)
        {
            // 씬 전환 중지
            StopCoroutine(sceneChangeCoroutine);
            // 씬 전환 코루틴 초기화
            sceneChangeCoroutine = null;
        }
    }

    // 씬 전환 코루틴
    private IEnumerator SceneChangeRoutine()
    {
        // 화면 전환 UI가 비어있지 않다면
        if (sceneChangeUI != null)
            // 화면 전환 기다리기
            yield return new WaitForSeconds(sceneChangeUI.Duration);

        // 씬 전환 코루틴 초기화
        sceneChangeCoroutine = null;

        // 이동할 씬 이름이 있다면
        if (!string.IsNullOrEmpty(sceneName))
            // 씬 이동
            SceneManager.LoadScene(sceneName);
        // 없다면
        else
            Debug.Log($"[Error] {gameObject.name}에 이동할 씬이 지정되지 않았습니다.");
    }
}