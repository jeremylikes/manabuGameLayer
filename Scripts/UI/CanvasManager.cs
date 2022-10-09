using KanjiUnity;
using System.Collections;
using System.Collections.Generic;
//using System.Net.Http.Headers;
using UnityEngine;
//using UnityEngine.UIElements;

namespace UI
{
    public class CanvasManager : MonoBehaviour
    {
        public GameObject _kanjiPanel;
        private float _duration = 1f;
        public bool _faded;
        private Coroutine _currentCoroutine;
        public GameObject _equipIcon;
        public GameObject _screenOverlay;

        public void ToggleKanjiPanel()
        {
            _kanjiPanel.SetActive(!_kanjiPanel.activeInHierarchy);
            CanvasGroup cg = _kanjiPanel.GetComponent<CanvasGroup>();
            if (_kanjiPanel.activeInHierarchy)
            {
                _kanjiPanel.GetComponent<DrawKanji>().Reset();
                if (_currentCoroutine != null)
                    StopCoroutine(_currentCoroutine);
                _currentCoroutine = StartCoroutine(FadeCanvasGroup(cg, cg.alpha, 1f));
            }
            else
            {
                cg.alpha = 0f;
                if (_currentCoroutine != null)
                {
                    StopCoroutine(_currentCoroutine);
                    _currentCoroutine = null;
                }
                _kanjiPanel.SetActive(false);
            }
            //_currentCoroutine = StartCoroutine(FadeCanvasGroup(cg, cg.alpha, _kanjiPanel.activeInHierarchy ? 0 : 1, 
            //    _kanjiPanel.activeInHierarchy ? true : false));
        }

        public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, bool disableAfterFade = false)
        {
            if (!cg.gameObject.activeInHierarchy)
                cg.gameObject.SetActive(true);
            float counter = 0f;
            while (counter < _duration)
            {
                counter += Time.deltaTime *3f;
                cg.alpha = Mathf.Lerp(start, end, counter / _duration);
                yield return null;
            }
            if (disableAfterFade)
                cg.gameObject.SetActive(false);
        }

        public void ToggleScreenOverlay(bool setting)
        {
            _screenOverlay.SetActive(setting);
        }

    }


}

