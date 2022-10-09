using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;

public class RadiantNotification : MonoBehaviour {

    bool _grow;
    private float _growthFactor = 1.4f;
    Vector3 _originalSize;
    Vector3 _maxSize;
    float _pulseSpeed = 0.002f;

    private void Awake()
    {
        AudioManager._instance.PlaySoundEffect(
            Resources.Load<AudioClip>(@"Audio/SE/se_pro_tip"));
    }

    void Start ()
	{
        _grow = true;
	    _originalSize = transform.localScale;
        _maxSize = new Vector3(_originalSize.x * _growthFactor, _originalSize.y * _growthFactor, _originalSize.z);

	}
	
	// Update is called once per frame
	void Update () {

        if (Vector3.Distance(transform.localScale, _maxSize) >= 0f && _grow)
        {
            transform.localScale += new Vector3(_pulseSpeed, _pulseSpeed, 0f);
            if (transform.localScale == _maxSize)
                _grow = false;
        }
        else
        {
            transform.localScale -= new Vector3(_pulseSpeed, _pulseSpeed, 0f);
            if (transform.localScale == _originalSize)
                _grow = true;
        }

    }

}
