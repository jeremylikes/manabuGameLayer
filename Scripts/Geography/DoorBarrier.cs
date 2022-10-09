using Calculators;
using Characters;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DoorBarrier : MonoBehaviour
{
    [SerializeField] private Transform _effect;
    [SerializeField] private AudioClip _collideSE;

    private void OnValidate()
    {
        _effect = transform.Find("Effect");
    }

    private void Update()
    {
        ExecuteBarrierEffect();
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Manabu>())
        {
            AudioManager._instance.PlaySoundEffect(_collideSE);
            collision.GetComponent<Manabu>().KnockMeBack(transform);
        }
    }


    private void ExecuteBarrierEffect()
    {
        _effect.Rotate(0f, 0f, 0.5f, Space.Self);
        _effect.Rotate(0f, 0f, 0.5f, Space.World);
    }

}
