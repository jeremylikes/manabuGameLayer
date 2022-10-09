using Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;


namespace Interactables
{
    public class GemRoot : MonoBehaviour, IAttackInteractable
    {
        [SerializeField] private AudioClip _activeSound;
        [SerializeField] private AudioClip _inactiveSound;
        [SerializeField] Sprite _activeSprite;
        [SerializeField] Sprite _inactiveSprite;
        [SerializeField] bool _active;
        [SerializeField] private Color _activeColor = Color.blue;
        [SerializeField] private Color _inactiveColor = Color.red;
        [SerializeField] private GemRootNetwork _gemRootNetwork;
        [SerializeField] private float _timeout;
        private Coroutine _countdown;

        public bool Active => _active;

        private void OnValidate()
        {
            ToggleVisuals();
        }

        public void Interact()
        {
            ToggleGemRoot();
        }

        private void ToggleGemRoot()
        {
            if (_active && _countdown != null)
                StopCoroutine(_countdown);
            _active = !_active;
            ToggleVisuals();
            AudioManager._instance.PlaySoundEffect(_active ? _activeSound : _inactiveSound);
            if (_active)
            {
                _countdown = StartCoroutine(StartCountdownTimer());
            }
        }

        private void ToggleGemRoot(bool value)
        {

            _active = value;
            ToggleVisuals();
            AudioManager._instance.PlaySoundEffect(_active ? _activeSound : _inactiveSound);
            if (_active)
            {
                _countdown = StartCoroutine(StartCountdownTimer());
            }
        }

        private void ToggleVisuals()
        {
            GetComponent<SpriteRenderer>().sprite = _active ? _activeSprite : _inactiveSprite;
            UnityEngine.Rendering.Universal.Light2D[] lights = GetComponentsInChildren<UnityEngine.Rendering.Universal.Light2D>();
            foreach (var light in lights)
                light.color = _active ? _activeColor : _inactiveColor;
        }

        public IEnumerator StartCountdownTimer()
        {
            yield return new WaitForSeconds(_timeout == 0f ? 5f: _timeout);
            if (_gemRootNetwork != null && !_gemRootNetwork._conditionIsMet)
                ToggleGemRoot(false);
            
        }
    }
}

