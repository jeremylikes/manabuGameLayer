using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class Rotator : MonoBehaviour
    {
        private enum RotationDirections {
            Clockwise,
            Counterclockwise
        }
        [SerializeField] private float _speed = 4f;
        [SerializeField] private RotationDirections _rotationDirection;
        // Update is called once per frame
        void Update()
        {
            if (_rotationDirection == RotationDirections.Clockwise)
                transform.Rotate(Vector3.back * (_speed * Time.deltaTime));
            else
                transform.Rotate(Vector3.forward * (_speed * Time.deltaTime));
        }
    }

}
