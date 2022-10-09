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

public class Wasp : Character
{
    private bool _stingCountdownStarted = false;

    public override CharacterTypes GetCharacterType()
    {
        return CharacterTypes.Enemy;
    }

    public override string GetName()
    {
        var langCode = GameStateManager._instance.GetCurrentLanguageCode();
        var name = Resources.Load($"Messages/Characters/Enemies/Wasp/{langCode}/waspName") as TextAsset;
        return name.text;
    }

    public override void SetBaseStats()
    {
        _isAlive = true;
        _stats[CharacterStats.HP] = new Stat(25, 25);
        _stats[CharacterStats.MP] = new Stat(40, 40);
        _stats[CharacterStats.Speed] = new Stat(0.9f, 0.9f);
        _stats[CharacterStats.Stamina] = new Stat(30, 30);
        _stats[CharacterStats.Defense] = new Stat(10, 10);
        _stats[CharacterStats.Strength] = new Stat(15, 15);
        _stats[CharacterStats.Resolve] = new Stat(10, 10);
        _stats[CharacterStats.Sight] = new Stat(2f, 2f);
    }

    void Start()
    {
        //_movementSpeed = _movementSpeed == 0 ? 0.8f : _movementSpeed;
        //GetComponent<SphereCollider>().radius = 4f; // the enemy's "sight" / aggro radius
        SetBaseStats();
    }

    public override void Attack(Character target)
    {
        StartCoroutine(Sting(target));
    }

    private IEnumerator Sting(Character target)
    {

        var anim = GetComponent<Animator>();
        var controller = GetComponent<EnemyAI>();
        controller._attacking = true;
        anim.SetTrigger("windup");
        float windupAnimDelay = 0.75f;
        var dir = target.transform.position - transform.position;
        yield return new WaitForSeconds(windupAnimDelay);
        if (_attackSound.Count > 0)
            GetComponent<AudioSource>().PlayOneShot(_attackSound[0]);
        anim.SetBool("attack", true);
        //var dir = controller.GetCurrentDirectionVector();
        float swoopPower = 7f;
        var swoopVect = dir * swoopPower;
        GetComponent<Rigidbody2D>().velocity = swoopVect;
        //GetComponent<Rigidbody2D>().AddForce(swoopVect, ForceMode2D.Impulse);
        float swoopTimer = 0.5f;
        bool connectedWithTarget = false;
        while (swoopTimer > 0f)
        {
            var hits = Physics2D.CircleCastAll(transform.position, 0.2f, dir, 0.2f);
            foreach (var hit in hits)
            {
                if (hit.collider.tag == "Player" && !connectedWithTarget)
                {
                    connectedWithTarget = true;
                    target.TakeDamage(transform, 25);
                    break;
                }
            }
            swoopTimer -= Time.deltaTime;
            yield return null;
        }
        //GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        anim.SetBool("attack", false);
        float attackAnimDelay = 1f;
        yield return new WaitForSeconds(attackAnimDelay);
        if (GetComponent<EnemyAI>() != null)
        {
            controller._attacking = false;
            controller._attackFinished = true;
            _stingCountdownStarted = false;
        }

        
    }

    public override string GetPrefabPath()
    {
        return @"Prefabs/Characters/Enemies/Wasp";
    }

    public override int GetSpellDamage(SpellNames spellName)
    {
        switch (spellName)
        {
            case SpellNames.Fireball:
                return 25;
            case SpellNames.Ice:
                return 30;
            case SpellNames.Water:
                return 35;
            default:
                return 0;
        }
    }
}
