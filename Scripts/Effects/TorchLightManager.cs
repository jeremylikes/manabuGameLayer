using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLightManager : MonoBehaviour {

    public float _maxDist = 3f;
    public float _minDist = 1f;
    public float _maxIntensity = 1f;
    public float _minIntensity = 0.5f;
    public Light _torchLight;
    public float _speed = 2f;
    float _randomDistValue;
	// Use this for initialization
	void Start () {
        _randomDistValue = Random.Range(0f, 5f);
	}
	
	// Update is called once per frame
	void Update () {
        _torchLight.range = Mathf.PingPong(Time.time * _speed, _maxDist);
        float noise = Mathf.PerlinNoise(_randomDistValue, Time.time);
        _torchLight.range = Mathf.Lerp(_minDist, _maxDist, noise);
        _torchLight.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, noise) - 0.5f;
    }
}
