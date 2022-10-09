using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using System.IO;
using System.Text.RegularExpressions;
using GameSystems;
//using UnityEditor.UIElements;
using UI;
using Controllers;
using Effects;
using Messages;
using UnityEngine.UI;
using Spells;
using Interactables;
using TMPro;
using System;
using Geography;
using Items;
using UnityEngine.SceneManagement;
using System.Runtime.Remoting.Messaging;
using Calculators;
using Inventory;
using Microsoft.Win32.SafeHandles;
using Collectables;

namespace Managers
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager _instance = null;
        private float _globalSpawnTime = 20f;
        public Manabu _mainCharacter;
        public CanvasManager _canvasManager;
        public AudioManager _audioManager;
        public CharacterController _characterController;
        public DebugMessages _debugMessages;
        public MessageFX _messageFX;
        public TextMeshProUGUI _systemMessageText;
        public float _defaultCameraOrthoSize;
        public Image _headsUpImage;
        public event EventHandler _onLearnedKanjiCutsceneFinsihed;
        //public MapInfo _currentMap;
        public Coroutine _respawnCoroutine;
        private Vector3 _targetPlayerPositionAfterLoad;
        [SerializeField] private GameOverMenu _gameOverMenu;
        [SerializeField] private Transform _pauseMenu;
        [SerializeField] private TextMeshProUGUI _pauseText;
        [SerializeField] private TextMeshProUGUI _unpauseHint;
        [SerializeField] private Transform _statPanel;

        private bool _paused = false;

        public void ToggleStatPanel(bool setting)
        {
            if (_statPanel != null)
                _statPanel.gameObject.SetActive(setting);
        }
        void Awake()
        {
            SaveSystem.Init();
            if (_instance == null)
                _instance = this;
            //else if (_instance != this)
            //    Destroy(gameObject);
            //DontDestroyOnLoad(gameObject);
        }

        public void ReturnToTile()
        {
            if (Time.timeScale != 1f)
                Time.timeScale = 1f;
            SceneManager.LoadScene("Title");
        }

        public void TriggerGameOver()
        {
            _gameOverMenu.InitMenu();
        }

        public void ProcessKnockBack(Transform assailant, Transform target)
        {
            var ph = new PhysicsHelpers();
            StartCoroutine(ph.ProcessKnockback2D(assailant, target));

        }

        public void OnEnemyPerish(string pathToPrefab, Vector3 startingPos, Vector2 minCoords, Vector2 maxCoords)
        {
            StartCoroutine(RespawnEnemy(pathToPrefab, startingPos, minCoords, maxCoords));
            // Maybe do some other glorious things
        }

        public IEnumerator RespawnEnemy(string pathToPrefab, Vector3 startingPos, Vector2 minCoords, Vector2 maxCoords)
        {
            yield return new WaitForSeconds(_globalSpawnTime);
            var mob = Resources.Load<GameObject>($"{pathToPrefab}");
            var con = mob.GetComponent<EnemyAI>();
            con._min = minCoords;
            con._max = maxCoords;
            mob.GetComponent<Character>().SetRespawn(true);
            Instantiate(mob, startingPos, Quaternion.identity);

        }

        void Start()
        {
            _mainCharacter = FindObjectOfType<Manabu>();
            _characterController = _mainCharacter.GetComponent<CharacterController>();
            _defaultCameraOrthoSize = Camera.main.orthographicSize;
        }

        public void TogglePause()
        {
            var unityPause = Resources.Load($"{FileManagement.MessagesUIDirectory}/Menus/pauseMenu") as TextAsset;
            var pauseText = unityPause.text;

            var unityUnpause = Resources.Load($"{FileManagement.MessagesUIDirectory}/Menus/unpauseHint") as TextAsset;
            var unpauseHint = unityUnpause.text;
            _pauseText.text = pauseText;
            _unpauseHint.text = unpauseHint;
            if (!_paused)
            {
                Time.timeScale = 0f;
                ControlsManager._instance.SetPauseMenuControls();
                _pauseMenu.gameObject.SetActive(true);
                _paused = true;
                AudioManager._instance.PauseAllSound(true);
            }
            else
            {
                ControlsManager._instance.SetActiveControls();
                _pauseMenu.gameObject.SetActive(false);
                _paused = false;
                Time.timeScale = 1f;
                AudioManager._instance.PauseAllSound(false);
            }
        }


    }


}

