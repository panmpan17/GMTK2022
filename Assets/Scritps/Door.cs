using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private string requireKeyId;
    [SerializeField]
    private bool removeKey;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite openSprite;
    [SerializeField]
    private new Collider2D collider2D;

    [SerializeField]
    private GameObject needKeyIndicate;
    [SerializeField]
    private float needKeyShowDuration;

    private bool _opened;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (_opened)
            return;
        if (!collider.CompareTag("Player"))
            return;

        if (PlayerControl.ins.HasKey(requireKeyId))
        {
            if (removeKey)
                PlayerControl.ins.RemoveKey(requireKeyId);
            
            OpenDoor();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShowNeedKeyyPrompt());
        }
    }

    void OpenDoor()
    {
        _opened = true;
        collider2D.enabled = false;
        spriteRenderer.sprite = openSprite;
    }

    IEnumerator ShowNeedKeyyPrompt()
    {
        needKeyIndicate.SetActive(true);
        yield return new WaitForSeconds(needKeyShowDuration);
        needKeyIndicate.SetActive(false);
    }
}
