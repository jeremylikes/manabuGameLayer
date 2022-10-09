using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Geography;

namespace ScriptedEvents
{
    public class WreckageWakeupCutscene : MonoBehaviour
    {
        [SerializeField] private Transform _manabuPostWreck;
        [SerializeField] private MapManager _mapManager;
        [SerializeField] private Teleporter _teleToPrologueScene;
        [SerializeField] private OpeningSceneManager _openingSceneManager;

        private void Start()
        {
            ControlsManager._instance.SetLockedControls();
            StartCoroutine(ZoomOutCamera());
            _teleToPrologueScene._onTeleporterReached = LaunchPrologue;
        }

        private void LaunchPrologue()
        {
            _openingSceneManager.enabled = true;
            GetComponent<AudioSource>().enabled = false;
        }

        private IEnumerator ZoomOutCamera()
        {
            float targetCamSize = 1.5f;
            float zoomOutTime = 10f;
            // delay before pullback
            yield return new WaitForSeconds(5f);
            while (Camera.main.orthographicSize < targetCamSize)
            {
                Camera.main.orthographicSize += Time.deltaTime / zoomOutTime;
                yield return null;
            }
            if (Camera.main.orthographicSize != targetCamSize)
                Camera.main.orthographicSize = targetCamSize;

            yield return new WaitForSeconds(6f);
            Destroy(_manabuPostWreck.gameObject);
            GameManager._instance._mainCharacter.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            ControlsManager._instance.SetActiveControls();
            _mapManager.gameObject.SetActive(true);
        }
    }

}