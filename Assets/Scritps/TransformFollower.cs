using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 positionOffset;
    [SerializeField]
    private bool useFixedUpdate;

    void LateUpdate()
    {
        if (!useFixedUpdate)
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
            transform.position = target.position + positionOffset;
        }
    }

    void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
            transform.position = target.position + positionOffset;
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    public void SetTarget(Transform target, Vector3 offset)
    {
        this.target = target;
        positionOffset = offset;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!UnityEditor.EditorApplication.isPlaying && target != null)
        {
            transform.position = target.position + positionOffset;
        }
    }
#endif
}
