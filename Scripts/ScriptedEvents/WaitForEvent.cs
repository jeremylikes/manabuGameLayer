using ScriptedEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptedEvents
{
    public class WaitForEvent : MonoBehaviour
    {
        [SerializeField] private Transform _objectToStop;
        [SerializeField] private ArrivalEvent _waitingOnArrival;

        private void Start()
        {
            _waitingOnArrival._onObjectHasArrived = TriggerNextAction;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var autoController = collision.gameObject.GetComponent<IAutoController>() ?? null;

            if (autoController != null && collision.gameObject == _objectToStop.gameObject)
            {
                autoController.WaitForEvent();
            }
        }

        private void TriggerNextAction()
        {
            var obj = _objectToStop.GetComponent<IAutoController>();
            obj.DoPostWaitAction();
        }
    }
}

