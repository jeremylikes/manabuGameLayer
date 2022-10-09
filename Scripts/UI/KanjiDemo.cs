using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class KanjiDemo : MonoBehaviour
    {
        private Coroutine _currentKanjiDemo;
        [SerializeField] private Transform _kanjiCanvas;
        public bool _demoActive = false;

        public void StartKanjiDemo(string kanji)
        {
            _demoActive = true;
            ControlsManager._instance.SetKanjiDemoControls();
            _kanjiCanvas.gameObject.SetActive(true);
            if (_currentKanjiDemo != null)
                StopKanjiDemo();
            _currentKanjiDemo = StartCoroutine(RenderStrokesToKanjiCanvas(kanji));
        }

        public void StopKanjiDemo()
        {
            _demoActive = false;
            ControlsManager._instance.SetMenuControls();
            _kanjiCanvas.gameObject.SetActive(false);
            StopCoroutine(_currentKanjiDemo);
            _currentKanjiDemo = null;
        }

        private IEnumerator RenderStrokesToKanjiCanvas(string targetKanji)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>($@"Sprites/Radiants/anim/{targetKanji}");
            int index = 0;
            while (index < sprites.Length)
            {
                _kanjiCanvas.GetComponent<Image>().sprite = sprites[index];
                index++;
                float delayTimer = 0.5f;
                if (index == sprites.Length)
                {
                    index = 0;
                    while (delayTimer > 0f)
                    {
                        delayTimer -= Time.unscaledDeltaTime;
                        yield return null;
                    }
                    //yield return new WaitForSeconds(0.5f);
                }

                //yield return new WaitForSeconds(0.05f);
                float timeBetweenStrokes = 0.05f;
                while (timeBetweenStrokes > 0f)
                {
                    timeBetweenStrokes -= Time.unscaledDeltaTime;
                    yield return null;
                }
            }
        }
    }


}
