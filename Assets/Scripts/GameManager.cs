using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
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
    public float jumpForceMultiplier = 5;
    public float maxJumpForce = 600;
    public AudioClip GameFinishSuccessAudioClip;
    public AudioClip GameFinishFailAudioClip;
    public AudioClip PickedStarAudioClip;
    public AudioClip PickedBadStarAudioClip;
    public AudioClip BackgroundMusic;
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

    private AudioSource gameManagerAudioSource;
    private AudioSource mainCameraAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        mainCameraAudioSource = Camera.main.GetComponent<AudioSource>();
        gameManagerAudioSource = GetComponent<AudioSource>();
        LevelMenu = FindObjectOfType<LevelMenu>();
        LevelMenu.ExitMenuClicked += LevelMenu_ExitMenuClicked;
        LevelMenu.ExitClicked += LevelMenu_ExitClicked;
        LevelMenu.NextLevelClicked += LevelMenu_NextLevelClicked;
        LevelMenu.ResumeClicked += LevelMenu_ResumeClicked;
        LevelMenu.StartClicked += LevelMenu_StartClicked;
        LevelMenu.LoadCheckpointClicked += LevelMenu_LoadCheckpointClicked;
        LevelMenu.TryAgainClicked += LevelMenu_TryAgainClicked;

        foreach (Star star in stars)
        {
            if (star.starType == Star.StarType.CheckPoint && stars.Where(curr => curr.starType == Star.StarType.CheckPoint &&
           curr.CheckPointIndex == star.CheckPointIndex).Count() > 1)
            {
                throw new System.Exception($"Error: two checkpoints have the same index {star.CheckPointIndex}");
            }
        }

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

    private void LevelMenu_LoadCheckpointClicked(object sender, System.EventArgs e)
    {
        LevelMenu.HideMenu();
        CheckPointHandler.CheckPoint checkPoint = CheckPointHandler.GetCheckPoint(Level);
        foreach (Star star in stars)
        {
            if (star.starType == Star.StarType.CheckPoint && star.CheckPointIndex < checkPoint.Index)
            {
                Destroy(star.gameObject);
            }
        }
        StartGame(checkPoint.Index, checkPoint.Time);
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
    bool clickedMouseDownEver = false;
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
            Debug.Log("Lost by time");
            FinishGame(new GameFinishArgs { Win = false });
            return;
        }

        //Input
        if (Input.GetMouseButtonDown(0))
        {
            xBefore = Input.mousePosition.x;
            yBefore = Input.mousePosition.y;
            clickedMouseDownEver = true;
        }
        else if (Input.GetMouseButton(0))
        {
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            force = new Vector2(xBefore - x, yBefore - y);
            float finalForceMagnitude = Mathf.Clamp(force.magnitude * jumpForceMultiplier, 0, maxJumpForce);
            Vector2 finalForce = force.normalized * finalForceMagnitude;
            player.Aim(finalForce);
        }
        else if (clickedMouseDownEver && Input.GetMouseButtonUp(0))
        {
            player.HideArrow();
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
            float finalForceMagnitude = Mathf.Clamp(force.magnitude * jumpForceMultiplier, 0, maxJumpForce);
            Vector2 finalForce = force.normalized * finalForceMagnitude;
            player.Jump(finalForce);
        }
    }

    public void PickedStar(Star.StarType starType)
    {
        mainCameraAudioSource.loop = false;
        if (starType == Star.StarType.Time)
        {
            mainCameraAudioSource.clip = PickedBadStarAudioClip;
        }
        else
        {
            mainCameraAudioSource.clip = PickedStarAudioClip;

        }
        mainCameraAudioSource.Play();
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
        StopBackgroundMusic();
        oldStatus = gameStatus;
        gameStatus = GameStatus.Pause;
        Time.timeScale = 0;
        LevelMenu.ShowMenu(LevelMenu.MenuType.MidLevelMenu);
    }

    void ResumeGame()
    {
        PlayBackgroundMusic();
        gameStatus = oldStatus;
        Time.timeScale = 1;
    }

    void StartGame(int checkpointIndex = -1, float time = 0)
    {
        PlayBackgroundMusic();
        gameStatus = GameStatus.Running;
        Time.timeScale = 1;
        float highScore = HighScoreManager.GetHighScore(Level);
        if (highScore != -1)
        {
            LevelMenu.SetHighScore(highScore);
        }

        Debug.Log($"Got here: checkINde: {checkpointIndex}, time: {time}");
        if (checkpointIndex != -1)
        {
            Star star = stars.Find(star => star.starType == Star.StarType.CheckPoint && star.CheckPointIndex == checkpointIndex);
            Vector2 position = star.gameObject.transform.position;
            Destroy(star.gameObject);
            player.transform.position = position;
            Seconds = time;
        }
    }

    void PlayBackgroundMusic()
    {
        gameManagerAudioSource.loop = true;
        gameManagerAudioSource.clip = BackgroundMusic;
        gameManagerAudioSource.Play();
    }

    void StopBackgroundMusic()
    {
        gameManagerAudioSource.Stop();
    }

    public void FinishGame(GameFinishArgs gameFinishArgs)
    {
        gameStatus = GameStatus.Finished;
        mainCameraAudioSource.loop = false;
        if (gameFinishArgs.Win)
        {
            HighScoreManager.StoreHighScore(Level, Seconds);
            mainCameraAudioSource.clip = GameFinishSuccessAudioClip;
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
            mainCameraAudioSource.clip = GameFinishFailAudioClip;
            LevelMenu.ShowMenu(LevelMenu.MenuType.LevelFailedMenu);
        }
        if (gameFinishArgs.DelayAudioClip != null)
        {
            mainCameraAudioSource.PlayDelayed((int)gameFinishArgs.DelayAudioClip);
        }
        else
        {
            mainCameraAudioSource.Play();
        }
    }
}

