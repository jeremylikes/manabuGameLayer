using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Controllers
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        //public float _smoothing;
        [SerializeField] private CameraBounds _cameraBounds;
        [SerializeField] private float _smoothSpeed = 10f;

        public void SetCameraBounds(Bounds currentMapBounds)
        {
            var height = 2 * Camera.main.orthographicSize;
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            var width = height * aspectRatio;
            _cameraBounds._min.x = currentMapBounds.min.x + (0.5f * width);
            _cameraBounds._max.x = currentMapBounds.max.x - (0.5f * width);
            _cameraBounds._min.y = currentMapBounds.min.y + (0.5f * height);
            _cameraBounds._max.y = currentMapBounds.max.y - (0.5f * height);
        }

        private void LateUpdate()
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, _cameraBounds._min.x, _cameraBounds._max.x),
                Mathf.Clamp(transform.position.y, _cameraBounds._min.y, _cameraBounds._max.y),
                transform.position.z
            );
        }
        private void FixedUpdate()
        {
            Vector3 targetPos = new Vector3(_target.position.x, _target.position.y, transform.position.z);
            if (transform.position != targetPos)
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Bottom Line
            Gizmos.DrawLine(new Vector2(_cameraBounds._min.x, _cameraBounds._min.y),
                new Vector2(_cameraBounds._max.x, _cameraBounds._min.y));
            //Left Line
            Gizmos.DrawLine(new Vector2(_cameraBounds._min.x, _cameraBounds._min.y),
                new Vector2(_cameraBounds._min.x, _cameraBounds._max.y));
            //Top Line
            Gizmos.DrawLine(new Vector2(_cameraBounds._min.x, _cameraBounds._max.y),
                new Vector2(_cameraBounds._max.x, _cameraBounds._max.y));
            //Right Line
            Gizmos.DrawLine(new Vector2(_cameraBounds._max.x, _cameraBounds._min.y),
                new Vector2(_cameraBounds._max.x, _cameraBounds._max.y));
        }
    }

    [Serializable]
    public class CameraBounds
    {
        public Vector2 _min;
        public Vector2 _max;
    }
}

