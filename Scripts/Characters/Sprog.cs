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
using System.IO;
using Spells;

public class Sprog : Character
{

    public Transform _spitBullet;

    public override CharacterTypes GetCharacterType()
    {
        return CharacterTypes.Enemy;
    }

    public override string GetName()
    {
        var langCode = GameStateManager._instance.GetCurrentLanguageCode();
        var name = Resources.Load($"Messages/Characters/Enemies/Sprog/{langCode}/sprogName") as TextAsset;
        return name.text;
    }

    public override void SetBaseStats()
    {
        _isAlive = true;
        _stats[CharacterStats.HP] = new Stat(25, 25);
        _stats[CharacterStats.MP] = new Stat(40, 40);
        _stats[CharacterStats.Speed] = new Stat(0.5f, 0.5f);
        _stats[CharacterStats.Stamina] = new Stat(30, 30);
        _stats[CharacterStats.Defense] = new Stat(8, 8);
        _stats[CharacterStats.Strength] = new Stat(15, 15);
        _stats[CharacterStats.Resolve] = new Stat(10, 10);
        _stats[CharacterStats.Sight] = new Stat(1.5f, 1.5f);
    }

    void Start()
    {
        SetBaseStats();
    }

    public override void Attack(Character target)
    {
        StartCoroutine(SpitAttack(target));
    }

    private IEnumerator SpitAttack(Character target)
    {
        AudioClip ac = Resources.Load<AudioClip>(@"Audio/SE/Characters/Sprog/sprog_spit_charge");
        AudioManager._instance.PlaySoundEffect(ac);
        var controller = GetComponent<EnemyAI>();
        var anim = GetComponent<Animator>();
        anim.SetBool("attack", true);
        float fullWindupLength = 1.6f;
        float preTargetLockWindow = 1.3f;
        yield return new WaitForSeconds(preTargetLockWindow);
        var targetPosAtWarmup = target.transform.position;
        yield return new WaitForSeconds(fullWindupLength - preTargetLockWindow);
        var statePriorToAttack = controller._currentState;
        controller._currentState = EnemyAI._states.Stun;

        anim.SetBool("attack", false);
        ac = Resources.Load<AudioClip>(@"Audio/SE/Characters/Sprog/sprog_spit");
        AudioManager._instance.PlaySoundEffect(ac);

        //Using a prefab sprite
        Transform spitBullet = Instantiate(_spitBullet, transform.position, Quaternion.identity);
        Vector3 shootDir = (targetPosAtWarmup - transform.position).normalized;
        spitBullet.eulerAngles = new Vector3(transform.position.x, transform.position.y, GlobalCalculator.VectorFloatToAngle(shootDir));
        spitBullet.parent = transform; // make sure if the sprog dies the spit goes with it
        //spitBullet.GetComponent<ProjectileMovement>().Setup(shootDir);

        //Set up the raycast and hit if Manabu found
        var dir = targetPosAtWarmup - transform.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, 2f);
        //bool manabuStruck = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.GetComponent<IDestructable>() != null)
            {
                hit.transform.GetComponent<IDestructable>().TakeDamage(15);
                yield return new WaitForSeconds(0.05f);
                Destroy(spitBullet.gameObject);
                break;
            }

            if (hit.transform.GetComponent<Manabu>() /*&& !manabuStruck*/)
            {
                var manabu = hit.transform.GetComponent<Manabu>();
                bool wasCritical;
                var dmg = GlobalCalculator.CalculateDamage(this, manabu, out wasCritical);
                //manabuStruck = true;
                hit.transform.GetComponent<Manabu>().TakeDamage(transform, dmg);
            }

 
        }
        
        controller._currentState = statePriorToAttack;
        //controller.ManageDirection();
        yield return new WaitForSeconds(1.3f);

        if (spitBullet != null)
            Destroy(spitBullet.gameObject);
        if (GetComponent<EnemyAI>() != null)
            controller._attackFinished = true;
    }

    public override string GetPrefabPath()
    {
        return @"Prefabs/Characters/Enemies/Sprog";
    }

    public override int GetSpellDamage(SpellNames spellName)
    {
        switch (spellName)
        {
            case SpellNames.Fireball:
                return 25;
            case SpellNames.Ice:
                return 30;
            default:
                return 0;
        }
    }
}
