using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    [Serializable]
    public abstract class KeyPoint : MonoBehaviour
    {
        [SerializeField] protected MapManager _mapManager;

        protected void Awake()
        {
            _mapManager = FindObjectOfType<MapManager>();
        }
        public abstract void ResolveState();
    }
}

