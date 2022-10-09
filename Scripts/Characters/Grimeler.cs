using Items;
using Spells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters {
    public class Grimeler : Character
    {

        private void Start()
        {
            //_movementSpeed = 0.7f;
            SetBaseStats();
        }

        public override CharacterTypes GetCharacterType()
        {
            return CharacterTypes.Enemy;
        }

        public override string GetName()
        {
            return "Grimeler";
        }

        public override void SetBaseStats()
        {
            _isAlive = true;
            _stats[CharacterStats.HP] = new Stat(50, 50);
            _stats[CharacterStats.MP] = new Stat(40, 40);
            _stats[CharacterStats.Speed] = new Stat(15, 15);
            _stats[CharacterStats.Stamina] = new Stat(30, 30);
            _stats[CharacterStats.Defense] = new Stat(10, 10);
            _stats[CharacterStats.Strength] = new Stat(15, 15);
            _stats[CharacterStats.Resolve] = new Stat(10, 10);
            _stats[CharacterStats.Sight] = new Stat(7, 7);
        }

        public override string GetPrefabPath()
        {
            throw new System.NotImplementedException();
        }

        public override int GetSpellDamage(SpellNames spellName)
        {
            switch (spellName)
            {
                case SpellNames.Fireball:
                    return 15;
                default:
                    return 0;
            }
        }
    }
}

