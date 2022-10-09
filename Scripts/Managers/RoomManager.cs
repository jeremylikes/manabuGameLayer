using Characters;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private int _roomNo;
        [SerializeField] BoxCollider2D _roomBounds;
        [SerializeField] BoxCollider2D _manabuCollider;
        [SerializeField] bool _manabuIsHere = false;

        public Action _onManabuEnter;
        public Action _onManabuLeave;

        private void Start()
        {
            //var xBounds = _collision.bounds.size.x;
            //var yBounds = _collision.bounds.size.y;
            //Debug.Log($"xBounds: {xBounds}\nyBounds: {yBounds}");
            //Debug.Log($"ManabuXBunds: {_manabuCollider.bounds.size.x}\nManabuYBounds: {_manabuCollider.bounds.size.y}");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Manabu>() != null)
            {
                _manabuIsHere = true;
                _onManabuEnter?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<Manabu>() != null)
            {
                _manabuIsHere = false;
                _onManabuLeave?.Invoke();
            }

        }

    }

}
