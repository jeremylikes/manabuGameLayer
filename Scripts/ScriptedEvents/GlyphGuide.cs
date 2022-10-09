using Characters;
using Effects;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace ScriptedEvents
{

    public class GlyphGuide : MonoBehaviour
    {
        [SerializeField] List<Transform> _wayPoints;
        [SerializeField] int _currentWayPointIndex = 0;
        [SerializeField] private bool _moving = false;
        private bool _wasTriggered = false;

        private void Update()
        {
            if (!_moving)
                LookForManabu();
        }

        private void LookForManabu()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.6f);
            foreach (var c in colliders)
            {
                var manabu = c.GetComponent<Manabu>() ?? null;
                if (manabu != null && !_wasTriggered)
                {
                    _wasTriggered = true;
                    _moving = true;
                    StartCoroutine(MoveToNextWayPoint());
                }
            }

        }

        private IEnumerator MoveToNextWayPoint()
        {
            if (_currentWayPointIndex < _wayPoints.Count)
            {
                var target = _wayPoints[_currentWayPointIndex].position;
                var dist = Vector3.Distance(transform.position, target);
                while (dist > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 1.4f);
                    dist = Vector3.Distance(transform.position, target);
                    yield return null;
                }
                _moving = false;
                _currentWayPointIndex++;
                _wasTriggered = false;
            }
            else
                StartCoroutine(InitDissolveSequence());
            
        }

        private IEnumerator InitDissolveSequence()
        {
            
            var sr = GetComponent<SpriteRenderer>();
            float fadeTimer = 2f;
            var sfx = GetComponent<SpecialEffects>();
            StartCoroutine(sfx.FadeSprite(sr, SpecialEffects.FadeOptions.fadeOut, fadeTimer));
            yield return new WaitForSeconds(fadeTimer);
            Destroy(gameObject);
        }

    }

}
