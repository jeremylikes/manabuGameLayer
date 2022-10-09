using Characters;
using Collectables;
using GameSystems;
using Spells;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameStateManager : MonoBehaviour
    {
        /// <summary>
        /// The grand daddy class that needs to stay static throughout all scenes
        /// </summary>
        public static GameStateManager _instance = null;
        [SerializeField] private List<int> _resolvedKeyPointIndeces;
        [SerializeField] private MapManager _mapManager;
        private bool _loadFromSave = false;
        public bool _playerDataWasSaved = false;
        public enum Platforms { pc, mobile };
        [SerializeField] private Platforms _currentPlatform;
        public bool _tutorialMode = false;

        public Platforms CurrentPlatform { get => _currentPlatform; set => _currentPlatform = value; }
        private List<SceneNames> _nonPlayerDataScenes = new List<SceneNames>()
        {
            SceneNames.ChooseLanguage, SceneNames.Splash, SceneNames.DesertTransport,
            SceneNames.Title, SceneNames.Prologue, SceneNames.SystemRefinement,
            SceneNames.Prologue
        };

        #region LanguageSettings

        public enum GameLanguages { English, Japanese }
        public GameLanguages CurrentLanguage => _currentLanguage;
        [SerializeField] private GameLanguages _currentLanguage;
        public string GetCurrentLanguageCode()
        {
            string code = "";
            switch (_currentLanguage)
            {
                case GameLanguages.English:
                    code = "en-US";
                    break;
                case GameLanguages.Japanese:
                    code = "ja-JP";
                    break;
                default:
                    code = "en-US";
                    break;
            }
            return code;
        }
        public void SetGameLanguage(GameLanguages lang) => _currentLanguage = lang;

        #endregion

        public void AddToResolvedKeyPoints(int index)
        {
            _resolvedKeyPointIndeces.Add(index);
        }
        private bool IsNonPlayerDataScene(string sceneName)
        {
            foreach (var scene in _nonPlayerDataScenes)
            {
                if (scene.ToString() == sceneName)
                    return true;
            }
            return false;
        }

        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else if (_instance != this)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);

            if (IsNonPlayerDataScene(SceneManager.GetActiveScene().name))
            {
                _playerDataWasSaved = false;
            }

            var mapManager = FindObjectOfType<MapManager>() ?? null;
            if (mapManager != null)
                _mapManager = mapManager;
            SceneManager.sceneUnloaded += TriggerPlayerDataSave;
            SceneManager.sceneLoaded += TriggerPlayerDataLoad;
            SceneManager.sceneLoaded += CheckTutorialMode;
        }

        private void CheckTutorialMode(Scene one, LoadSceneMode mode)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            _tutorialMode = sceneName == SceneNames.Intro.ToString() ? true : false;
            if (sceneName == SceneNames.Intro.ToString() || sceneName == SceneNames.Prologue.ToString())
                _tutorialMode = true;
            if (_tutorialMode)
                GameManager._instance.ToggleStatPanel(false);
        }

        //public string GetLocalizedDialoguePath()
        //{
        //    return $@"Messages/{GetCurrentLanguageCode()}/Dialogue";
        //}

        public void Save(int saveFileIndex)
        {
            var manabu = GameManager._instance._mainCharacter;

            SaveObject saveObj = new SaveObject
            {
                _timeOfSave = System.DateTime.Now.ToString("MM-dd-yy HH:mm"),
                _currentSceneIndex = SceneManager.GetActiveScene().buildIndex,
                _currentHP = manabu.GetCharacterStat(CharacterStats.HP)._current,
                _currentMP = manabu.GetCharacterStat(CharacterStats.MP)._current,
                _currentPosition = manabu.transform.position,
                _equippedItems = manabu._equipmentInventory.GetEquippedItems(),
                _equipment = manabu._equipmentInventory.GetEquipmentInventory(),
                _items = manabu._itemInventory.GetItemInventory(),
                _knownKanji = manabu._kanjiInventory.GetKnownKanji(),
                _resolvedKeyPoints = _resolvedKeyPointIndeces,
                _activeLanguage = _currentLanguage
            };
            string json = JsonUtility.ToJson(saveObj);
            SaveSystem.Save(json, saveFileIndex);
            AudioClip ac = Resources.Load<AudioClip>(@"Audio/SE/UI/gameSaved");
            AudioManager._instance.PlaySoundEffect(ac);
        }

        private void TriggerPlayerDataSave(Scene one)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (_loadFromSave || IsNonPlayerDataScene(sceneName))
                return;
            SaveToPersistantData();
        }

        private void TriggerPlayerDataLoad(Scene scene, LoadSceneMode mode)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            if (IsNonPlayerDataScene(sceneName))
            {
                _playerDataWasSaved = false;
                return;
            }
            if (!_loadFromSave && _playerDataWasSaved)
                LoadPersistantData();
        }

        public void SaveToPersistantData()
        {
            var manabu = GameManager._instance._mainCharacter;
            SaveObject saveObj = new SaveObject
            {
                _currentHP = manabu.GetCharacterStat(CharacterStats.HP)._current,
                _currentMP = manabu.GetCharacterStat(CharacterStats.MP)._current,
                _equippedItems = manabu._equipmentInventory.GetEquippedItems(),
                _equipment = manabu._equipmentInventory.GetEquipmentInventory(),
                _items = manabu._itemInventory.GetItemInventory(),
                _knownKanji = manabu._kanjiInventory.GetKnownKanji(),
            };

            string json = JsonUtility.ToJson(saveObj);
            _playerDataWasSaved = true;
            SaveSystem.SavePlayerDataToPersistantData(json);
        }
        // If you pass in a default parameter like I tried to do below
        // the function will no longer be exposed to the inspector
        // and will break association with any buttons, etc that are wired up
        public void Load(int saveFileIndex/*, bool forceReloadLevel = false*/)
        {
            string saveString = SaveSystem.Load(saveFileIndex);
            if (saveString != null)
            {
                SaveObject saveObj = JsonUtility.FromJson<SaveObject>(saveString);
                if (saveObj._currentSceneIndex != SceneManager.GetActiveScene().buildIndex /*|| forceReloadLevel*/)
                {
                    StartCoroutine(AsyncSceneLoad(saveObj, saveFileIndex));

                }
                else
                {
                    SetPlayerDataFromSave(saveObj, saveFileIndex);
                }

            }

        }

        public void LoadPersistantData()
        {
            string saveString = SaveSystem.LoadPersistantData();
            if (saveString != null)
            {
                SaveObject saveObj = JsonUtility.FromJson<SaveObject>(saveString);
                SetPlayerDataFromPersistantData(saveObj);
            }
        }

        public void LoadAysnc(int saveFileIndex)
        {
            string saveString = SaveSystem.Load(saveFileIndex);
            if (saveString != null)
            {
                SaveObject saveObj = JsonUtility.FromJson<SaveObject>(saveString);
                StartCoroutine(AsyncSceneLoad(saveObj, saveFileIndex));
            }
        }

        private IEnumerator AsyncSceneLoad(SaveObject saveObj, int saveFileIndex)
        {
            _loadFromSave = true;
            AsyncOperation loadAsymc = SceneManager.LoadSceneAsync(saveObj._currentSceneIndex);
            while (!loadAsymc.isDone)
                yield return null;
            SetPlayerDataFromSave(saveObj, saveFileIndex);
            _loadFromSave = false;
        }

        private void SetPlayerDataFromPersistantData(SaveObject saveObj)
        {
            var manabu = GameManager._instance._mainCharacter;

            // Stats
            manabu.SetBaseStats();
            manabu.SetStat(CharacterStats.HP, saveObj._currentHP);
            manabu.SetStat(CharacterStats.MP, saveObj._currentMP);

            manabu._equipmentInventory.ClearEquipmentData();
            manabu._itemInventory.ClearItemInventory();
            manabu._kanjiInventory.ClearKanjiInventory();

            // Items
            foreach (var itemData in saveObj._items)
            {
                Item item = (Item)CollectableManager.GetCollectableByName(itemData._item);
                for (int i = 0; i < itemData._quantity; i++)
                {
                    manabu._itemInventory.AddToItemInventory(item);
                }
            }

            // Equipment
            foreach (var eqData in saveObj._equipment)
            {
                Equipment equipment = (Equipment)CollectableManager.GetCollectableByName(eqData);
                manabu._equipmentInventory.AddToEquipmentInventory(equipment);
            }
            foreach (var equipped in saveObj._equippedItems)
            {
                Equipment alreadyEquipped = (Equipment)CollectableManager.GetCollectableByName(equipped._equipment);
                foreach (Equipment eq in manabu._equipmentInventory.GetRawEquipmentInenvtory())
                {
                    if (eq.GetName() == alreadyEquipped.GetName())
                    {
                        manabu.Equip(eq);
                    }
                }
            }

            // Kanji
            foreach (var kanji in saveObj._knownKanji)
            {
                manabu._kanjiInventory.AddToKnownKanji(kanji);
            }
        }

        public void SetPlayerDataFromSave(SaveObject saveObj, int saveFileIndex)
        {
            if (SceneManager.GetActiveScene().name != "Title")
                _mapManager = FindObjectOfType<MapManager>();
            var manabu = GameManager._instance._mainCharacter;
            manabu.SetBaseStats();
            manabu.SetStat(CharacterStats.HP, saveObj._currentHP);
            manabu.SetStat(CharacterStats.MP, saveObj._currentMP);
            manabu.transform.position = saveObj._currentPosition;

            // Clear inventories
            manabu._equipmentInventory.ClearEquipmentData();
            manabu._itemInventory.ClearItemInventory();
            manabu._kanjiInventory.ClearKanjiInventory();

            // Add to inventories based on saved data
            foreach (var itemData in saveObj._items)
            {
                Item item = (Item)CollectableManager.GetCollectableByName(itemData._item);
                for (int i = 0; i < itemData._quantity; i++)
                {
                    manabu._itemInventory.AddToItemInventory(item);
                }
            }
            foreach (var eqData in saveObj._equipment)
            {
                Equipment equipment = (Equipment)CollectableManager.GetCollectableByName(eqData);
                manabu._equipmentInventory.AddToEquipmentInventory(equipment);
            }
            foreach (var equipped in saveObj._equippedItems)
            {
                Equipment alreadyEquipped = (Equipment)CollectableManager.GetCollectableByName(equipped._equipment);
                foreach (Equipment eq in manabu._equipmentInventory.GetRawEquipmentInenvtory())
                {
                    if (eq.GetName() == alreadyEquipped.GetName())
                    {
                        manabu.Equip(eq);
                    }
                }
            }
            foreach (var kanji in saveObj._knownKanji)
            {
                manabu._kanjiInventory.AddToKnownKanji(kanji);
            }
            if (SceneManager.GetActiveScene().name != "Title")
            {
                foreach (int index in saveObj._resolvedKeyPoints)
                {
                    //_keyPoints[index].ResolveState();
                    _mapManager.ResolveKeyPoint(index);
                }
            }
            _currentLanguage = saveObj._activeLanguage;

        }

        public void ReturnToTile()
        {
            if (Time.timeScale != 1f)
                Time.timeScale = 1f;
            SceneManager.LoadScene("Title");
        }

    }
}

