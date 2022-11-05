using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lightning : MonoBehaviour
{
    [SerializeField]
    private float damagePoint;
    [SerializeField]
    private float disapearAfter;
    [SerializeField]
    private UnityEvent disapear;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private SoundClip thunderSound;

    public void Setup(AbstractEnemy enemy, float percentage)
    {
        transform.position = enemy.transform.position;

        enemy.Damage(damagePoint * percentage);

        thunderSound.Play(audioSource);
        Destroy(gameObject, thunderSound.Clip.length);
        StartCoroutine(Disapear());
    }

    IEnumerator Disapear()
    {
        yield return new WaitForSeconds(disapearAfter);
        disapear.Invoke();
    }
}
