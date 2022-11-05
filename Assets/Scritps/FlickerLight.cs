using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(UnityEngine.Rendering.Universal.Light2D))]
public class FlickerLight : MonoBehaviour
{
    [SerializeField]
    private Timer timer; 
    [SerializeField]
    private float min;
    [SerializeField]
    private float max;

    private UnityEngine.Rendering.Universal.Light2D _light2D;
    private float _lerpFrom;
    private float _lerpTo;

    void Awake()
    {
        _light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        _lerpFrom = _light2D.intensity;
        _lerpTo = Random.Range(min, max);
    }

    void Update()
    {
        if (timer.UpdateEnded())
        {
            _lerpFrom = _light2D.intensity;
            _lerpTo = Random.Range(min, max);
        }
        else
        {
            _light2D.intensity = Mathf.Lerp(_lerpFrom, _lerpTo, timer.Progress);
        }
    }
}
