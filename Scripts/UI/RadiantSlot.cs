using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RadiantSlot : MonoBehaviour
    {
        [SerializeField] private string _romajiString;
        [SerializeField] private string _kanji;
        [SerializeField] private bool _hasKanji;

        //public TextMeshProUGUI _slotText;
        [SerializeField] ItemSubMenu _subMenu;
        [SerializeField] private Color _originalColor;
        [SerializeField] private KanjiDemo _kanjiDemo;

        private void OnValidate()
        {
            _originalColor = GetComponent<Image>().color;
        }

        public void InitSubMenu()
        {
            if (_romajiString != "")
            {
                _subMenu._useButton.onClick.RemoveAllListeners();
                //_subMenu._useButton.onClick.AddListener(UseItemContents);
                _subMenu.GetComponent<Menu>().DisplayMenu(_subMenu.GetComponent<Menu>());
                _subMenu._dropButton.onClick.RemoveAllListeners();
                //_subMenu._dropButton.onClick.AddListener(DropItemContents);
            }
        }

        public void SetSlotKanji(string romajiString)
        {
            _romajiString = romajiString;
            var img = GetComponent<Image>();
            if (romajiString != "")
            {
                _kanji = KanjiMap._kanjiMap[romajiString];
                GetComponent<Image>().sprite = Resources.Load<Sprite>($@"Sprites/Radiants/anim/{_kanji}/final");
                //GetComponent<Image>().sprite = Resources.Load<Sprite>($@"Sprites/Radiants/{transposed}");
                _hasKanji = true;
                img.color = _originalColor;
            }
            else
            {
                img.sprite = null;
                var clearColor = _originalColor;
                clearColor.a = 0;
                img.color = clearColor;
                _hasKanji = false;
            }
        }

        public void ToggleKanjiDemo()
        {
            if (_kanji == "")
                return;
            if (!_kanjiDemo._demoActive)
                StartKanjiDemo();
            else
                StopKanjiDemo();
        }

        private void StartKanjiDemo()
        {
            _kanjiDemo.StartKanjiDemo(_kanji);
        }


        private void StopKanjiDemo()
        {
            _kanjiDemo.StopKanjiDemo();
        }
    }
}

