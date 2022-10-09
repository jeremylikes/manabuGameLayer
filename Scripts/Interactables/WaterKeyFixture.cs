using Collectables;
using Managers;
using Spells;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Interactables
{
    public class WaterKeyFixture: MonoBehaviour, ISpellAffectedObject, IInteractable
    {
        [SerializeField] bool _isFrozen = false;
        [SerializeField] AudioClip _freezeSE;
        [SerializeField] AudioClip _takeKeySE;

        public SpellNames GetSpellAffectedBy()
        {
            return SpellNames.Ice;
        }

        public void Interact()
        {
            if (_isFrozen)
            {
                var thing = CollectableManager.GetCollectableByName(CollectableNames.WaterKey);
                var manabu = GameManager._instance._mainCharacter;
                manabu._itemInventory.AddToItemInventory((Item)thing);
                GetComponent<Animator>().SetTrigger("empty");
                GetComponent<UnityEngine.Rendering.Universal.Light2D>().enabled = false;
                _isFrozen = false;
                PlayAudioClip(_takeKeySE);
            }
            else
            {
                var langCode = GameStateManager._instance.GetCurrentLanguageCode();
                var filePath = $"{FileManagement.MessagesUIDirectory}/System/waterKey_hint";
                SystemMessageManager._instance.TriggerSystemMessageFromFilePath(filePath);
                return;
            }
        }

        private void PlayAudioClip(AudioClip clip)
        {
            var audio = GetComponent<AudioSource>();
            audio.clip = clip;
            audio.Play();
        }

        public void ReactToSpell()
        {
            GetComponent<Animator>().SetTrigger("frozen");
            _isFrozen = true;
            PlayAudioClip(_freezeSE);
        }

    }
}

