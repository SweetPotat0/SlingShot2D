using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public struct GameFinishArgs
    {
        public bool Win;
        public int? DelayAudioClip;
    }

    public enum GameStatus
    {
        Pause,
        Running,
        Finished
    }

    public int LevelCount = 1;
    public int Level;
    public float forceMultiplier = 1;
    public AudioClip GameFinishSuccessAudioClip;
    public AudioClip GameFinishFailAudioClip;
    public AudioClip PickedStarAudioClip;
    public AudioClip PickedBadStarAudioClip;
    public float FloorYLocation = -5;
    public float MaxLevelTime = 15;

    [HideInInspector]
    public GameStatus gameStatus = GameStatus.Pause;
    [HideInInspector]
    public List<Platform> platforms = new List<Platform>();
    [HideInInspector]
    public List<Star> stars = new List<Star>();
    [HideInInspector]
    public SlingShotPlayer player;
    [HideInInspector]
    public LevelMenu LevelMenu;
    private float seconds;
    [HideInInspector]
    public float Seconds
    {
        get { return seconds; }
        set
        {
            seconds = value > 0 ? value : 0;
        }
    }

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = Camera.main.GetComponent<AudioSource>();
        LevelMenu = FindObjectOfType<LevelMenu>();
        LevelMenu.ExitMenuClicked += LevelMenu_ExitMenuClicked;
        LevelMenu.ExitClicked += LevelMenu_ExitClicked;
        LevelMenu.NextLevelClicked += LevelMenu_NextLevelClicked;
        LevelMenu.ResumeClicked += LevelMenu_ResumeClicked;
        LevelMenu.StartClicked += LevelMenu_StartClicked;
        LevelMenu.TryAgainClicked += LevelMenu_TryAgainClicked;
        InitGame();
    }

    private void LevelMenu_TryAgainClicked(object sender, System.EventArgs e)
    {
        Debug.Log("Try again");
        SceneManager.LoadSceneAsync(Level);
    }

    private void LevelMenu_StartClicked(object sender, System.EventArgs e)
    {
        LevelMenu.HideMenu();
        StartGame();
    }

    private void LevelMenu_ResumeClicked(object sender, System.EventArgs e)
    {
        LevelMenu.HideMenu();
        ResumeGame();
    }



    private void LevelMenu_NextLevelClicked(object sender, System.EventArgs e)
    {
        if (LevelCount > Level)
        {
            SceneManager.LoadSceneAsync(Level);
        }
        else
        {
            //Shouldnt come here
        }
    }

    private void LevelMenu_ExitClicked(object sender, System.EventArgs e)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void LevelMenu_ExitMenuClicked(object sender, System.EventArgs e)
    {
        SceneManager.LoadSceneAsync(0);
    }

    bool touchedBefore = false;
    float xBefore = 0;
    float yBefore = 0;
    Vector2 force = new Vector2();
    // Update is called once per frame
    void Update()
    {
        if (gameStatus != GameStatus.Running) { return; }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        Seconds += Time.deltaTime;
        LevelMenu.SetSeconds(Seconds);
        if (Seconds > MaxLevelTime)
        {
            FinishGame(new GameFinishArgs { Win = false });
            return;
        }

        //Input
        if (Input.GetMouseButtonDown(0))
        {
            xBefore = Input.mousePosition.x;
            yBefore = Input.mousePosition.y;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            force = new Vector2(xBefore - x, yBefore - y);
            touchedBefore = true;
        }

        //Check if jump
        if (player.playerMode == SlingShotPlayer.PlayerMode.Grounded && touchedBefore)
        {
            touchedBefore = false;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.Jump(force * forceMultiplier);
        }
    }

    public void PickedStar(Star.StarType starType)
    {
        if (starType == Star.StarType.Time)
        {
            audioSource.clip = PickedBadStarAudioClip;
        }
        else
        {
            audioSource.clip = PickedStarAudioClip;

        }
        audioSource.Play();
    }

    private GameStatus oldStatus;

    void InitGame()
    {
        gameStatus = GameStatus.Pause;
        Time.timeScale = 0;
        LevelMenu.ShowMenu(LevelMenu.MenuType.StartLevelMenu);
    }
    void PauseGame()
    {
        oldStatus = gameStatus;
        gameStatus = GameStatus.Pause;
        Time.timeScale = 0;
        LevelMenu.ShowMenu(LevelMenu.MenuType.MidLevelMenu);
    }

    void ResumeGame()
    {
        gameStatus = oldStatus;
        Time.timeScale = 1;
    }

    void StartGame()
    {
        gameStatus = GameStatus.Running;
        Time.timeScale = 1;
    }

    public void FinishGame(GameFinishArgs gameFinishArgs)
    {
        gameStatus = GameStatus.Finished;
        if (gameFinishArgs.Win)
        {
            audioSource.clip = GameFinishSuccessAudioClip;
            if (LevelCount > Level)
            {
                LevelMenu.ShowMenu(LevelMenu.MenuType.LevelFinishedMenu);
            }
            else
            {
                LevelMenu.ShowMenu(LevelMenu.MenuType.GameFinishedMenu);
            }
        }
        else
        {
            audioSource.clip = GameFinishFailAudioClip;
            LevelMenu.ShowMenu(LevelMenu.MenuType.LevelFailedMenu);
        }
        if (gameFinishArgs.DelayAudioClip != null)
        {
            audioSource.PlayDelayed((int)gameFinishArgs.DelayAudioClip);
        }
        else
        {
            audioSource.Play();
        }
    }
}

