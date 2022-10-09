using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameSystems
{
    public class LevelSystem
    {
        public event EventHandler OnExperienceChanged;
        public event EventHandler OnLevelGained;

        private int _currentLevel;
        private int _currentExperience;
        private int _experienceToNextLevel;

        public LevelSystem()
        {
            SetCurrentLevel();
            SetExperienceToNextLevel();
            SetCurrentExperience();
        }


        public void AddExperience(int amount)
        {
            _currentExperience += amount;
            if (_currentExperience >= _experienceToNextLevel)
            {
                _currentLevel++;
                _currentExperience -= _experienceToNextLevel;
                OnLevelGained?.Invoke(this,EventArgs.Empty);
            }
            OnExperienceChanged?.Invoke(this,EventArgs.Empty);
        }

        public int CurrentLevel => _currentLevel;

        public void SetCurrentLevel()
        {
            // check for file
            // if nothing return 1
            _currentLevel = 1;
        }

        public void SetCurrentExperience()
        {
            // check for file
            // if nothing return 0
            _currentExperience = 0;
        }

        public void SetExperienceToNextLevel()
        {
            _experienceToNextLevel = _currentLevel == 1 ?
                100:
                _currentLevel * 100 + (10 * _currentLevel);
        }

        // Get / Set from Save Data
        public void SetExperienceDataFromSaveData()
        {

        }

        public void SaveExperienceDataToSaveData()
        {

        }
    }
}
