using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class SpriteFade : MonoBehaviour
    {
        private enum fadeTypes { fadeIn, fadeOut };
        [SerializeField] private float _fadeTime = 2f;
        [SerializeField] fadeTypes _fadeType = fadeTypes.fadeIn;
        private SpriteRenderer _sprite;
        private bool _fadeComplete = false;

        public float FadeTime => _fadeTime;

        private void Start()
        {
            _sprite = gameObject.GetComponent<SpriteRenderer>();
            if (!_sprite.enabled)
                _sprite.enabled = true;
            if (_fadeType == fadeTypes.fadeIn)
            {
                var tempColor = _sprite.color;
                tempColor.a = 0.001f;
                _sprite.color = tempColor;
            }

        }
        // Update is called once per frame
        void Update()
        {
            if (!_fadeComplete)
            {
                if (_fadeType == fadeTypes.fadeIn && _sprite.color.a < 1f)
                {
                    var tempColor = _sprite.color;
                    tempColor.a += Time.deltaTime / _fadeTime;
                    _sprite.color = tempColor;
                    return;
                }
                if (_fadeType == fadeTypes.fadeOut && _sprite.color.a > 0.001f)
                {
                    var tempColor = _sprite.color;
                    tempColor.a -= Time.deltaTime / _fadeTime;
                    _sprite.color = tempColor;
                    return;
                }
                else
                    _fadeComplete = true;
            }


        }
    }
}

