using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AbstractEnemy
{
    [SerializeField]
    private float damage;
    [SerializeField]
    private float idleMoveSpeed;
    [SerializeField]
    private float chaseMoveSpeed;

    [SerializeField]
    private float angleOffset;
    [SerializeField]
    private float senseRange;
    [SerializeField]
    private float immediateSenseRange;
    [SerializeField]
    private Timer idleTimer;
    [SerializeField]
    private float idleWalkRangeMin;
    [SerializeField]
    private float idleWalkRangeMax;
    private Timer loseTimer = new Timer(0.3f);
    private Timer idleWalkGiveupTimer = new Timer(2);

    private Vector3 _idleDestination;


    private EnemyState state;
    private enum EnemyState { Idle, IdleWalk, Chase, Wait }


    void Update()
    {
        if (_freezeCount > 0)
            return;

        Vector3 playerDelta = PlayerControl.ins.transform.position - transform.position;
        switch (state)
        {
            case EnemyState.Idle:
                if (playerDelta.sqrMagnitude <= senseRange * senseRange)
                {
                    state = EnemyState.Chase;
                }

                if (idleTimer.UpdateEnded())
                {
                    state = EnemyState.IdleWalk;
                    float range = Random.Range(idleWalkRangeMin, idleWalkRangeMax);
                    _idleDestination = transform.position + ((Vector3)Random.insideUnitCircle * range);
                }
                break;

            case EnemyState.IdleWalk:
                transform.position = Vector3.MoveTowards(transform.position, _idleDestination, idleMoveSpeed * Time.deltaTime);

                Vector3 destinationDelta = _idleDestination - transform.position;
                float angle = Mathf.Atan2(destinationDelta.y, destinationDelta.x) * 180 / Mathf.PI;
                transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);

                if (idleWalkGiveupTimer.UpdateEnded())
                {
                    state = EnemyState.Idle;
                    _rigidbody2D.velocity = Vector2.zero;
                    _rigidbody2D.angularVelocity = 0;
                }

                if ((transform.position - _idleDestination).sqrMagnitude <= 0.01f)
                {
                    state = EnemyState.Idle;
                    _rigidbody2D.velocity = Vector2.zero;
                    _rigidbody2D.angularVelocity = 0;
                }

                if (playerDelta.sqrMagnitude <= immediateSenseRange * immediateSenseRange)
                    state = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                HandleChasePlayer(playerDelta);

                if (playerDelta.sqrMagnitude >= senseRange * senseRange)
                {
                    if (loseTimer.UpdateEnded())
                    {
                        state = EnemyState.Idle;
                        _rigidbody2D.velocity = Vector2.zero;
                        _rigidbody2D.angularVelocity = 0;
                    }
                }
                else
                    loseTimer.Reset();
                break;
        }

    }

    void HandleChasePlayer(Vector3 playerDelta)
    {
        playerDelta.Normalize();

        _rigidbody2D.velocity = chaseMoveSpeed * playerDelta;

        float angle = Mathf.Atan2(playerDelta.y, playerDelta.x) * 180 / Mathf.PI;
        transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (state == EnemyState.IdleWalk)
        {
            state = EnemyState.Idle;
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0;
        }

        if (collision2D.collider.CompareTag("Player"))
        {
            PlayerControl.ins.Damage(damage, transform.position);
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        state = EnemyState.Wait;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0;
        yield return new WaitForSeconds(Random.Range(0.4f, 0.6f));
        state = EnemyState.Idle;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, senseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, immediateSenseRange);
    }
}
