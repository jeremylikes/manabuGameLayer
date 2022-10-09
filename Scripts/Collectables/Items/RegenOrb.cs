using Characters;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenOrb : MonoBehaviour
{
    private enum RegenTypes { health, mana }
    [SerializeField] RegenTypes _regenType;
    [SerializeField] AudioClip _absorb;

    private bool _manabuFound = false;

    private void Update()
    {
        if (!_manabuFound)
            CheckForManabu();
    }

    private void CheckForManabu()
    {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach(var target in possibleTargets)
        {
            var manabu = target.gameObject.GetComponent<Manabu>() ?? null;
            if (manabu != null)
            {
                _manabuFound = true;
                StartCoroutine(AbsorbIntoManabu(manabu));
                return;
            }
        }
    }

    private IEnumerator AbsorbIntoManabu(Manabu manabu)
    {
        var dist = Vector3.Distance(transform.position, manabu.transform.position);
        float currSpeed = 0.1f;
        float maxSpeed = 1.3f;
        while (dist > 0.01f)
        {
            if (currSpeed < maxSpeed)
                currSpeed += Time.deltaTime * 1.2f;
            transform.position = Vector3.MoveTowards(transform.position, manabu.transform.position, Time.deltaTime * currSpeed);
            dist = Vector3.Distance(transform.position, manabu.transform.position);
            yield return null;
        }
        CharacterStats statToAdj = _regenType == RegenTypes.health ? CharacterStats.HP : CharacterStats.MP;
        manabu.AdjustStat(statToAdj, 5f);
        AudioManager._instance.PlaySoundEffect(_absorb);
        Destroy(gameObject);
    }

}
