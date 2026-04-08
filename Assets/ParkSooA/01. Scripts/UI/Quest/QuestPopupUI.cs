using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopupUI : MonoBehaviour
{
    [Header("UI가 들어갈 위치(버튼)")]
    [SerializeField] private Button questButton;

    [Header("시작된 후 UI가 나타날 시간")]
    [SerializeField][Range(0f, 10f)] private float waitDuration;

    [Header("UI를 보여줄 시간")]
    [SerializeField][Range(0f, 15f)] private float displayDuration;

    [Header("UI가 버튼으로 움직이는 시간(작을수록 빠름)")]
    [SerializeField][Range(0f, 1f)] private float movingDuration;

    [Header("연출이 멈추는 시점 판단 거리(작을수록 느림)")]
    [SerializeField][Range(0f, 0.1f)] private float stopDistance;

    private Vector3 originPosition;                             // 원래 위치
    private Vector3 moveVelocity = Vector3.zero;                // 움직였던 속도
    private Vector3 scaleVelocity = Vector3.zero;               // 줄었던 크기

    private QuestUIState curUIState = QuestUIState.First;       // 현재 UI 상태

    private void Awake()
    {
        // 퀘스트 버튼 누를 수 없음
        questButton.interactable = false;
        // 원래 위치 저장
        originPosition = transform.position;
    }

    private void Start()
    {
        // 크기 0으로 바꾸기
        transform.localScale = Vector3.zero;
        // UI 열기
        UIHandler();
    }

    // UI 관리 함수
    public void UIHandler()
    {
        // 현재 UI 상태에 따라
        switch (curUIState)
        {
            // 처음이라면
            case QuestUIState.First:
                // UI 처음 열기 실행
                StartCoroutine(FirstOpenUICoroutine());
                break;
            // 열림이라면
            case QuestUIState.Open:
                // UI 닫기 실행
                StartCoroutine(MoveToTargetCoroutine(questButton.transform.position, Vector3.zero));
                break;
            // 닫힘이라면
            case QuestUIState.Close:
                // UI 열기
                gameObject.SetActive(true);
                // UI 열기 실행
                StartCoroutine(MoveToTargetCoroutine(originPosition, Vector3.one));
                break;
            // 그 외라면
            default:
                // 예외 처리
                Debug.Log("없는 UI 상태입니다.");
                break;
        }
    }

    // 처음 열기 코루틴
    private IEnumerator FirstOpenUICoroutine()
    {
        // 기다리는 시간만큼 기다리기
        yield return new WaitForSeconds(waitDuration);

        // 도착 판단 거리(원래 크기)에 도달할 때까지
        while (Vector3.Distance(transform.localScale, Vector3.one) > stopDistance)
        {
            // 크기 키우기
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one,
                                                            ref scaleVelocity, movingDuration);
            // 프레임 기다리기
            yield return null;
        }

        // 크기 맞추기
        transform.localScale = Vector3.one;
        // 현재 UI 상태 바꾸기
        curUIState = QuestUIState.Open;
        // 보여주는 시간만큼 기다리기
        yield return new WaitForSeconds(displayDuration);
        // UI 관리 함수 실행
        UIHandler();
    }

    // 타겟 이동 코루틴
    private IEnumerator MoveToTargetCoroutine(Vector3 pos, Vector3 scale)
    {
        // 퀘스트 버튼 누를 수 없음
        questButton.interactable = false;

        // 도착 판단 거리에 도달할 때까지
        while (Vector3.Distance(transform.position, pos) > stopDistance)
        {
            // 이동
            transform.position = Vector3.SmoothDamp(transform.position, pos,
                                                                    ref moveVelocity, movingDuration);
            // 크기 조절
            transform.localScale = Vector3.SmoothDamp(transform.localScale, scale,
                                                            ref scaleVelocity, movingDuration);
            // 프레임 기다리기
            yield return null;
        }

        // 위치 맞추기
        transform.position = pos;
        // 크기 맞추기
        transform.localScale = scale;
        // 퀘스트 버튼 누를 수 있음
        questButton.interactable = true;

        // 현재 UI 상태가 열림이라면
        if (curUIState == QuestUIState.Open)
        {
            // 닫힌 상태로 변경
            curUIState = QuestUIState.Close;
            // UI 닫기
            gameObject.SetActive(false);
        }
        // 현재 UI 상태가 닫힘이라면
        else if (curUIState == QuestUIState.Close)
            // 열기 상태로 변경
            curUIState = QuestUIState.Open;
    }
}