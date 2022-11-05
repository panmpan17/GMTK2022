using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEventDispatcher : MonoBehaviour
{
    [SerializeField]
    private string compareTag;

    [SerializeField]
    private UnityEvent onTriggerEnter;

    public void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (compareTag != "" && !collider2D.CompareTag(compareTag))
            return;

        onTriggerEnter.Invoke();
    }
}
