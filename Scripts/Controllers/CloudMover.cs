using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class CloudMover : MonoBehaviour
    {
        public Vector2 _targetPos = new Vector2(10f, 10f);
        public float _movementSpeed = 0.2f;
        private Vector2 _startPos = Vector3.zero;
        // Start is called before the first frame update
        void Start()
        {
            _startPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPos, Time.deltaTime * _movementSpeed);
            var dist = Vector3.Distance(transform.position, _targetPos);
            if (dist <= 0f)
                transform.position = _startPos;
        }
    }
}

