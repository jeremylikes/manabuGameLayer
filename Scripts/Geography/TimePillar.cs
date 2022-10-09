using Characters;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Geography
{
    public class TimePillar : MonoBehaviour
    {
        [SerializeField] private Transform _orbLight;
        [SerializeField] private Transform _connectBeam;
        [SerializeField] private float _countdownTimer = 10f;
        [SerializeField] private AudioClip _warningTone;
        [SerializeField] private AudioClip _releaseLightTone;
        [SerializeField] private TimePillar _upstreamPillar;
        [SerializeField] private TimePillar _downstreamPillar;
        [SerializeField] private bool _isRootNode;
        [SerializeField] private bool _isObstructed = false;
        [SerializeField] private RoomManager _roomManager;
        private Coroutine _countdownCoroutine = null;
        private Debuff _timeDebuff = null;
        private bool _initThisObject = true;

        private void OnValidate()
        {
            if (_downstreamPillar != null)
                _downstreamPillar.enabled = false;
            if (_upstreamPillar == null)
                _isRootNode = true;
        }

        private void Awake()
        {
            if (_isRootNode)
            {
                _initThisObject = false;
                _roomManager._onManabuEnter += () => _initThisObject = true;
                //_roomManager._onManabuLeave += () => _initThisObject = false; ResetPillar();
                _roomManager._onManabuLeave += DisablePillar;
            }

        }

        private void DisablePillar()
        {
            _initThisObject = false;
            ResetPillar();
            
        }

        private void Update()
        {
            if (_initThisObject)
            {
                MonitorObstructions();
                if (!_isObstructed && _countdownCoroutine == null)
                    _countdownCoroutine = StartCoroutine(InitCountdown());
                if (_isObstructed)
                {
                    ResetPillar();
                }
            }

        }

        private void ResetPillar()
        {
            //Take care of this pillar
            if (_timeDebuff != null)
                RemoveTimeEffect();
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = null;
            }

            DisableLights();
            // Take care of downstream nodes
            if (_downstreamPillar != null)
            {
                _downstreamPillar.ResetPillar();
                _downstreamPillar.enabled = false;
            }

            
            
        }

        private void DisableLights()
        {
            _orbLight.gameObject.SetActive(false);
            _connectBeam.gameObject.SetActive(false);
        }

        private IEnumerator InitCountdown()
        {

            var timer = _countdownTimer;
            var warningDelta = 1f; // Flash light every one second
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                if (timer <= 5f)
                {
                    warningDelta += Time.deltaTime;
                    if (warningDelta >= 1f)
                    {
                        warningDelta = 0f;
                        ToggleOrbWarning();
                    }
                }
                yield return null;
            }
            if (_downstreamPillar != null)
                _downstreamPillar.enabled = true;
            _orbLight.gameObject.SetActive(true);
            if (_connectBeam != null && _downstreamPillar != null)
                _connectBeam.gameObject.SetActive(true);
            ExecuteTimeEffect();
        }

        private void ToggleOrbWarning()
        {
            _orbLight.gameObject.SetActive(!_orbLight.gameObject.activeInHierarchy);
        }

        private void ExecuteTimeEffect()
        {
            var currSpeed = GameManager._instance._mainCharacter.GetCharacterStat(CharacterStats.Speed)._current;
            _timeDebuff = new Debuff { _impactedStat = CharacterStats.Speed, _debuffAmount = currSpeed / 2f };
            GameManager._instance._mainCharacter.AddDebuffToStack(_timeDebuff);
        }

        private void RemoveTimeEffect()
        {
            GameManager._instance._mainCharacter.RemoveDebuffFromStack(_timeDebuff);
            _timeDebuff = null;
        }

        private void MonitorObstructions()
        {
            if (_downstreamPillar == null)
                return;
            Vector3 origin = transform.position;
            Vector3 target = _downstreamPillar.transform.position;
            Vector3 direction = target - origin;
            float range = direction.magnitude;
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, range);
            foreach (var hit in hits)
            {
                // let's see if the beam intersects an obstruction
                if (hit.collider.tag == "obstruction")
                {
                    if (!_isObstructed)
                        _isObstructed = true;
                    return;
                }

            }
            if (_isObstructed) _isObstructed = false;
        }

    }
}

