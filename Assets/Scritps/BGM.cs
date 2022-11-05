using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private static BGM ins;

    void Awake()
    {
        if (ins)
        {
            Destroy(gameObject);
            return;
        }

        ins = this;
        DontDestroyOnLoad(gameObject);
    }
}
