using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject Label;
    public GameObject Buttons;
    public GameObject StoryContainer;
    private int StorySlidesCount = 0;
    private int StorySlideIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        StorySlidesCount = StoryContainer.transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnStartClick()
    {
        if (PlayerPrefs.HasKey("FirstTime") == false)
        {
            //First time in the game. Show Story
            PlayerPrefs.SetInt("FirstTime", 2);
            ShowStory();
        }
        else
        {
            SceneManager.LoadSceneAsync(1);
        }
    }

    public void ShowStory()
    {
        Label.SetActive(false);
        Buttons.SetActive(false);
        StoryContainer.SetActive(true);
        StoryContainer.GetComponent<AudioSource>().Play();
        MoveStorySlide();
    }

    public void MoveStorySlide()
    {
        if (StorySlideIndex != 0)
        {
            StoryContainer.transform.GetChild(StorySlideIndex - 1).gameObject.SetActive(false);
        }
        if (StorySlideIndex == StorySlidesCount)
        {
            SceneManager.LoadSceneAsync(1);
            return;
        }
        Debug.Log($"{StorySlideIndex} is not equal to {StorySlidesCount}");
        StoryContainer.transform.GetChild(StorySlideIndex).gameObject.SetActive(true);
        StorySlideIndex++;
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
