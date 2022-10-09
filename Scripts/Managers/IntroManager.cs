using Characters;
using Effects;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Managers
{
    public class IntroManager : MonoBehaviour
    {

        [SerializeField] private Transform _manabu;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private SpecialEffects _effects;
        [SerializeField] private Transform _glyph;
        [SerializeField] private SpriteRenderer _honeycomb;
        [SerializeField] private SpriteRenderer _manabuReflection;
        [SerializeField] private Transform _impactParticles;
        [SerializeField] private AudioClip _impactSE;
        [SerializeField] private bool _skipOpening;

        private void OnValidate()
        {
            if (!_skipOpening)
                _manabu.position = _spawnPoint.position;
        }

        private void Start()
        {
            if (!_skipOpening)
                StartCoroutine(InitIntro());
            _manabu.GetComponent<Manabu>().AllowEnchantments = false;
        }

        private IEnumerator InitScene()
        {
            yield return null;
        }

        private IEnumerator InitIntro()
        {
            ControlsManager._instance.SetLockedControls();
            float timeToTouchdown = 7f;
            var anim = _manabu.GetComponent<CharacterController>()._anim;
            var sr = _manabu.GetComponent<SpriteRenderer>();
            var noAlpha = sr.color;
            noAlpha.a = 0;
            sr.color = noAlpha;
            anim.SetBool("floating", true);
            StartCoroutine(_effects.FadeSprite(sr, SpecialEffects.FadeOptions.fadeIn, 4f));
            Camera.main.transform.parent = null;
            while (timeToTouchdown > 0f)
            {
                _manabu.position = Vector3.MoveTowards(_manabu.position, Vector3.right, Time.deltaTime * 0.2f);
                Camera.main.transform.position = new Vector3(_manabu.position.x, _manabu.position.y, Camera.main.transform.position.z);
                _manabu.Rotate(Vector3.back * (Time.deltaTime * 48f));
                timeToTouchdown -= Time.deltaTime;
                yield return null;
            }
            _manabu.rotation = Quaternion.identity;
            anim.SetBool("floating", false);
            GameManager._instance._characterController.SetCurrentDirection(CharacterController._directions.Up);
            GameManager._instance._characterController._currentState = CharacterController._states.Idle;
            Camera.main.transform.parent = _manabu;
            _impactParticles.transform.parent = null;
            _impactParticles.gameObject.SetActive(true);
            AudioManager._instance.PlaySoundEffect(_impactSE);
            if (!_manabuReflection.gameObject.activeInHierarchy)
                _manabuReflection.gameObject.SetActive(true);
            StartCoroutine(InitFadeIn(_honeycomb, 0.3f));
            StartCoroutine(InitFadeIn(_manabuReflection, 0.5f));
            Vector3 offset = new Vector3(0.5f, 1f, 0f);
            _glyph.transform.position = _manabu.position + offset;
            yield return new WaitForSeconds(2f);
            _glyph.gameObject.SetActive(true);
            ControlsManager._instance.SetActiveControls();
        }

        private IEnumerator InitFadeIn(SpriteRenderer sr, float targetAlpha)
        {
            if (sr.color.a != 0.01f)
            {
                var tempColor = sr.color;
                tempColor.a = 0.01f;
                sr.color = tempColor;
            }

            while (sr.color.a < targetAlpha)
            {
                var tempColor = sr.color;
                var step = Time.deltaTime * 0.15f;
                tempColor.a += step;
                sr.color = tempColor;
                yield return null;
            }
        }

    }

}

