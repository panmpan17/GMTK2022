using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField]
    private string id;
    [SerializeField]
    private SoundClip pickupSound;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player"))
            return;

        PlayerControl.ins.AddKey(id);
        Destroy(gameObject);

        pickupSound.Play(PlayerControl.ins.AudioSource);
    }
}
