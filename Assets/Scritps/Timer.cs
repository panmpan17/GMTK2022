using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Timer
{
    public float TargetTime;

    private float _runTime;

    public float Progress => _runTime / TargetTime;

    public Timer(float targetTime)
    {
        TargetTime = targetTime;
    }

    public bool UpdateEnded()
    {
        _runTime += Time.deltaTime;
        if (_runTime >= TargetTime)
        {
            _runTime = 0;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        _runTime = 0;
    }
}
