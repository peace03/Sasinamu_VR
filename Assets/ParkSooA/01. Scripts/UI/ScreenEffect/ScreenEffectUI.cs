using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectUI : MonoBehaviour
{
    [Header("연출 이미지")]
    [SerializeField] private Image effectImage;

    [Header("연출 글자")]
    [SerializeField] private Text effectText;

    [Header("블랙 아웃 - 시작 값")]
    [SerializeField][Range(0f, 1f)] private float blackOutStartValue = 0f;

    [Header("블랙 아웃 - 끝 값")]
    [SerializeField][Range(0f, 1f)] private float blackOutEndValue = 1f;

    [Header("페이드 인 - 시작 값")]
    [SerializeField][Range(0f, 1f)] private float fadeInStartValue = 1f;

    [Header("페이드 인 - 끝 값")]
    [SerializeField][Range(0f, 1f)] private float fadeInEndValue = 0f;

    [Header("연출의 총 시간")]
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
        // 모든 코루틴 멈추기
        StopAllCoroutines();
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
            imageColor.a = Mathf.Lerp(blackOutStartValue, blackOutEndValue,
                                effectCurve.Evaluate(progress / effectDuration));
            // 이미지에 적용하기
            effectImage.color = imageColor;
            // 프레임 기다리기
            yield return null;
        }

        // 불투명도 맞추기
        imageColor.a = blackOutEndValue;
        // 이미지에 적용하기
        effectImage.color = imageColor;
        // 30초 기다리기
        yield return effectDuration;
        // 소리 재생하기
        Debug.Log("소리 재생하기");
        // 페이드 인 시작
        StartCoroutine(FadeInCoroutine());
    }

    // 페이드 인 코루틴
    private IEnumerator FadeInCoroutine()
    {
        // 이미지의 색깔 저장
        Color imageColor = effectImage.color;
        // 글자의 색깔 저장
        Color textColor = effectText.color;
        // 진행률 변수
        float progress = 0f;

        // 진행이 끝날 때까지(30초)
        while (progress < effectDuration)
        {
            // 진행률 증가
            progress += Time.deltaTime;
            // 마지막에서 현재 진행률을 뺀 지점의 페이드 효과 그래프 값을 저장하기(시작이 1에서 시작하므로)
            imageColor.a = textColor.a = Mathf.Lerp(fadeInStartValue, fadeInEndValue,
                                        effectCurve.Evaluate(progress / effectDuration));
            // 이미지에 적용하기
            effectImage.color = imageColor;
            // 글자에 적용하기
            effectText.color = textColor;
            // 프레임 기다리기
            yield return null;
        }

        // 불투명도 맞추기
        imageColor.a = textColor.a = fadeInEndValue;
        // 이미지에 적용하기
        effectImage.color = imageColor;
        // 글자에 적용하기
        effectText.color = textColor;
        // UI 닫기
        gameObject.SetActive(false);
    }
}