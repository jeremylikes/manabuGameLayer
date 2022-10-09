using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geography
{
    public enum TerrainTypes
    {
        NotSet, Normal, Water, Gravel,
        Metal, Snow, Ice, Sand
    }
    public class TerrainManager : MonoBehaviour
    {

        [SerializeField]
        private TerrainTypes _activeTerrain = TerrainTypes.NotSet;

        private void Start()
        {
            if (_activeTerrain == TerrainTypes.NotSet)
                _activeTerrain = TerrainTypes.Normal;
        }
        public TerrainTypes GetActiveTerrain()
        {
            return _activeTerrain;
        }
        public void ChangeTerrain(TerrainTypes targetTerrain)
        {
            _activeTerrain = targetTerrain;
        }


    }
}


