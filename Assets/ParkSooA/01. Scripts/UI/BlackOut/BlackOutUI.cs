using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackOutUI : MonoBehaviour
{
    [Header("연출 이미지")]
    [SerializeField] private Image effectImage;

    [Header("불투명도 시작 값")]
    [SerializeField][Range(0f, 1f)] private float startValue = 0f;

    [Header("불투명도 끝 값")]
    [SerializeField][Range(0f, 1f)] private float endValue = 1f;

    [Header("연출 시간")]
    [SerializeField][Range(5f, 30f)] private float effectDuration = 30f;

    [Header("연출 변화율 그래프")]
    [SerializeField] private AnimationCurve effectCurve;

    private void OnEnable()
    {
        // 블랙 아웃 시작
        StartCoroutine(BlackOutCoroutine());
    }

    private void OnDisable()
    {
        // 블랙 아웃 종료
        StopCoroutine(BlackOutCoroutine());
    }

    // 블랙 아웃 코루틴
    private IEnumerator BlackOutCoroutine()
    {
        // 이미지의 색깔 저장
        Color imageColor = effectImage.color;
        // 진행률 변수
        float progress = 0f;

        // 진행이 끝날 때까지(30초)
        while (progress < effectDuration)
        {
            // 진행률 증가
            progress += Time.deltaTime;
            // 현재 진행률의 스크린 효과 그래프 값을 저장하기
            imageColor.a = Mathf.Lerp(startValue, endValue, effectCurve.Evaluate(progress / effectDuration));
            // 이미지에 적용하기
            effectImage.color = imageColor;
            // 프레임 기다리기
            yield return null;
        }

        // 불투명도 맞추기
        imageColor.a = endValue;
        // 이미지에 적용하기
        effectImage.color = imageColor;
    }
}