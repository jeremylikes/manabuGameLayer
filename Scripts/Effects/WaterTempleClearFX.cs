using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;

namespace Effects
{
    public class WaterTempleClearFX : MonoBehaviour
    {
        [SerializeField] private Transform _waterheartValleyMap;
        [SerializeField] private Transform _waterheartValleySpawn;
        [SerializeField] private AudioClip _waterHeartValleyBGM;

        void Start()
        {
            StartCoroutine(InitTransitionToWaterHeartValley());
        }

        private IEnumerator InitTransitionToWaterHeartValley()
        {
            AudioManager._instance.PlaySong(_waterHeartValleyBGM, true);
            ControlsManager._instance.SetLockedControls();
            var sfx = GameManager._instance.gameObject.GetComponent<SpecialEffects>();
            float fadeTime = 4f;
            StartCoroutine(sfx.FadeInCanvasOverlay(fadeTime));
            yield return new WaitForSeconds(fadeTime);
            MapManager mm = FindObjectOfType<MapManager>();
            GameManager._instance._mainCharacter.transform.position = _waterheartValleySpawn.position;
            mm.UpdateCurrentMap();
            StartCoroutine(sfx.FadeOutCanvasOverlay(fadeTime));
            yield return new WaitForSeconds(fadeTime);
            ControlsManager._instance.SetActiveControls();
        }

    }
}

