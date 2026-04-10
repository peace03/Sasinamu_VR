using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("플레이어 레이어")]
    [SerializeField] private LayerMask playerLayer;

    [Header("이동할 씬 종류")]
    [SerializeField] private SceneType sceneType;

    [Header("블랙 아웃 기다리는 시간")]
    [SerializeField][Range(5f, 30f)] private float waitDuration = 30f;

    [Header("진입 이벤트 발생")]
    [SerializeField] private UnityEvent OnEnterEvent;

    [Header("퇴장 이벤트 발생")]
    [SerializeField] private UnityEvent OnExitEvent;

    private Coroutine sceneChangeCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        // 들어온 물체가 플레이어가 아니거나, 씬 전환 코루틴이 진행 중이면
        if ((1 << other.gameObject.layer & playerLayer.value) == 0 || sceneChangeCoroutine != null)
            // 종료
            return;

        // 씬 전환 시작
        sceneChangeCoroutine = StartCoroutine(SceneChangeCoroutine());
    }

    private void OnTriggerExit(Collider other)
    {
        // 나간 물체가 플레이어가 아니거나, 씬 전환 코루틴이 진행 중이 아니라면
        if ((1 << other.gameObject.layer & playerLayer.value) == 0 || sceneChangeCoroutine == null)
            // 종료
            return;

        // 씬 전환 중지
        StopCoroutine(SceneChangeCoroutine());
        // 씬 전환 코루틴 초기화
        sceneChangeCoroutine = null;
    }

    // 씬 전환 코루틴
    private IEnumerator SceneChangeCoroutine()
    {
        // 블랙 아웃 기다리기
        yield return waitDuration;
        // 코루틴 초기화
        sceneChangeCoroutine = null;

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