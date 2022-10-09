using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using GameSystems;
using System.IO;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameOverMenu : Menu
    {
        [SerializeField] private Transform _loadGameButton;


        public void Start()
        {
            AudioManager._instance.StopBGM();
            var ac = Resources.Load<AudioClip>(@"Audio/SE/Events/gameOver");
            AudioManager._instance.PlaySoundEffect(ac);
        }

        public void SetMenuLayout()
        {
            if (SaveSystem.VerifySaveFilesExist())
            {
                _loadGameButton.gameObject.SetActive(true);
                _menuNodes.Add(_loadGameButton);
                _layout._columns = 2;
            }

        }

        public void ReturnToTitleScreen()
        {
            if (Time.timeScale != 1f)
                Time.timeScale = 1f;
            SceneManager.LoadScene("Title");
        }

        public void LoadMostRecentSaveFile()
        {
            if (!SaveSystem.VerifySaveFilesExist())
                return;
            int mostRecentIndex = SaveSystem.GetMostRecentSaveFileIndex();
            //GameStateManager._instance.Load(mostRecentIndex/*, true*/);
            GameStateManager._instance.LoadAysnc(mostRecentIndex);
            //CloseMenu();
        }
    }

}
