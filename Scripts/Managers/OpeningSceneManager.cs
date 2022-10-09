using Characters;
using Geography;
using JetBrains.Annotations;
using ScriptedEvents;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class OpeningSceneManager : MonoBehaviour
    {
        [SerializeField] DialogueManager _dialogueManager;
        [SerializeField] Transform _blackBackdrop;
        [SerializeField] ArrivalEvent[] _triggersToReset;
        [SerializeField] Teleporter _finalTeleporter;
        [SerializeField] Transform _schelle;
        [SerializeField] GameObject _debrisFieldMap;
        private void Start()
        {
            if (_debrisFieldMap != null)
                _debrisFieldMap.SetActive(false);
            _schelle.gameObject.SetActive(true);
            var path = FileManagement.MessagesDialogueDirectory;
            DialogueManager._instance.TriggerConversation($"{path}/Schelle/MeetingSchelle");
            _finalTeleporter._onTeleporterReached = ResetTriggers;
        }

        public IEnumerator KickoffTransitionToTitle()
        {
            _blackBackdrop.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            var sr = GameManager._instance._mainCharacter.GetComponent<SpriteRenderer>();
            float fadeTimer = 4f;
            var tempColor = sr.color;
            while (sr.color.a >= 0.001f)
            {
                sr.color = new Color(tempColor.r, tempColor.g, tempColor.b, tempColor.a -= Time.deltaTime * 0.25f);
                fadeTimer -= Time.deltaTime;
                yield return null;
            }
            StartCoroutine(AudioManager._instance.FadeOutBGM(4f));
            yield return new WaitForSeconds(4f);
            SceneManager.LoadScene("Title");
        }

        public void ResetTriggers()
        {
            foreach(var t in _triggersToReset)
            {
                t.ResetArrivalTrigger();
            }
            //_teleporterReached = null;
        }
    }

}
