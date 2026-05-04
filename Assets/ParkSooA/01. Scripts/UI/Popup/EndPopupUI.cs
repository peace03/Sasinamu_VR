using UnityEngine;

public class EndPopupUI : PopupUI
{
    private void Start()
    {
        // 초기화
        PopupUIHandler(true);
    }

    // 게임 종료 버튼 선택 함수
    public void SelectGameQuitButton()
    {
        // 유니티 에디터라면
#if UNITY_EDITOR
        // 실행 종료
        UnityEditor.EditorApplication.isPlaying = false;
        // 그 외라면
#else
            // 게임 종료
            Application.Quit();
#endif
    }
}