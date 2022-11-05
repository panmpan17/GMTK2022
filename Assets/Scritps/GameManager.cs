using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager ins;

    [SerializeField]
    private GameObject win;
    [SerializeField]
    private GameObject lose;

    [SerializeField]
    private float loseDelay;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private SoundClip loseSound;

    void Awake()
    {
        ins = this;
    }

    public void ShowWin()
    {
        Time.timeScale = 0;
        win.SetActive(true);
    }

    public void ShowLose()
    {
        Time.timeScale = 0;
        StartCoroutine(DisplayLose());
    }

    IEnumerator DisplayLose()
    {
        loseSound.Play(audioSource);
        yield return new WaitForSecondsRealtime(loseDelay);
        lose.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }


    public void Replay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }
}
