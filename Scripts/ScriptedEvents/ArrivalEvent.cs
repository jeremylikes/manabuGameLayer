using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptedEvents
{

    public class ArrivalEvent : MonoBehaviour
    {
        [SerializeField] private Transform _waitingForObject;
        [SerializeField] private bool _arrived = false;
        public Action _onObjectHasArrived;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform == _waitingForObject && !_arrived)
            {
                _arrived = true;
                _onObjectHasArrived?.Invoke();
            }
        }

        public void ResetArrivalTrigger()
        {
            _arrived = false;
        }

    }

}