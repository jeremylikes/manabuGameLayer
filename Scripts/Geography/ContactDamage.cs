using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geography
{
    public class ContactDamage : MonoBehaviour
    {
        [SerializeField] private int _damageAmount = 4;
        [SerializeField] private float _damageRadius = 0.5f;

        //private void Update()
        //{
        //    var targets = Physics2D.OverlapCircleAll(transform.position, _damageRadius);
        //    foreach (var target in targets)
        //    {
        //        var manabu = target.gameObject.GetComponent<Manabu>() ?? null;
        //        if (manabu == null)
        //            return;
        //        if (!manabu.IsInvincible())
        //            manabu.TakeDamage(transform, _damageAmount);
        //    }
        //}
        //private void OnCollisionEnter2D(Collision2D collision)
        //{

        //    var manabu = collision.gameObject.GetComponent<Manabu>() ?? null;
        //    if (manabu != null)
        //        manabu.TakeDamage(transform, _damageAmount);
        //}

        private void OnCollisionStay2D(Collision2D collision)
        {
            var manabu = collision.gameObject.GetComponent<Manabu>() ?? null;
            if (manabu != null && !manabu.IsInvincible())
                manabu.TakeDamage(transform, _damageAmount);
        }
    }
}

