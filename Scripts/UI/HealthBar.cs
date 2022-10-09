using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        private Transform _bar;
        private Color _normalColor = Color.red;
        private Color _criticalColor = Color.white;
        private Color _currentColor = Color.red;
        private bool _isCritical = false;
        private Character _host;

        void Start()
        {
            _bar = transform.Find("HPBar");
            SetColor(_normalColor);
            _host = GetComponentInParent<Character>();
            UpdateHealth();
            _host.OnStatChanged += UpdateHealth;
        }

        public void SetSize(float sizeNormalized)
        {
            _bar.localScale = new Vector3(sizeNormalized, 1f, 1f);
        }

        public void UpdateHealth()
        {
            var hpStat = _host.GetCharacterStat(CharacterStats.HP);
            float normalizedSize = (float)hpStat._current / (float)hpStat._max;
            SetSize(normalizedSize);
        }

        public void SetColor(Color c)
        {
            _bar.GetComponentInChildren<SpriteRenderer>().color = c;
            _currentColor = c;
        }
        void Update()
        {
            if (_bar.localScale.x <= .5f && !_isCritical)
            {
                _isCritical = true;
                InvokeRepeating("FlashCritical", 0f, 0.5f);
            }

            if (_bar.localScale.x > .5f & _isCritical)
            {
                _isCritical = false;
                CancelInvoke();

            }

        }

        void FlashCritical()
        {
            SetColor(_currentColor == _normalColor ? _criticalColor : _normalColor);
        }
    }
}
