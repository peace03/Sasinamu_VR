using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private UnityEvent OnEnterStandbyStart;   //Object 못 움직일 때
    [SerializeField] private UnityEvent OnEnterStandbyEnd;     //다시 움직일 때
    [SerializeField] private UnityEvent OnAttentionFix;     //헤드셋 고개 못돌릴 때
    [SerializeField] private UnityEvent OnAttentionFree;    //다시 고객 돌릴 때
    [SerializeField] private float onlyMovePauseTime;       //움직임 정지 시간

    private void Start()
    {
        StartCoroutine(StandBy());
        StartCoroutine(ShowUI());
    }

    private IEnumerator StandBy()   //처음 시작하고 대기할 때(고개만 움직일 수 있음)
    {
        OnEnterStandbyStart?.Invoke();
        yield return new WaitForSeconds(onlyMovePauseTime);
    }
    private IEnumerator ShowUI()    //UI나타났을 때 모든 움직임 정지 및 문 바라보기
    {
        OnAttentionFix?.Invoke();
        yield return new WaitForSeconds(onlyMovePauseTime);
        OnAttentionFree?.Invoke();
        OnEnterStandbyEnd?.Invoke();
    }
}
