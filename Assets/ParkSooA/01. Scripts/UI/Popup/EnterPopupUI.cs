using System.Collections;
using UnityEngine;

public class EnterPopupUI : PopupUI
{
    [Header("UI들")]
    [SerializeField] private GameObject[] enterUis;

    private int curIndex = 0;

    private void OnEnable()
    {
        PopupUIHandler(true);
    }

    private void OnDisable()
    {
        PopupUIHandler(false);
    }

    public void NextPage()
    {
        enterUis[curIndex].SetActive(false);
        enterUis[curIndex++].SetActive(true);
    }
}