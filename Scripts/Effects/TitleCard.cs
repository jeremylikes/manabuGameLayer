using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Effects
{
    public class TitleCard : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] string[] _textLines;

        private void Start()
        {
            StartCoroutine(ProcessTextFade());
        }

        private IEnumerator ProcessTextFade()
        {
            int currLineIndex = 0;
            yield return new WaitForSeconds(7f);
            _text.gameObject.SetActive(true);
            while (currLineIndex <= _textLines.Length - 1)
            {
                _text.text = _textLines[currLineIndex];
                _text.canvasRenderer.SetAlpha(0f);
                _text.CrossFadeAlpha(1f, 5f, false);
                yield return new WaitForSeconds(5f);
                _text.CrossFadeAlpha(0f, 5f, false);
                yield return new WaitForSeconds(5f);
                currLineIndex++;
            }

        }
    }
}

