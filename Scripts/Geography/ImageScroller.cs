using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageScroller : MonoBehaviour
{
    [SerializeField] private Vector3 _targetPos;
    private Vector3 _startPos;

    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, _targetPos) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, Time.deltaTime);
        }
        else
            transform.position = _startPos;
    }
}
