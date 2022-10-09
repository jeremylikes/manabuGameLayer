using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Characters;
using System;
using Items;
using Random = UnityEngine.Random;

namespace Calculators
{
    public static class GlobalCalculator
    {
        //public static int CalculateBaseDmg(Character assailant, Character target, Weapon w = null)
        //{
        //    //Attack Values
        //    float strengthMultiplier = assailant.Strength * 0.75f;

        //    //Positional Bonuses

        //    float rawAttackValue = w == null ? strengthMultiplier :
        //        w._power + strengthMultiplier;
        //    float accuracyMultiplier = Random.Range(assailant.Accuracy * 0.01f, 1f);
        //    float staminaMultiplier = 1.5f * (assailant.Stamina / assailant.MaxStamina);
        //    float enchanemntBonus = 0f;
        //    if (w != null)
        //        enchanemntBonus = w._enchantment != null ? w._enchantment.CastedPower * 2f :
        //        0f;
        //    float totalAttackValue = (rawAttackValue + enchanemntBonus) * accuracyMultiplier * staminaMultiplier;
        //    //Defence Values
        //    float targetRawDefenceValue = target.GetTotalDefence();
        //    //Final Calculation
        //    float totalDamageOutput = (totalAttackValue) - targetRawDefenceValue;
        //    if (totalDamageOutput <= 0f)
        //        totalDamageOutput = 0f;
        //    return Mathf.RoundToInt(totalDamageOutput);
        //}

        public static int CalculateDamage(Character assailant, Character target, out bool wasCritical)
        {
        //Factors: Resolve, Strength, Weight, Weapon's Attack Value
            float accuracyFactor = Random.Range(0.6f, 1f);
            float rawPower = assailant.GetCharacterStat(CharacterStats.Strength)._current;
            float defFactor = target.GetCharacterStat(CharacterStats.Defense)._current;
            float totalDmgPotential = rawPower - defFactor;
            float criticalFactor = GetYesNoChance(10f) ? 1.75f : 1f; // 10% chance for a crit
            int actualDamage = (int)Math.Ceiling(totalDmgPotential * accuracyFactor * criticalFactor);
            wasCritical = criticalFactor > 1f;
            return actualDamage;
        }

        public static float VectorFloatToAngle(Vector3 direction)
        {
            direction = direction.normalized;
            float i = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (i <= 0f) i += 360f;
            return i;
        }

        public static float GetStatCelingValue()
        {
            return 999f;
        }

        public static float GetGlobalTickInterval()
        {
            return 2f;
        }

        public static float GetDefaultAttackDelay()
        {
            return 0.4f;
        }

        //public static float GetAttackDelay(List<Weapon> equippedWeapons)
        //{
        //    float netWeight = 0f;
        //    float delayFactor = 0.04f;
        //    foreach (Weapon w in equippedWeapons)
        //    {
        //        if (!w.IsTwoHanded())
        //            netWeight += w._weight;
        //        else
        //            return w._weight * delayFactor;
        //    }
        //    return netWeight * delayFactor;
        //}

        public static bool GetYesNoChance(float percentChance)
        {
            //We will allow percents represented as either whole numbers or decimals
            if (percentChance > 1f)
            {
                if (percentChance > 100f)
                    percentChance = 100f;
                percentChance *= 0.01f;
            }
                
            float rand = UnityEngine.Random.value;
            //return rand > (1-percentChance);
            return rand <= percentChance;
        }

        //public static int GetMPRegenValue(Character c)
        //{
        //    //factors: mysticism
        //    return Mathf.RoundToInt(1 + (c.MaxMP * 0.02f) +
        //        c.Mysticism);
        //}

        //public int GetHPRegenValue(Character c)
        //{
        //    //factors: maxHP
        //    return Mathf.RoundToInt(c.MaxHP * 0.01f);
        //}

        public static int GetRegenValuePerTick(Character c, CharacterStats stat)
        {
            //factors: resolve
            //return (int)Math.Ceiling(c.GetCharacterStat(CharacterStats.Resolve)._current * 0.05);
            switch (stat)
            {
                case CharacterStats.HP:
                    return 2;
                case CharacterStats.MP:
                    return 1;
                case CharacterStats.Stamina:
                    //return (int)Math.Floor((double)c.GetTotalWeaponWeight() / 2);
                    return 1;
                default:
                    return 0;

            }
        }

        public static float GetAttackDelay(Character c)
        {
            //TODO: This will need to be refactored if dual wield implemented
            //For the MVP we will just use a value that "feels good"
            //return c.GetEquippedWeapons()[0]._weight / 2f;
            return 0.6f;
        }

        //public static float GetWeaponWeightDrag(Character c)
        //{
        //    List<Weapon> weapons = c.GetEquippedWeapons();
        //    float weightFactor = 0.15f;
        //    return weapons[0].IsTwoHanded() ? weapons[0]._weight * weightFactor :
        //        weapons.Sum(w => w._weight) * weightFactor;
        //}

    }

}
