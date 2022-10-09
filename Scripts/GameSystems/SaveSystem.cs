using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using GameKanji;
using System.Linq;
using Characters;
using Collectables;
using Inventory;
using System;
using Managers;

namespace GameSystems
{
    public static class SaveSystem
    {
        //private static readonly string SAVE_DIR = Path.Combine(Application.persistentDataPath, "Resources/Saves");
        private static readonly string SAVE_DIR = Path.Combine(Application.dataPath, "Resources/Saves");
        private static readonly string[] SAVE_FILE_PATHS = { 
            Path.Combine(SAVE_DIR, "save1.json"),
            Path.Combine(SAVE_DIR, "save2.json"),
            Path.Combine(SAVE_DIR, "save3.json")
        };
        public static readonly string PERSISTANT_DATA_PATH = Path.Combine(SAVE_DIR, "persistantData.json");
        private static readonly string LEARNED_KANJI_FILE_PATH = Path.Combine(SAVE_DIR, "learnedKanji.json");
        private static readonly int MAX_NUMBER_OF_SAVES = 3;

        public static void Init()
        {
            if (!Directory.Exists(SAVE_DIR))
                Directory.CreateDirectory(SAVE_DIR);
        }

        public static string GetSaveTileData(int saveFileIndex)
        {
            if (saveFileIndex > MAX_NUMBER_OF_SAVES)
                return "Max Save File Count Exceeded";
            if (File.Exists(SAVE_FILE_PATHS[saveFileIndex]))
            {
                string saveString = Load(saveFileIndex);
                if (saveString != null)
                {
                    SaveObject saveObj = JsonUtility.FromJson<SaveObject>(saveString);
                    return saveObj._timeOfSave;
                }
            }
            return "Empty";
        }

        public static void Save(string saveString, int saveFileIndex)
        {
            File.WriteAllText(SAVE_FILE_PATHS[saveFileIndex], saveString);
        }

        public static void SavePlayerDataToPersistantData(string data)
        {
            File.WriteAllText(PERSISTANT_DATA_PATH, data);
        }

        public static void SaveToLearnedKanji(string kanjiString)
        {
            List<string> learnedKanji = GetLearnedKanjiList();
            if (learnedKanji.Contains(kanjiString))
                return;
            File.AppendAllText(LEARNED_KANJI_FILE_PATH, $"{kanjiString}\n");

        }

        public static List<string> GetLearnedKanjiList()
        {
            List<string> learnedKanji = new List<string>();
            if (File.Exists(LEARNED_KANJI_FILE_PATH))
            {
                string textBlob = File.ReadAllText(LEARNED_KANJI_FILE_PATH);
                var split = textBlob.Split('\n');
                foreach (var line in split)
                    if (line != "")
                        learnedKanji.Add(line);
            }
            return learnedKanji;
        }

        public static bool VerifySaveFilesExist()
        {
            foreach (var path in SAVE_FILE_PATHS)
            {
                if (File.Exists(path))
                {
                    return true;
                }
            }
            return false;
            
        }

        public static int GetMostRecentSaveFileIndex()
        {
            int mostRecentIndex = 0;
            DateTime lastTimestamp = new DateTime(2020, 1, 1);
            foreach (var file in Directory.GetFiles(SAVE_DIR))
            {
                if (!file.Contains("save"))
                    continue;
                DateTime currTime = File.GetLastWriteTime(file);
                if (DateTime.Compare(currTime, lastTimestamp) > 0)
                {
                    lastTimestamp = currTime;
                    for(int i = 0; i < SAVE_FILE_PATHS.Length; i++)
                    {
                        if (SAVE_FILE_PATHS[i] == file)
                            mostRecentIndex = i;
                    }
                }
            }
            return mostRecentIndex;
        }

        public static string Load(int saveFileIndex)
        {
            if (File.Exists(SAVE_FILE_PATHS[saveFileIndex]))
            {
                string saveString = File.ReadAllText(SAVE_FILE_PATHS[saveFileIndex]);
                return saveString;
            }
            else
            {
                return null;
            }

        }

        public static string LoadPersistantData()
        {
            if (File.Exists(PERSISTANT_DATA_PATH))
            {
                string saveString = File.ReadAllText(PERSISTANT_DATA_PATH);
                return saveString;
            }
            else
            {
                return null;
            }
        }

    }

    public class SaveObject
    {
        public string _timeOfSave;
        public int _currentSceneIndex;
        public float _currentHP;
        public float _currentMP;
        public int _currentLevel;
        public Vector3 _currentPosition;
        public List<EquipmentData> _equippedItems;
        public List<CollectableNames> _equipment;
        public List<ItemData> _items;
        public List<string> _knownKanji;
        public List<int> _resolvedKeyPoints;
        public GameStateManager.GameLanguages _activeLanguage;
        //public Dictionary<int, string> _inventoryInfo, _equipmentInfo;
        //public List<(int, string)> _inventoryInfo, _equipmentInfo;
        //public List<SlotData> _inventoryData, _equipmentData;
    }
}

