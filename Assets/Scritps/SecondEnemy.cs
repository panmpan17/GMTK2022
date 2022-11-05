using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondEnemy : AbstractEnemy
{
    [SerializeField]
    private float damage;
    [SerializeField]
    private float idleMoveSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private Timer runTimer;
    private Vector3 _runDirection;

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
    private Timer _loseTimer = new Timer(0.3f);
    private Timer _idleWalkGiveupTimer = new Timer(2);
    private Timer _waitBeforeRunTimer = new Timer(0.3f);

    [SerializeField]
    private ParticleSystem dustParticle;

    private Vector3 _idleDestination;


    private EnemyState state;
    private enum EnemyState { Idle, IdleWalk, WaitRun, Run, Wait }


    void Update()
    {
        if (_freezeCount > 0)
            return;

        Vector3 playerDelta = PlayerControl.ins.transform.position - transform.position;
        float angle;
        switch (state)
        {
            case EnemyState.Idle:
                if (playerDelta.sqrMagnitude <= senseRange * senseRange)
                {
                    StartRun(playerDelta);
                    return;
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
                angle = Mathf.Atan2(destinationDelta.y, destinationDelta.x) * 180 / Mathf.PI;
                transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);

                if (_idleWalkGiveupTimer.UpdateEnded())
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
                    StartRun(playerDelta);
                break;

            case EnemyState.WaitRun:
                if (_waitBeforeRunTimer.UpdateEnded())
                    state = EnemyState.Run;
                break;

            case EnemyState.Run:
                _rigidbody2D.velocity = runSpeed * _runDirection;

                angle = Mathf.Atan2(_runDirection.y, _runDirection.x) * 180 / Mathf.PI;
                transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);

                if (runTimer.UpdateEnded())
                {
                    dustParticle.Stop();
                    StartCoroutine(Wait());
                }
                break;
        }

    }

    void StartRun(Vector3 playerDelta)
    {
        _runDirection = playerDelta.normalized;
        state = EnemyState.WaitRun;
        dustParticle.Play();
        runTimer.Reset();


        float angle = Mathf.Atan2(_runDirection.y, _runDirection.x) * 180 / Mathf.PI;
        transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
    }


    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (state == EnemyState.IdleWalk)
        {
            dustParticle.Stop();
            state = EnemyState.Idle;
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0;
        }

        if (collision2D.collider.CompareTag("Player"))
        {
            PlayerControl.ins.Damage(damage, transform.position);
            StartCoroutine(Wait());
            dustParticle.Stop();
        }
    }

    IEnumerator Wait()
    {
        state = EnemyState.Wait;
        _rigidbody2D.velocity = Vector3.zero;
        yield return new WaitForSeconds(Random.Range(0.7f, 1.1f));
        state = EnemyState.Idle;
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        dustParticle.Stop();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, senseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, immediateSenseRange);
    }
}
