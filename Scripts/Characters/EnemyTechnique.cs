using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Characters
{
    public class EnemyTechnique : MonoBehaviour
    {
        [SerializeField] int _damage;
        [SerializeField] float _killTimer;
        [SerializeField] bool _fakeDamage;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var manabu = collision.GetComponent<Manabu>() ?? null;
            if (manabu != null)
            {
                manabu.TakeDamage(transform, _damage, false, _fakeDamage);
            }
        }

        private void Start()
        {
            StartCoroutine(StartDestroyCountdown());
        }

        private IEnumerator StartDestroyCountdown()
        {
            yield return new WaitForSeconds(_killTimer);
            Destroy(gameObject);
        }
    }

}
