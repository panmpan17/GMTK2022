using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Header("Health")]
    [SerializeField]
    protected FillBarControl healthBarPrefab;
    protected FillBarControl _healthBar;
    [SerializeField]
    protected float maxHealth;
    protected float _health;


    protected int _freezeCount;
    protected Rigidbody2D _rigidbody2D;


    protected virtual void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _healthBar = Instantiate(healthBarPrefab);
        _healthBar.GetComponent<TransformFollower>().SetTarget(transform);

        _healthBar.SetFillAmount(1);
        _health = maxHealth;
    }

    public void Damage(float amount)
    {
        _health -= amount;
        _healthBar.SetFillAmount(_health / maxHealth);

        if (_health <= 0)
        {
            HandleDeath();
            return;
        }
    }

    protected virtual void HandleDeath()
    {
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0;
        _rigidbody2D.simulated = false;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(DeadAnimation());
    }

    IEnumerator DeadAnimation()
    {
        Color color = spriteRenderer.color;

        Timer timer = new Timer(0.5f);
        while (!timer.UpdateEnded())
        {
            color.a = 1 - timer.Progress;
            spriteRenderer.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void Freeze()
    {
        _freezeCount++;
        _rigidbody2D.velocity = Vector3.zero;
        _rigidbody2D.angularVelocity = 0;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Unfreeze()
    {
        if (--_freezeCount <= 0)
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
