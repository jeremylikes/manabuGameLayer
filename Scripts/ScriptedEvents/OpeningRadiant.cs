using Characters;
using Controllers;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ScriptedEvents
{
    public class OpeningRadiant : MonoBehaviour
    {
        [SerializeField] Transform _prognus;
        [SerializeField] bool _wasTriggered;
        [SerializeField] AudioClip _prognusAppearSound;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var manabu = collision.GetComponent<Manabu>() ?? null;
            if (manabu != null && !_wasTriggered)
            {
                _wasTriggered = true;
                StartCoroutine(InitPrognusCutscene());
            }
        }

        private IEnumerator InitPrognusCutscene()
        {
            ControlsManager._instance.SetLockedControls();
            var camera = Camera.main;
            camera.transform.parent = null;
            camera.GetComponent<CameraMovement>().enabled = false;
            float zVal = -1f;
            float scrollTimer = 2f;
            var start = new Vector3(camera.transform.position.x, camera.transform.position.y, zVal);
            var target = new Vector3(_prognus.position.x, _prognus.position.y - 0.5f, zVal);
            var dist = Vector3.Distance(start, target);
            var step = dist / scrollTimer;
            while (dist > 0f)
            {
                dist = Vector3.Distance(camera.transform.position, target);
                camera.transform.position = Vector3.MoveTowards(camera.transform.position, target, Time.deltaTime * step);
                yield return null;
            }
            _prognus.gameObject.SetActive(true);
            if (_prognusAppearSound != null)
                AudioManager._instance.PlaySoundEffect(_prognusAppearSound);
        }
    }
}
