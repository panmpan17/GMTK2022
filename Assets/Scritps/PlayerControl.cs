using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl ins;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float angleOffset;

    [SerializeField]
    private float knockbackForce;
    [SerializeField]
    private Timer knockbackTimer;
    private bool _isKnockback;
    [SerializeField]
    private Timer invincibleTimer;
    [SerializeField]
    private Timer invincibleBlinkTimer;
    [SerializeField]
    private Color blinkColor;
    private bool _isInvincible;


    [Header("Sprite")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private Sprite holdSprite;
    [SerializeField]
    private Sprite[] walkSprites;
    [SerializeField]
    private Timer walkAnimaitionInterval;
    private int walkSpriteIndex;

    [Header("Dice")]
    [SerializeField]
    private Collider2D detectDice;
    [SerializeField]
    private Dice dice;
    [SerializeField]
    private float throwPower;

    [SerializeField]
    private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;
    [SerializeField]
    private AudioSource footstepAudioSource;

    [Header("Health")]
    [SerializeField]
    private FillBarControl healthBarPrefab;
    private FillBarControl _healthBar;
    [SerializeField]
    private float maxHealth;
    private float _health;

    [Header("Tutorual")]
    [SerializeField]
    private float sqrMagnitudeRequired;
    [SerializeField]
    private UnityEvent onReachEvent;
    private float sqrMagnitudeAccumulate;
    private bool sqrMagnitudeReached;

    [SerializeField]
    private UnityEvent onGrabEvent;
    [SerializeField]
    private UnityEvent onThrowEvent;

    private InputScheme _inputScheme;
    private Vector2 _movementAxis;
    private Rigidbody2D _rigidbody2D;
    public Rigidbody2D Rigidbody2D => _rigidbody2D;
    public Collider2D Collider { get; private set; }

    private bool _diceGrabbed;
    private bool _keyReleased;
    public bool DiceGrabbed => _diceGrabbed;

    private bool _walking;

    private List<string> keys = new List<string>();

    [SerializeField]
    private Transform dicePoint;
    public Vector3 DicePosition => dicePoint.position;

    void Awake()
    {
        ins = this;

        _rigidbody2D = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();

        _inputScheme = new InputScheme();
        _inputScheme.Game.Movement.started += MovementChanged;
        _inputScheme.Game.Movement.performed += MovementChanged;
        _inputScheme.Game.Movement.canceled += MovementChanged;


        _inputScheme.Game.Throw.started += ThrowStarted;
        _inputScheme.Game.Throw.canceled += ThrowEnded;


        _healthBar = Instantiate(healthBarPrefab);
        _healthBar.GetComponent<TransformFollower>().SetTarget(transform);

        _healthBar.SetFillAmount(1);
        _health = maxHealth;
    }

    void OnEnable()
    {
        _inputScheme.Enable();
    }

    void OnDisable()
    {
        _inputScheme.Disable();
    }


    void Update()
    {
        if (_isInvincible)
        {
            if (invincibleBlinkTimer.UpdateEnded())
            {
                if (spriteRenderer.color == Color.white)
                    spriteRenderer.color = blinkColor;
                else
                    spriteRenderer.color = Color.white;
            }

            if (invincibleTimer.UpdateEnded())
            {
                spriteRenderer.color = Color.white;
                _isInvincible = false;
            }
        }
        if (_isKnockback)
        {
            if (knockbackTimer.UpdateEnded())
                _isKnockback = false;
            return;
        }

        float sqrMagnitude = _movementAxis.sqrMagnitude;
        if (sqrMagnitude > 0.01f)
        {
            if (!sqrMagnitudeReached)
            {
                sqrMagnitudeAccumulate += sqrMagnitude;
                if (sqrMagnitudeAccumulate >= sqrMagnitudeRequired)
                {
                    onReachEvent.Invoke();
                    sqrMagnitudeReached = true;
                }
            }

            _rigidbody2D.velocity = _movementAxis * moveSpeed;

            float angle = Mathf.Atan2(_movementAxis.y, _movementAxis.x) * 180 / Mathf.PI;
            transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);

            if (!_walking)
            {
                _walking = true;
                footstepAudioSource.Play();
            }

            if (!_diceGrabbed && walkAnimaitionInterval.UpdateEnded())
            {
                if (++walkSpriteIndex >= walkSprites.Length)
                {
                    walkSpriteIndex = 0;
                }
                spriteRenderer.sprite = walkSprites[walkSpriteIndex];
            }
        }
        else
        {
            _rigidbody2D.velocity = Vector2.zero;

            if (_walking)
            {
                _walking = false;
                footstepAudioSource.Stop();

                if (!_diceGrabbed)
                {
                    walkSpriteIndex = 0;
                    spriteRenderer.sprite = sprite;
                }
            }
        }
    }


    void MovementChanged(CallbackContext callbackContext)
    {
        _movementAxis = callbackContext.ReadValue<Vector2>();
    }

    void ThrowStarted(CallbackContext callbackContext)
    {
        if (_diceGrabbed)
            dice.PrepareThrow();
        else
        {
            if (detectDice.OverlapPoint(dice.transform.position))
            {
                _diceGrabbed = true;
                dice.OnGrab();
                dice.transform.position = _rigidbody2D.position;
                dice.transform.rotation = transform.rotation;

                _keyReleased = false;
                spriteRenderer.sprite = holdSprite;

                onGrabEvent.Invoke();
            }
        }
    }

    void ThrowEnded(CallbackContext callbackContext)
    {
        if (!_keyReleased)
        {
            _keyReleased = true;
            return;
        }

        if (_diceGrabbed)
        {
            _diceGrabbed = false;
            dice.Throw(detectDice.transform.position, detectDice.transform.rotation, transform.up.normalized * throwPower);
            spriteRenderer.sprite = sprite;

            onThrowEvent.Invoke();
        }
    }

    public void Damage(float amount)
    {
        if (_isInvincible)
            return;

        _health -= amount;
        _healthBar.SetFillAmount(_health / maxHealth);

        if (_health <= 0)
        {
            GameManager.ins.ShowLose();
        }
    }
    public void Damage(float amount, Vector3 enemyPosition)
    {
        if (_isInvincible)
            return;

        Vector3 delta = transform.position - enemyPosition;
        _rigidbody2D.velocity = delta.normalized * knockbackForce;
        _isKnockback = true;
        knockbackTimer.Reset();
        _isInvincible = true;
        invincibleTimer.Reset();

        _health -= amount;
        _healthBar.SetFillAmount(_health / maxHealth);

        if (_health <= 0)
        {
            GameManager.ins.ShowLose();
        }
    }


#region key
    public void AddKey(string keyName)
    {
        keys.Add(keyName);
    }

    public bool HasKey(string keyName)
    {
        return keys.Contains(keyName);
    }

    public void RemoveKey(string keyName)
    {
        keys.Remove(keyName);
    }
#endregion
}
