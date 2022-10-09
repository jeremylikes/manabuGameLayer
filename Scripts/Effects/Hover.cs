using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Effects
{
    public class Hover : MonoBehaviour
    {
        [SerializeField] private float _yDiff = 1f;
        [SerializeField] private float _speed = 1f;
        private Vector3 _startingPos;
        [SerializeField] private Vector3 _target;
        private bool _moveUp = true;
        [SerializeField] private bool _reverse = false;
        private void Start()
        {
            _startingPos = transform.position;
            _target = _startingPos;
            _yDiff = _reverse ? -_yDiff : _yDiff;
            _target += new Vector3(0f, _yDiff, 0f);
        }
        private void Update()
        {
            if (!_reverse)
            {
                if (_moveUp)
                {
                    if (transform.position.y < _target.y)
                        transform.Translate(Vector3.up * Time.deltaTime * _speed);
                    else
                        _moveUp = false;
                }

                else
                {
                    if (transform.position.y > _startingPos.y)
                        transform.Translate(Vector3.down * Time.deltaTime * _speed);
                    else
                        _moveUp = true;
                }
            }

            else
            {
                if (_moveUp)
                {
                    if (transform.position.y > _target.y)
                        transform.Translate(Vector3.down * Time.deltaTime * _speed);
                    else
                        _moveUp = false;
                }
                else
                {
                    if (transform.position.y < _startingPos.y)
                        transform.Translate(Vector3.up * Time.deltaTime * _speed);
                    else
                        _moveUp = true;
                }
            }


        }
    }
}

