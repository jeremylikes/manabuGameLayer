using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    public class IceOrb : MonoBehaviour
    {
        [SerializeField] private Transform _surge;
        public bool _isObstructed = false;
        [SerializeField] private Transform _beam;
        public Action _onBeamObstructed;
        public Action _onBeamUnobstructed;
        private void Update()
        {
            MonitorObstructions();
        }

        private void ToggleBeam(bool setting)
        {
            _beam.gameObject.SetActive(setting);
        }

        private void MonitorObstructions()
        {
            Vector3 origin = transform.position;
            Vector3 target = _surge.position;
            Vector3 direction = target - origin;
            float range = direction.magnitude;
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, range);
            foreach (var hit in hits)
            {
                if (hit.collider.tag == "obstruction")
                {
                    if (!_isObstructed)
                    {
                        _isObstructed = true;
                        _onBeamObstructed?.Invoke();
                        ToggleBeam(false);
                    }
                    return;
                }
            }
            if (_isObstructed)
            {
                _isObstructed = false;
                _onBeamUnobstructed?.Invoke();
                ToggleBeam(true);

            }
        }
    }

}
