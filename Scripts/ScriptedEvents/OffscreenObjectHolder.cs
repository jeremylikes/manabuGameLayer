using Managers;
using ScriptedEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptedEvents
{
    public class OffscreenObjectHolder : MonoBehaviour
    {
        [SerializeField] private Transform _releasePosition;
        [SerializeField] private Transform _jailedObject;
        [SerializeField] private ArrivalEvent _waitingOnArrival;

        private void Start()
        {
            _waitingOnArrival._onObjectHasArrived = ReleaseJailedObject;
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            var autoController = collider.GetComponent<IAutoController>() ?? null;
            if (autoController != null)
            {
                HoldInPosition(collider.transform);
            }
        }

        private void HoldInPosition(Transform target)
        {
            target.gameObject.SetActive(false);
            target.transform.position = transform.position;
            _jailedObject = target;
        }

        private void ReleaseJailedObject()
        {
            if (_jailedObject != null)
            {
                _jailedObject.gameObject.SetActive(true);
                _jailedObject.position = _releasePosition.position;
                _jailedObject = null;
            }

        }
    }

}
