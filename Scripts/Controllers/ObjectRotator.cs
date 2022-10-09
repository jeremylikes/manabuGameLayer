using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controllers
{
    public class ObjectRotator : MonoBehaviour
    {
        public float _xAngle, _yAngle, _zAngle;
        public float _maxScaleSize = 4f;


        void Update()
        {
            transform.Rotate(_xAngle, _yAngle, _zAngle, Space.Self);
            transform.Rotate(_xAngle, _yAngle, _zAngle, Space.World);


        }
    }

}
