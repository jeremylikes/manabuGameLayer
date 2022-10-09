using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Characters;
using Managers;
using System.IO;
using ScriptedEvents;
using Controllers;

namespace Geography
{
    public class Teleporter : MonoBehaviour
    {
        
        public Transform _target;
        public Vector3 _setPositionAfterSceneLoad;
        public bool _loadScene;
        public SceneNames _sceneToLoad;
        public Managers.CharacterController._directions _playerDirectionAfterSpawn;
        [SerializeField] private SceneLoader _sceneLoader;
        public Action _onTeleporterReached;
        float _exitOffset = 0.3f;
        [SerializeField] MapManager _mapManager;

        private void Start()
        {
            GetComponentInParent<MapManager>();
        }
        private void OnValidate()
        {
            _sceneLoader = FindObjectOfType<SceneLoader>();
        }

        void OnTriggerEnter2D(Collider2D c)
        {
            var passenger = c.transform;
            if (passenger.GetComponent<Manabu>() != null || passenger.tag == "useTeleporter")
            {
                Vector3 offsetVector = Vector3.zero;
                switch (_playerDirectionAfterSpawn)
                {
                    case Managers.CharacterController._directions.Up:
                        offsetVector = new Vector3(0f, _exitOffset, 0f);
                        break;
                    case Managers.CharacterController._directions.Down:
                        offsetVector = new Vector3(0f, -_exitOffset, 0f);
                        break;
                    case Managers.CharacterController._directions.Left:
                        offsetVector = new Vector3(-_exitOffset, 0f, 0f);
                        break;
                    case Managers.CharacterController._directions.Right:
                        offsetVector = new Vector3(_exitOffset, 0f, 0f);
                        break;
                }
                var spawnPoint = new Vector3();
                if (_loadScene)
                {
                    spawnPoint = _setPositionAfterSceneLoad + offsetVector;
                    _sceneLoader.LoadLevel(_sceneToLoad, spawnPoint, _playerDirectionAfterSpawn);
                    return;
                }
                var targetPos = new Vector3(_target.position.x, _target.position.y, passenger.transform.position.z);
                spawnPoint = targetPos + offsetVector;
                passenger.transform.position = spawnPoint;
                if (_mapManager != null)
                    _mapManager.UpdateCurrentMap();
                passenger.GetComponent<Managers.CharacterController>().SetCurrentDirection(_playerDirectionAfterSpawn);
                _onTeleporterReached?.Invoke();
            }

        }

    }
}

