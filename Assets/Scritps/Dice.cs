using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public enum AttackType
{
    Normal,
    Lightning,
    Iced,
    Fire,
}

public class Dice : MonoBehaviour
{
    [SerializeField]
    private PlayerControl player;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float beforeTouchedLinearDrag;
    [SerializeField]
    private float afterTouchedLinearDrag;

    [Header("Rotate")]
    [SerializeField]
    private Vector3 rotateAccerate;
    [SerializeField]
    private Vector3 rotateMaxSpeed;
    private Vector3 _rotateSpeed;
    [SerializeField]
    private AnimationCurve curve;

    [Header("Attacks")]
    [SerializeField]
    private float normalAttackPoint;
    public Chance normalAttackChance;
    [SerializeField]
    private Sprite normalAttackFace;

    [SerializeField]
    private Lightning lightningEffectPrefab;
    public Chance lightningAttackChance;
    [SerializeField]
    private Sprite lightningAttackFace;

    [SerializeField]
    private SpecialAttack icedEffectPrefab;
    public Chance icedAttackChance;
    [SerializeField]
    private Sprite icedAttackFace;

    [SerializeField]
    private SpecialAttack fireEffectPrefab;
    public Chance fireAttackChance;
    [SerializeField]
    private Sprite fireAttackFace;

    public event System.Action onChanceChanged;

    [Header("SoundEffect")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private SoundClip startSpin;
    [SerializeField]
    private SoundClip repeatSpin;
    [SerializeField]
    private SoundClip throwSound;
    [SerializeField]
    private SoundClip hitWallSound;
    [SerializeField]
    private SoundClip hitBoxSound;
    [SerializeField]
    private SoundClip normalHitSound;


    public Collider2D Collider { get; private set; }

    private Rigidbody2D _rigidbody2D;
    private bool _prepareThrow;
    private bool _touched;
    
    private AttackType _attackType;

    void Awake()
    {
        Collider = GetComponent<Collider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.drag = afterTouchedLinearDrag;
    }

    public void OnGrab()
    {
        Collider.enabled = false;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0;
        _rigidbody2D.simulated = false;
    }

    void Update()
    {
        if (!PlayerControl.ins.DiceGrabbed)
            return;

        if (_prepareThrow)
        {
            if (_rotateSpeed.z != rotateMaxSpeed.z)
            {
                _rotateSpeed += rotateAccerate * Time.deltaTime;
                if (_rotateSpeed.z >= rotateMaxSpeed.z)
                {
                    _rotateSpeed.z = rotateMaxSpeed.z;
                    repeatSpin.Play(audioSource);
                }
            }

            transform.Rotate(_rotateSpeed * Time.deltaTime);
            transform.position = player.DicePosition;
            return;
        }

        transform.SetPositionAndRotation(player.DicePosition, player.transform.rotation);
    }

    public void PrepareThrow()
    {
        _prepareThrow = true;
        _rotateSpeed = Vector3.zero;

        startSpin.Play(audioSource);
    }

    public void Throw(Vector3 position, Quaternion rotation, Vector2 velocity)
    {
        _prepareThrow = false;

        Collider.enabled = true;
        // transform.SetPositionAndRotation(position, rotation);
        transform.position = position;

        Collider.enabled = true;
        _rigidbody2D.velocity = velocity;
        _rigidbody2D.angularVelocity = _rotateSpeed.z;
        _rigidbody2D.simulated = true;

        _rigidbody2D.drag = beforeTouchedLinearDrag;

        Physics2D.IgnoreCollision(Collider, player.Collider, true);

        _touched = false;
        throwSound.Play(audioSource);

        _attackType = DecideAttackType();

        switch (_attackType)
        {
            case AttackType.Normal:
                spriteRenderer.sprite = normalAttackFace;
                break;
            case AttackType.Lightning:
                spriteRenderer.sprite = lightningAttackFace;
                break;
            case AttackType.Iced:
                spriteRenderer.sprite = icedAttackFace;
                break;
            case AttackType.Fire:
                spriteRenderer.sprite = fireAttackFace;
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Physics2D.IgnoreCollision(Collider, player.Collider, false);

        if (collision.collider.CompareTag("Box"))
        {
            float attackPercentage = curve.Evaluate(_rigidbody2D.angularVelocity);

            var box = collision.collider.GetComponent<Box>();
            box.Damage(_rigidbody2D.velocity.sqrMagnitude);
            hitBoxSound.Play(audioSource);
            return;
        }

        if (collision.collider.CompareTag("Wall"))
        {
            hitWallSound.Play(audioSource);
        }

        if (_touched)
            return;

        _touched = true;
        _rigidbody2D.drag = afterTouchedLinearDrag;

        var enemy = collision.collider.GetComponent<AbstractEnemy>();
        if (enemy)
        {
            HandleEnemyDamage(enemy);
        }
    }

    AttackType DecideAttackType()
    {
        float totalChance = normalAttackChance.Amount + lightningAttackChance.Amount + icedAttackChance.Amount + fireAttackChance.Amount;
        float chance = Random.Range(0, totalChance);

        if (chance < normalAttackChance.Amount)
        {
            normalAttackChance.Reset();

            lightningAttackChance.Increment();
            icedAttackChance.Increment();
            fireAttackChance.Increment();
            return AttackType.Normal;
        }

        chance -= normalAttackChance.Amount;
        if (chance < lightningAttackChance.Amount)
        {
            lightningAttackChance.Reset();

            normalAttackChance.Increment();
            icedAttackChance.Increment();
            fireAttackChance.Increment();
            return AttackType.Lightning;
        }

        chance -= lightningAttackChance.Amount;
        if (chance < icedAttackChance.Amount)
        {
            icedAttackChance.Reset();

            normalAttackChance.Increment();
            lightningAttackChance.Increment();
            fireAttackChance.Increment();
            return AttackType.Iced;
        }

        fireAttackChance.Reset();

        normalAttackChance.Increment();
        lightningAttackChance.Increment();
        icedAttackChance.Increment();
        return AttackType.Fire;
    }

    void HandleEnemyDamage(AbstractEnemy enemy)
    {
        float attackPercentage = curve.Evaluate(_rigidbody2D.angularVelocity / rotateMaxSpeed.z);

        switch (_attackType)
        {
            case AttackType.Normal:
                enemy.Damage(normalAttackPoint * attackPercentage);
                normalHitSound.Play(audioSource);
                break;

            case AttackType.Lightning:
                Instantiate(lightningEffectPrefab).Setup(enemy, attackPercentage);
                normalHitSound.Play(audioSource);
                break;

            case AttackType.Iced:
                Instantiate(icedEffectPrefab).Setup(enemy, attackPercentage);
                normalHitSound.Play(audioSource);
                break;

            case AttackType.Fire:
                Instantiate(fireEffectPrefab).Setup(enemy, attackPercentage);
                normalHitSound.Play(audioSource);
                break;
        }

        onChanceChanged?.Invoke();
    }
}


[System.Serializable]
public struct Chance
{
    [Range(0, 1)]
    public float Base;

    [Range(0, 1)]
    public float DynamcIncrement;
    [Range(0, 1)]
    public float DynamcMaximum;

    private float _dynamic;

    public void Increment()
    {
        _dynamic += DynamcIncrement;
        if (_dynamic >= DynamcMaximum)
            _dynamic = DynamcMaximum;
    }

    public void Reset()
    {
        _dynamic = 0;
    }

    public float Amount => Base + _dynamic;
}


#if UNITY_EDITOR
[CustomEditor(typeof(Dice))]
public class DiceEditor : Editor
{
    private Dice _dice;

    void OnEnable()
    {
        _dice = (Dice) target;
        _dice.onChanceChanged += Repaint;
    }

    void OnDisable()
    {
        _dice.onChanceChanged -= Repaint;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        float totalChance = _dice.normalAttackChance.Amount +
                            _dice.lightningAttackChance.Amount +
                            _dice.icedAttackChance.Amount +
                            _dice.fireAttackChance.Amount;

        EditorGUILayout.HelpBox(string.Format(
            "普通: {0}%\n閃電: {1}%\n冰凍: {2}%\n燃燒: {3}%",
            Mathf.RoundToInt(_dice.normalAttackChance.Amount / totalChance * 100),
            Mathf.RoundToInt(_dice.lightningAttackChance.Amount / totalChance * 100),
            Mathf.RoundToInt(_dice.icedAttackChance.Amount / totalChance * 100),
            Mathf.RoundToInt(_dice.fireAttackChance.Amount / totalChance * 100)
            ), MessageType.Info);
    }
}
#endif
