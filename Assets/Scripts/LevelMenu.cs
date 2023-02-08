using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static LevelMenu;

public class LevelMenu : MonoBehaviour
{
    public enum MenuType
    {
        StartLevelMenu,
        MidLevelMenu,
        LevelFinishedMenu,
        LevelFailedMenu,
        GameFinishedMenu
    }

    private MenuType shownMenuType;

    [SerializeField]
    private CanvasGroup StartLevelMenu;
    [SerializeField]
    private CanvasGroup MidLevelMenu;
    [SerializeField]
    private CanvasGroup LevelFinishedMenu;
    [SerializeField]
    private CanvasGroup LevelFailedMenu;
    [SerializeField]
    private CanvasGroup GameFinishedMenu;
    [SerializeField]
    private TextMeshProUGUI GlobalSecondsText;

    [HideInInspector]
    public event EventHandler StartClicked;
    public event EventHandler ResumeClicked;
    public event EventHandler NextLevelClicked;
    public event EventHandler ExitClicked;
    public event EventHandler ExitMenuClicked;
    public event EventHandler TryAgainClicked;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideMenu()
    {
        switch (shownMenuType)
        {
            case MenuType.StartLevelMenu:
                {
                    HideCanvasGroup(StartLevelMenu);
                    break;
                }
            case MenuType.MidLevelMenu:
                {
                    HideCanvasGroup(MidLevelMenu);
                    break;
                }
            case MenuType.LevelFinishedMenu:
                {
                    HideCanvasGroup(LevelFinishedMenu);
                    break;
                }
            case MenuType.LevelFailedMenu:
                {
                    HideCanvasGroup(LevelFailedMenu);
                    break;
                }
            case MenuType.GameFinishedMenu:
                {
                    HideCanvasGroup(GameFinishedMenu);
                    break;
                }
        }
    }

    public void ShowMenu(MenuType menuType)
    {
        switch (menuType)
        {
            case MenuType.StartLevelMenu:
                {
                    ShowCanvasGroup(StartLevelMenu);
                    break;
                }
            case MenuType.MidLevelMenu:
                {
                    ShowCanvasGroup(MidLevelMenu);
                    break;
                }
            case MenuType.LevelFinishedMenu:
                {
                    ShowCanvasGroup(LevelFinishedMenu);
                    break;
                }
            case MenuType.LevelFailedMenu:
                {
                    ShowCanvasGroup(LevelFailedMenu);
                    break;
                }
            case MenuType.GameFinishedMenu:
                {
                    ShowCanvasGroup(GameFinishedMenu);
                    break;
                }
        }
        shownMenuType = menuType;
    }

    public void SetSeconds(float seconds)
    {
        GlobalSecondsText.text = String.Format("{0:0.00}", seconds);
    }

    private void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnStartClick()
    {
        StartClicked.Invoke(this, null);
    }

    public void OnResumeClick()
    {
        ResumeClicked.Invoke(this, null);
    }

    public void OnNextLevelClick()
    {
        NextLevelClicked.Invoke(this, null);
    }

    public void OnExitToMenuClick()
    {
        ExitMenuClicked.Invoke(this, null);
    }

    public void OnTryAgainClick()
    {
        Debug.Log("WHAT!");
        TryAgainClicked.Invoke(this, null);
    }

    public void OnExitClick()
    {
        ExitClicked.Invoke(this, null);
    }
}
