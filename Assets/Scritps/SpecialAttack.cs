using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    [SerializeField]
    private float destroyAfter;

    [SerializeField]
    private bool freeze;
    [SerializeField]
    private bool followEnemy;

    [Header("Constant Damge")]
    [SerializeField]
    private bool constantDamge;
    [SerializeField]
    private float contantDamagePoint;
    [SerializeField]
    private Timer damageInterval;

    private AbstractEnemy _enemy;

    public void Setup(AbstractEnemy enemy, float percentage)
    {
        _enemy = enemy;
        transform.position = _enemy.transform.position;

        if (freeze || constantDamge)
            destroyAfter *= percentage;

        if (freeze)
            _enemy.Freeze();

        Destroy(gameObject, destroyAfter);
    }

    void Update()
    {
        if (!_enemy)
        {
            Destroy(gameObject);
            return;
        }

        if (followEnemy)
        {
            transform.SetPositionAndRotation(_enemy.transform.position, _enemy.transform.rotation);
        }

        if (constantDamge && damageInterval.UpdateEnded())
        {
            _enemy.Damage(contantDamagePoint);
        }
    }

    void OnDestroy()
    {
        if (freeze)
        {
            if (_enemy)
                _enemy.Unfreeze();
        }
    }
}
