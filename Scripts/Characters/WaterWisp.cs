using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Items;
using Controllers;
using System.Linq;
using System;
using System.Security.Cryptography;
using Calculators;
using Managers;
using Interactables;
using UnityEngine.Assertions.Must;
using TMPro;
using System.Net;
using System.IO;
using Spells;

public class WaterWisp : Character
{

    public override CharacterTypes GetCharacterType()
    {
        return CharacterTypes.Enemy;
    }

    public override string GetName()
    {
        var langCode = GameStateManager._instance.GetCurrentLanguageCode();
        var name = Resources.Load($@"Messages/Characters/Enemies/WaterWisp/{langCode}/waterWispName") as TextAsset;
        return name.text;
    }

    public override void SetBaseStats()
    {
        _isAlive = true;
        _stats[CharacterStats.HP] = new Stat(30, 30);
        _stats[CharacterStats.MP] = new Stat(40, 40);
        _stats[CharacterStats.Speed] = new Stat(1f, 1f);
        _stats[CharacterStats.Stamina] = new Stat(30, 30);
        _stats[CharacterStats.Defense] = new Stat(10, 10);
        _stats[CharacterStats.Strength] = new Stat(15, 15);
        _stats[CharacterStats.Resolve] = new Stat(10, 10);
        _stats[CharacterStats.Sight] = new Stat(1.5f, 1.5f);
    }

    void Start()
    {
        //_movementSpeed = _movementSpeed == 0 ? 0.8f : _movementSpeed;
        //GetComponent<SphereCollider>().radius = 4f; // the enemy's "sight" / aggro radius
        SetBaseStats();
    }

    public override void Attack(Character target)
    {
        StartCoroutine(CastTyphoon(target));
    }

    private IEnumerator CastTyphoon(Character target)
    {
        var typhoon = Resources.Load<GameObject>(@"Prefabs/Spells/Typhoon");
        var anim = GetComponent<Animator>();
        var controller = GetComponent<EnemyAI>();
        anim.SetTrigger("cast");
        yield return new WaitForSeconds(1.4f);
        Instantiate(typhoon, target.transform.position, Quaternion.identity);
        controller._attackFinished = true;
        //var anim = GetComponent<Animator>();
        //var controller = GetComponent<EnemyAI>();
        //controller._attacking = true;
        //anim.SetTrigger("windup");
        //float windupAnimDelay = 0.75f;
        //yield return new WaitForSeconds(windupAnimDelay);
        //anim.SetBool("attack", true);
        //var dir = controller.GetCurrentDirectionVector();
        //var hits = Physics2D.CircleCastAll(transform.position, 0.1f, dir, 0.5f);
        //foreach (var hit in hits)
        //{
        //    if (hit.collider.tag == "Player")
        //    {
        //        target.TakeDamage(transform, 25);
        //        break;
        //    }
        //}
        //anim.SetBool("attack", false);
        //float attackAnimDelay = 1f;
        //yield return new WaitForSeconds(attackAnimDelay);
        //controller._attacking = false;
        //controller._attackFinished = true;
        //_stingCountdownStarted = false;
    }

    public override string GetPrefabPath()
    {
        return @"Prefabs/Characters/Enemies/WaterWisp";
    }


    public override int GetSpellDamage(SpellNames spellName)
    {
        switch (spellName)
        {
            case SpellNames.Fireball:
                return 2;
            case SpellNames.Ice:
                return 35;
            default:
                return 0;
        }
    }
}
