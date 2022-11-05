using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private TutorialBubble bubble;
    [SerializeField]
    private Step[] steps;
    private int _index;

    void Awake()
    {
        bubble.Set(steps[0].BubbleTarget, steps[0].Text);
    }

    public void SkipStep(int index)
    {
        if (_index == index)
        {
            if (++_index >= steps.Length)
            {
                bubble.gameObject.SetActive(false);
                gameObject.SetActive(false);
                return;
            }
            bubble.Set(steps[_index].BubbleTarget, steps[_index].Text);
        }
    }

    [System.Serializable]
    private class Step
    {
        public Transform BubbleTarget;
        public string Text;
    }
}
