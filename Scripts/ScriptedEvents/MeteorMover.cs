using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptedEvents
{
    public class MeteorMover : MonoBehaviour
    {
        private Vector3 _moveVect;
        private Transform _manabuPos;
        private void Start()
        {
            _moveVect = new Vector3(-1f, -1f, 0f);
            _manabuPos = GameManager._instance._mainCharacter.transform;
        }
        private void Update()
        {
            // move this thing at 45 degree angle
            //transform.Translate(_moveVect * Time.deltaTime * 0.33f);
            transform.position = Vector2.MoveTowards(transform.position, _manabuPos.position, Time.deltaTime * 0.33f);
        }
    }

}
