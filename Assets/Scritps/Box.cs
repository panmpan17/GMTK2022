using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField]
    private float health;
    [SerializeField]
    private new Collider2D collider2D;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private SoundClip breakSound;


    public void Damage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            // Destroy(gameObject);
            collider2D.enabled = false;
            spriteRenderer.enabled = false;
            breakSound.Play(audioSource);
        }
    }
}
