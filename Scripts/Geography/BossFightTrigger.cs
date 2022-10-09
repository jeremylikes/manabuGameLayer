using Characters;
using Controllers;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geography
{
    public class BossFightTrigger : KeyPoint
    {
        [SerializeField] private Teleporter _doorToSeal;
        [SerializeField] private GameObject _bossToInit;
        [SerializeField] bool _wasTriggered = false;
        [SerializeField] AudioClip _bossMusic;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" && !_wasTriggered)
            {
                InitBattle();
            }
        }

        private void InitBattle()
        {
            _wasTriggered = true;
            _bossToInit.SetActive(true);
            _bossToInit.GetComponent<BossAI>()._onSurgeDeath += EndBattle;
            _doorToSeal.gameObject.SetActive(false);
            if (_bossMusic != null)
                AudioManager._instance.PlaySong(_bossMusic);
        }

        public void EndBattle()
        {
            _doorToSeal.gameObject.SetActive(true);
            //ResolveState();
            if (_mapManager._sceneKeypoints.Contains(this))
            {
                int kpIndex = _mapManager.GetKeyPointIndex(this);
                GameStateManager._instance.AddToResolvedKeyPoints(kpIndex);
            }
        }

        public override void ResolveState()
        {
            if (_mapManager._sceneKeypoints.Contains(this))
            {
                int kpIndex = _mapManager.GetKeyPointIndex(this);
                GameStateManager._instance.AddToResolvedKeyPoints(kpIndex);
            }
            Destroy(_bossToInit);
            Destroy(gameObject);
        }
    }


}
