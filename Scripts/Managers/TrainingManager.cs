using GameKanji;
using GameSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class TrainingManager : MonoBehaviour
    {
        [SerializeField] private Transform _practiceCanvas;
        [SerializeField] private Transform _statsCanvas;
        [Space(10)]
        [SerializeField] private List<KanjiTile> _kanjiTiles;
        [SerializeField] int _currentPage = 0;
        [SerializeField] private Transform _writingSurface;
        [SerializeField] private string _currentKanji;

        private const int MAX_TILES_PER_PAGE = 5;


        private void Start()
        {
            SaveSystem.SaveToLearnedKanji("火");
            SaveSystem.SaveToLearnedKanji("氷");
            RenderVisibleKanjiTiles();
        }

        private void RenderVisibleKanjiTiles()
        {
            int startKanjiIndex = _currentPage * MAX_TILES_PER_PAGE;
            int endKanjiIndex = startKanjiIndex + MAX_TILES_PER_PAGE - 1;
           
            int currentTileIndex = 0;
            var kanjiList = SaveSystem.GetLearnedKanjiList();

            for (; currentTileIndex < MAX_TILES_PER_PAGE; currentTileIndex++)
            {
                if (kanjiList.Count > currentTileIndex)
                {
                    var kanji = kanjiList[currentTileIndex];
                    _kanjiTiles[currentTileIndex].SetTargetKanji(kanji);
                }
                else
                    _kanjiTiles[currentTileIndex].gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            }
        }

        public void TransitionToKanjiPractice(KanjiTile targetTile)
        {
            if (targetTile.GetTargetKanji() != null)
            {
                _currentKanji = targetTile.GetTargetKanji();
                _statsCanvas.gameObject.SetActive(false);
                _practiceCanvas.gameObject.SetActive(true);
                StartCoroutine(RenderStrokesToWritingSurface());
            }
        }

        public void TransitionToStats()
        {
            StopAllCoroutines();
            _writingSurface.GetComponent<Image>().sprite = null;
            _writingSurface.gameObject.SetActive(false);
            _practiceCanvas.gameObject.SetActive(false);
            _statsCanvas.gameObject.SetActive(true);
        }

        public void ReturnToTitleScreen()
        {
            SceneManager.LoadScene("Title");
        }

        public IEnumerator RenderStrokesToWritingSurface()
        {
            _writingSurface.gameObject.SetActive(true);
            int index = 0;
            var sprites = Resources.LoadAll<Sprite>($@"Sprites/Radiants/anim/{_currentKanji}");
            while (index < sprites.Length)
            {
                _writingSurface.GetComponent<Image>().sprite = sprites[index];
                index++;
                // let's do a little hold on the last stroke to let it soak in for the user
                if (index == sprites.Length)
                {
                    index = 0;
                    yield return new WaitForSeconds(0.5f);
                }

                yield return new WaitForSeconds(0.05f);
            }
        }

    }

}
