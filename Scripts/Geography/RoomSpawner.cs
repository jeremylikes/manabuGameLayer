using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace Geography
{
    public class RoomSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnObject[] _spawnObjects;
        [SerializeField] private GameObject[] _enemies;

        private void Start()
        {
            foreach (var obj in _spawnObjects)
            {
                obj._enemyObj.GetComponent<Character>().OnCharacterDeath += ProcessRespawn;
            }
        }

        private void ResetRoom()
        {

        }

        private void ProcessRespawn()
        {

        }

        [Serializable]
        private class SpawnObject
        {
            public GameObject _enemyObj;
            public GameObject _prefabObj;
        }
    }


}