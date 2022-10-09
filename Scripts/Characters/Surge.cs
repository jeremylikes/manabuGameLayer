using Managers;
using Spells;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Characters
{
    public class Surge : Character
    {

        private void Awake()
        {
            SetBaseStats();
        }

        public override CharacterTypes GetCharacterType()
        {
            return CharacterTypes.Boss;
        }

        public override string GetName()
        {
            var langCode = GameStateManager._instance.GetCurrentLanguageCode();
            var name = Resources.Load($"Messages/Characters/Faces/Surge/{langCode}/surgeName") as TextAsset;
            return name.text;
        }

        public override string GetPrefabPath()
        {
            return "";
        }

        public override void SetBaseStats()
        {
            _isAlive = true;
            _stats[CharacterStats.HP] = new Stat(300, 300);
            _stats[CharacterStats.MP] = new Stat(0, 0);
            _stats[CharacterStats.Speed] = new Stat(0.2f, 0.2f);
            _stats[CharacterStats.Stamina] = new Stat(30, 30);
            _stats[CharacterStats.Defense] = new Stat(10, 10);
            _stats[CharacterStats.Strength] = new Stat(15, 15);
            _stats[CharacterStats.Resolve] = new Stat(10, 10);
            _stats[CharacterStats.Sight] = new Stat(3, 3);
        }

        public override int GetSpellDamage(SpellNames spellName)
        {
            switch (spellName)
            {
                case SpellNames.Fireball:
                    return 20;
                default:
                    return 0;
            }
        }
    }
}

