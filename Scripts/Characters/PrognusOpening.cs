using Controllers;
using Interactables;
using Managers;
using Messages;
using Spells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace Characters
{
    public class PrognusOpening : MonoBehaviour, IInteractable
    {
        private Material _material;
        private bool _isDissolving;
        private float _fade = 0f;
        [SerializeField] AudioClip[] _audio;
        private int _currentAudioIndex = 0;
        private Coroutine _moveLips;
        [SerializeField] private LearningPoint _startingRadiant;
        [SerializeField] private Transform _portal;
        [SerializeField] private Transform _reflection;
        [SerializeField] private int _tutorialIndex;
        [SerializeField] private Transform _tutorialKanjiPanel;
        [SerializeField] private Transform _tutorialEnemy;
        [SerializeField] private Sprite[] _goodSampleImages;
        [SerializeField] private Sprite[] _badSampleImages;
        private ControlsManager.ControlSchema _originalControls;
        private Action _onDemoStepFinished;
        
        void Start()
        {
            _material = GetComponent<SpriteRenderer>().material;
            _material.SetFloat("_fade", _fade);
            StartCoroutine(InitOpeningDialogue());
            DialogueManager._instance._onKanjiLearned += ProcessTutorial;
        }

        void Update()
        {
            if (_fade < 1f)
            {
                _fade += Time.deltaTime;
                _material.SetFloat("_fade", _fade);
            }
            if (_fade >= 1f && !_reflection.gameObject.activeInHierarchy)
                _reflection.gameObject.SetActive(true);
        }

        private IEnumerator InitOpeningDialogue()
        {
            yield return new WaitForSeconds(2f);
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            DialogueManager._instance._onDialogueAdvanced += PlayNextAudioLine;
            var pathPrefix = FileManagement.MessagesDialogueDirectory;
            DialogueManager._instance.TriggerConversation($"{pathPrefix}/Prognus/0");
            GameManager._instance._mainCharacter._castEnabled = true;
            Camera.main.GetComponent<EdgeCollider2D>().enabled = true;
        }

        private void InitFinalDialogue()
        {
            var manabu = GameManager._instance._mainCharacter;
            var controller = manabu.GetComponent<Managers.CharacterController>();
            StartCoroutine(ProcessFinalDialogue());
        }

        private IEnumerator ProcessFinalDialogue()
        {
            yield return new WaitForSeconds(1f);
            var controller = GameManager._instance._mainCharacter.GetComponent<Managers.CharacterController>();
            controller.InterruptAttackDelayRoutine();
            DialogueManager._instance._onDialogueEnded += OpenPortal;
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            DialogueManager._instance.TriggerConversation($"{FileManagement.MessagesDialogueDirectory}/Prognus/2");
        }

        private void OpenPortal()
        {
            _portal.gameObject.SetActive(true);
        }


        private void ProcessTutorial()
        {
            var convoblob = Resources.Load($"{FileManagement.MessagesDialogueDirectory}/Prognus/1") as TextAsset;
            var convoLines = new List<string>();
            DialogueManager._instance._onDialogueAdvanced += PlayNextAudioLine;
            //var speaker = "Prognus";
            var convoBlobText = convoblob.text.Split('\n');
            for (int i = 1; i < convoBlobText.Length; i++)
            {
                convoLines.Add(convoBlobText[i]);
            }

            if (_tutorialIndex == 0)
            {
                DialogueManager._instance.TriggerConversation(new[] { convoLines[0], convoLines[1] });
                DialogueManager._instance._onDialogueEnded = PerformStageZeroTutorial;
            }

            if (_tutorialIndex == 1)
            {
                DialogueManager._instance.TriggerConversation(new[] { convoLines[2], convoLines[3] });
                DialogueManager._instance._onDialogueEnded = PerformStageOneTutorial;
            }

            if (_tutorialIndex == 2)
            {
                DialogueManager._instance.TriggerConversation(new[] { convoLines[4] });
                DialogueManager._instance._onDialogueEnded = PerformStageTwoTutorial;
            }

            if (_tutorialIndex == 3)
            {
                DialogueManager._instance.TriggerConversation(new[] { convoLines[5], convoLines[6] });
                DialogueManager._instance._onDialogueEnded = PerformStageFourTutorial;

            }

            if (_tutorialIndex > 3)
            {
                InitFinalDialogue();
            }

        }

        public void AdvanceDemo()
        {
            _tutorialIndex++;
            ProcessTutorial();
        }

        private void PerformStageZeroTutorial()
        {
            SummonEnemy();
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            var platform = GameStateManager._instance.CurrentPlatform.ToString();
            var unity = Resources.Load($"Messages/{langCode}/UI/Help/cast_help_{platform}") as TextAsset;
            var lines1 = unity.text;

            var unity2 = Resources.Load($"Messages/{langCode}/UI/Help/release_help_{platform}") as TextAsset;
            var lines2 = unity2.text;

            var lines = (lines1 + lines2).Split('\n');
            
            SystemMessageManager._instance.TriggerSystemMessage(lines);
        }

        private void PerformStageOneTutorial()
        {
            StartCoroutine(RenderStrokesToKanjiCanvas(_badSampleImages));
        }

        private void PerformStageTwoTutorial()
        {
            StartCoroutine(RenderStrokesToKanjiCanvas(_goodSampleImages, 25));
        }
        //private void PerformStageThreeTutorial()
        //{
        //    StartCoroutine(RenderStrokesToKanjiCanvas(_goodSampleImages, 30, true));
        //}

        private void PerformStageFourTutorial()
        {
            var path = FileManagement.MessagesUIDirectory;
            var lines = Resources.Load($"{path}/Help/cast_help_pc") as TextAsset;
            SystemMessageManager._instance.TriggerSystemMessage(lines.text.Split('\n'));
            SystemMessageManager._instance._onSystemMessageEnded = KickoffDemoStrokes;
            if (!GameManager._instance._mainCharacter.AllowEnchantments)
                GameManager._instance._mainCharacter.AllowEnchantments = true;

        }

        private void KickoffDemoStrokes()
        {
            StartCoroutine(RepeatKanjiStrokes());
        }

        private void SummonEnemy()
        {
            _tutorialEnemy.gameObject.SetActive(true);
        }

        private IEnumerator RenderStrokesToKanjiCanvas(Sprite[] sprites, int damage = 10, bool fast = false)
        {
            _originalControls = ControlsManager._instance.GetCurrentControlSchema();
            ControlsManager._instance.SetLockedControls();
            _tutorialKanjiPanel.gameObject.SetActive(true);
            foreach (var s in sprites)
            {
                _tutorialKanjiPanel.gameObject.GetComponent<Image>().sprite = s;
                yield return new WaitForSeconds(fast ? 0.025f : 0.05f);
            }
            yield return new WaitForSeconds(2f);
            _tutorialKanjiPanel.gameObject.SetActive(false);
            StartCoroutine(CastFire(damage));
        }

        private IEnumerator RepeatKanjiStrokes()
        {
            int reps = 1;
            _tutorialKanjiPanel.gameObject.SetActive(true);
            while (reps > 0)
            {
                reps--;
                foreach (var s in _goodSampleImages)
                {
                    _tutorialKanjiPanel.gameObject.GetComponent<Image>().sprite = s;
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitForSeconds(0.5f);
            }
            _tutorialKanjiPanel.gameObject.SetActive(false);
        }
        public void StopKanjiStrokes()
        {
            StopCoroutine(RepeatKanjiStrokes());
            _tutorialKanjiPanel.gameObject.SetActive(false);

        }

        private IEnumerator CastFire(int damage)
        {
            var spellObj = Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/hiFire"), _tutorialEnemy.position, Quaternion.identity);
            AudioManager._instance.PlaySoundEffect(spellObj.GetComponent<Fireball>()._burstSE);
            //spellObj.GetComponent<Fireball>().ExecuteEffect();
            float animLength = spellObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            _tutorialEnemy.GetComponent<Character>().TakeDamage(transform, damage, false, true);
            yield return new WaitForSeconds(animLength);
            Destroy(spellObj);
            yield return new WaitForSeconds(1f);
            ControlsManager._instance.SetControls(_originalControls);
            AdvanceDemo();
        }

        private void PlayNextAudioLine()
        {
            var audio = GetComponent<AudioSource>();
            if (_currentAudioIndex > _audio.Length -1)
            {
                _currentAudioIndex = 0;
                return;
            }
            if (_audio[_currentAudioIndex] != null)
            {
                audio.clip = _audio[_currentAudioIndex];
                audio.Play();
            }

            if (_moveLips != null)
            {
                StopCoroutine(_moveLips);
                _moveLips = null;
            }
            _moveLips = StartCoroutine(MoveCharacterLips(_audio[_currentAudioIndex].length));
            _currentAudioIndex++;
        }

        private IEnumerator MoveCharacterLips(float audioLength)
        {
            var anim = GetComponent<Animator>();
            anim.SetBool("talking", true);
            yield return new WaitForSeconds(audioLength);
            anim.SetBool("talking", false);

        }

        public void Interact()
        {
            ProcessTutorial();
        }
    }
}
