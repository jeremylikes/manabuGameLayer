using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PulsingLight : MonoBehaviour {

    public float _speed = 2f;
    public float _pulseMin = 0f;
    public float _pulseMax = 3f;
    public float _pulseDuration = 2f;
    //public Light _light;
    public UnityEngine.Rendering.Universal.Light2D[] _lights;
    private float _timer = 0f;
    public bool _rotate = false;
    public float _rotationSpeed = 0f;

    void Update()
    {
        foreach (UnityEngine.Rendering.Universal.Light2D light in _lights)
        {
            _pulseDuration = _pulseDuration < 0f ? 0f : _pulseDuration;
            if (_pulseDuration == 0f)
            {
                light.intensity = Mathf.PingPong(Time.time * _speed, _pulseMax) + _pulseMin;

                if (_rotate)
                {
                    if (_rotationSpeed <= 0f)
                        _rotationSpeed = 0.05f;
                    gameObject.transform.Rotate(0f, 0f, _rotationSpeed);
                }

            }

            else
            {
                _timer += Time.deltaTime;
                if (_timer < _pulseDuration)
                    light.intensity = Mathf.PingPong(Time.time * _speed, _pulseMax) + _pulseMin;

                else
                    light.intensity = -Time.time * _speed;
            }
        }


    }


}
