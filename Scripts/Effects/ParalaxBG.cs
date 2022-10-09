using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxBG : MonoBehaviour
{
    private Transform _cameraTransform;
    private Vector3 _lastCameraPos;
    [SerializeField]
    private Vector2 _paralaxEffectMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _lastCameraPos = _cameraTransform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 delta = _cameraTransform.position - _lastCameraPos;
        transform.position += new Vector3(delta.x * _paralaxEffectMultiplier.x, delta.y * _paralaxEffectMultiplier.y, 0f);
        _lastCameraPos = _cameraTransform.position;
    }
}
