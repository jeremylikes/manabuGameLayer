using Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;

namespace Geography
{
    public class BattleArena : MonoBehaviour
    {
        [SerializeField] private int _noOfTargets;
        [SerializeField] private int _deaths;
        [SerializeField] private MapManager _mapManager;
        private void Start()
        {
            ToggleArenaCollision(false);
        }

        private void ToggleArenaCollision(bool enabled)
        {
            var bc = GetComponent<EdgeCollider2D>();
            bc.enabled = enabled;
            if (enabled)
                _mapManager.UpdateCurrentMap(transform);
            else
                _mapManager.UpdateCurrentMap();
        }
        // Start is called before the first frame update
        public void InitBattleArena(List<Character> targetCharacters)
        {
            ToggleArenaCollision(true);
            //Camera.main.GetComponent<CameraMovement>().ResetMapBounds(transform);
            foreach (var c in targetCharacters)
            {
                c.OnCharacterDeath += UpdateBattleArena;
                _noOfTargets += 1;
            }
        }

        private void UpdateBattleArena()
        {
            _deaths += 1;
            if (_deaths >= _noOfTargets)
            {
                _noOfTargets = 0;
                _deaths = 0;
                //Camera.main.GetComponent<CameraMovement>().ResetMapBounds(_originalMapBounds);
                ToggleArenaCollision(false);
            }
        }
    }

}
