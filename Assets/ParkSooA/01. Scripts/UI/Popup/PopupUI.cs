using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    #region 변수
    [Header("플레이어 왼쪽 손")]
    [SerializeField] private Transform playerLeftHand;

    [Header("UI가 나타날 시간")]
    [SerializeField][Range(0f, 10f)] private float delayDuration = 10f;

    [Header("UI를 보여줄 시간")]
    [SerializeField][Range(0f, 15f)] private float displayDuration = 15f;

    [Header("UI가 좌표로 이동하는 시간(작을수록 빠름)")]
    [SerializeField][Range(0f, 1f)] private float movingDuration = 1f;

    [Header("연출이 멈추는 시점 판단 거리(작을수록 느림)")]
    [SerializeField][Range(0f, 0.1f)] private float stopDistance = 0.01f;

    [Header("========== 예외 처리 ==========")]

    [Header("캔버스와의 강제 고정 거리")]
    [SerializeField][Range(1f, 4f)] private float snapDistance = 2f;

    [Header("최대 연출 시간")]
    [SerializeField][Range(1f, 4f)] private float maxMovingDuration = 2f;

    private Coroutine moveCoroutine;                    // UI 이동 조절 코루틴
    private Coroutine scaleCoroutine;                   // UI 크기 조절 코루틴

    private WaitForSeconds DelayDuration;
    private WaitForSeconds DisplayDuration;

    private Vector3 moveVelocity = Vector3.zero;        // 움직였던 속도
    private Vector3 scaleVelocity = Vector3.zero;       // 줄었던 크기

    private bool isOpen = false;
    #endregion

    private void Awake()
    {
        // 초기화
        DelayDuration = new WaitForSeconds(delayDuration);
        DisplayDuration = new WaitForSeconds(displayDuration);
    }

    private void Start()
    {
        // 처음 퀘스트 팝업 UI 열기 시작
        StartCoroutine(FirstPopupQuestUI());
    }

    // 퀘스트 UI 관리 함수
    private void QuestUIHander() => StartCoroutine(QuestUIHandleCoroutine());

    // 퀘스트 UI 관리 코루틴 함수
    private IEnumerator QuestUIHandleCoroutine()
    {
        // UI가 닫혀있다면
        if (!isOpen)
        {
            // UI 열림
            isOpen = true;
            // UI 열기
            scaleCoroutine = StartCoroutine(UIScaleHandleCoroutine(Vector3.one));
        }
        // UI가 열려있다면
        else
        {
            // UI 닫힘
            isOpen = false;
            // UI 닫기
            scaleCoroutine = StartCoroutine(UIScaleHandleCoroutine(Vector3.zero));
        }

        // UI 이동이 끝날 때까지 기다리기
        yield return moveCoroutine;
        // UI 크기 조절이 끝날 때까지 기다리기
        yield return scaleCoroutine;
        // 코루틴 초기화
        moveCoroutine = scaleCoroutine = null;
    }

    // 처음에 퀘스트 UI 띄우는 함수
    private IEnumerator FirstPopupQuestUI()
    {
        // UI가 나타날 시간 기다리기
        yield return DelayDuration;
        // UI 여는 연출이 끝날 때까지 기다리기
        yield return StartCoroutine(UIScaleHandleCoroutine(Vector3.one));
        // UI를 보여줄 시간 기다리기
        yield return DisplayDuration;
        // UI 닫기
        scaleCoroutine = StartCoroutine(UIScaleHandleCoroutine(Vector3.zero));
        // UI 이동이 끝날 때까지 기다리기
        yield return moveCoroutine;
        // UI 닫기가 끝날 때까지 기다리기
        yield return scaleCoroutine;
        // 코루틴 초기화
        moveCoroutine = scaleCoroutine = null;
    }

    // UI 이동 조절 코루틴
    private IEnumerator UIMoveHandleCoroutine(Transform target)
    {
        // 시간 확인할 변수 선언
        float timer = 0f;

        // 목표 위치에 도달할 때까지
        while (Vector3.Distance(transform.position, target.position) > stopDistance)
        {
            // 시간 더하기
            timer += Time.deltaTime;

            // 최대 연출 시간이 지났다면
            if (timer > maxMovingDuration
                || Vector3.Distance(transform.position, target.position) > snapDistance)
                // 종료
                break;

            // UI 이동
            transform.position = Vector3.SmoothDamp(transform.position, target.position,
                                                                            ref moveVelocity, movingDuration);
            // 프레임 기다리기
            yield return null;
        }

        // 위치 맞추기
        transform.position = target.position;
    }

    // UI 크기 조절 코루틴
    private IEnumerator UIScaleHandleCoroutine(Vector3 size)
    {
        // 목표 크기에 도달할 때까지
        while (Vector3.Distance(transform.localScale, size) > stopDistance)
        {
            // UI 크기 조절
            transform.localScale = Vector3.SmoothDamp(transform.localScale, size, ref scaleVelocity, movingDuration);
            // 프레임 기다리기
            yield return null;
        }

        // 크기 맞추기
        transform.localScale = size;
    }
}