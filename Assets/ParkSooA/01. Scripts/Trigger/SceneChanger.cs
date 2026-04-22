using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR    // 유니티 에디터에서만
using UnityEditor;
#endif

public class SceneChanger : MonoBehaviour
{
    [Header("플레이어 레이어")]
    [SerializeField] private LayerMask playerLayer;

    [Header("이동할 씬")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif

    [Header("화면 전환")]
    [SerializeField] private ScreenFader screenFader;

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

    private void OnTriggerEnter(Collider other)
    {
        // 들어온 물체가 플레이어가 아니라면
        if ((1 << other.gameObject.layer & playerLayer.value) == 0)
            // 종료
            return;

        // 씬 전환 중이라면
        if (sceneChangeCoroutine != null)
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
        sceneChangeCoroutine = StartCoroutine(SceneChangeRoutine());
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
        // 화면 전환이 비어있지 않다면
        if (screenFader != null)
            // 화면 전환 기다리기
            yield return new WaitForSeconds(screenFader.Duration);

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

//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//#if UNITY_EDITOR    // 유니티 에디터에서만
//using UnityEditor;
//#endif

//public class SceneChanger : ArrivalTrigger
//{
//#if UNITY_EDITOR
//    [Header("이동할 씬")]
//    [SerializeField] private SceneAsset sceneAsset;
//#endif

//    private Coroutine sceneChangeCoroutine;     // 씬 전환 코루틴

//    private string sceneName;                   // 이동할 씬 이름

//    private void OnValidate()
//    {
//#if UNITY_EDITOR
//        // 이동할 씬이 비어있지 않다면
//        if (sceneAsset != null)
//            // 이동할 씬 이름 저장
//            sceneName = sceneAsset.name;
//#endif
//    }

//    protected override void OnTriggerEnter(Collider other)
//    {
//        // 기존 함수 실행
//        base.OnTriggerEnter(other);

//        // 씬 전환 중이라면
//        if (sceneChangeCoroutine != null)
//            // 종료
//            return;

//        // 씬 전환 시작
//        sceneChangeCoroutine = StartCoroutine(SceneChangeRoutine());
//    }

//    protected override void OnTriggerExit(Collider other)
//    {
//        // 기존 함수 실행
//        base.OnTriggerExit(other);

//        // 씬 전환 중이라면
//        if (sceneChangeCoroutine != null)
//        {
//            // 씬 전환 중지
//            StopCoroutine(sceneChangeCoroutine);
//            // 씬 전환 코루틴 초기화
//            sceneChangeCoroutine = null;
//        }
//    }

//    // 씬 전환 코루틴
//    private IEnumerator SceneChangeRoutine()
//    {
//        // 화면 전환이 비어있지 않다면
//        if (screenFader != null)
//            // 화면 전환 기다리기
//            yield return new WaitForSeconds(screenFader.Duration);

//        // 씬 전환 코루틴 초기화
//        sceneChangeCoroutine = null;

//        // 이동할 씬 이름이 있다면
//        if (!string.IsNullOrEmpty(sceneName))
//            // 씬 이동
//            SceneManager.LoadScene(sceneName);
//        // 없다면
//        else
//            Debug.Log($"[Error] {gameObject.name}에 이동할 씬이 지정되지 않았습니다.");
//    }
//}