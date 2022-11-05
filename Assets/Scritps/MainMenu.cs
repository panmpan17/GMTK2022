using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject[] steps;
    private int stepIndex;

    void Awake()
    {
        for (int i = 0; i < steps.Length; i++)
            steps[i].SetActive(false);
    }

    public void StarStep()
    {
        steps[0].SetActive(true);
    }

    public void NextStep()
    {
        if (++stepIndex >= steps.Length)
        {
            SceneManager.LoadScene("SampleScene");
            return;
        }

        steps[stepIndex].SetActive(true);
    }
}
