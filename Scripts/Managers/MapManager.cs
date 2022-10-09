using Characters;
using Controllers;
using Geograpny;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Managers
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private Transform _currentMap;
        [SerializeField] private List<Transform> _sceneMaps;
        [SerializeField] private CameraMovement _cameraMovement;
        [SerializeField] private AudioClip _mainBGM;
        [SerializeField] public List<KeyPoint> _sceneKeypoints;
        [SerializeField] private bool _snapToSpawnPoint = false;
        [SerializeField] private Transform _spawnPoint;
        
        public AudioClip MainBGM => _mainBGM;
        public int GetKeyPointIndex(KeyPoint kp)
        {
            return _sceneKeypoints.IndexOf(kp);
        }

        private void OnValidate()
        {
            if (_snapToSpawnPoint && _spawnPoint != null)
            {
                FindObjectOfType<Manabu>().transform.position = _spawnPoint.position;
            }
        }

        //public void SetCurrentMap(Transform newMap)
        //{
        //    _currentMap = newMap;
        //    _cameraMovement.SetCameraBounds(_currentMap.GetComponent<EdgeCollider2D>().bounds);

        //}
        public void ResolveKeyPoint(int keyPointIndex)
        {
            if (_sceneKeypoints.Count <= 0)
                return;
            _sceneKeypoints[keyPointIndex].ResolveState();
        }
        private void Start()
        {
            AudioManager._instance.PlaySong(_mainBGM);
            UpdateCurrentMap();
        }

        public Bounds GetCurrentMapBounds()
        {
            return _currentMap.GetComponent<BoundsManager>().GetMapBounds();
        }

        public void UpdateCurrentMap(Transform battleArenaOverrideMap = null)
        {
            var manabuPos = GameManager._instance._mainCharacter.transform.position;
            if (battleArenaOverrideMap != null)
            {
                _currentMap = battleArenaOverrideMap;
                //_cameraMovement.SetCameraBounds(_currentMap.GetComponent<EdgeCollider2D>().bounds);
                _cameraMovement.SetCameraBounds(GetCurrentMapBounds());
                return;
            }
            foreach (var map in _sceneMaps)
            {
                if (map.GetComponent<EdgeCollider2D>().bounds.Contains(manabuPos))
                {
                    _currentMap = map;
                    break;
                }
            }
            _cameraMovement.SetCameraBounds(_currentMap.GetComponent<EdgeCollider2D>().bounds);
        }
    }

}
