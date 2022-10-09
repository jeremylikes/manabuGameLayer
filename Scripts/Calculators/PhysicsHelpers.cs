using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;
using System;
using Effects;
using Interactables;
using System.Linq;
using Controllers;

namespace Calculators
{

    public class PhysicsHelpers
    {
        public List<Character> GetCharactersWithinRadius2D(Vector3 origin, float targetRadius)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, targetRadius);
            List<Character> affectedTargets = new List<Character>();
            foreach (Collider2D c in colliders)
            {
                if (c.GetComponent<Character>())
                {
                    var target = c.GetComponent<Character>();
                    affectedTargets.Add(target);
                }

            }
            return affectedTargets;
        }

        public List<Character> GetCharactersWithinRadius2D(Vector3 origin, CharacterTypes characterType, float targetRadius)
        {

            //Collider[] colliders = Physics.OverlapSphere(transform.position, _effectRadius);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, targetRadius);
            List<Character> affectedTargets = new List<Character>();
            foreach (Collider2D c in colliders)
            {
                if (c.GetComponent<Character>())
                {
                    var target = c.GetComponent<Character>();
                    if (target.GetCharacterType() == characterType && !affectedTargets.Contains(target))
                        affectedTargets.Add(target);
                }

            }
            return affectedTargets;
        }

        public List<ITimeInteractable> GetObjectsAffectedByTime(Vector3 origin, float targetRadius)
        {

            //Collider[] colliders = Physics.OverlapSphere(transform.position, _effectRadius);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, targetRadius);
            List<ITimeInteractable> affectedTargets = new List<ITimeInteractable>();
            foreach (Collider2D c in colliders)
            {
                if (c.GetComponent<ITimeInteractable>() != null)
                    affectedTargets.Add(c.GetComponent<ITimeInteractable>());

            }
            return affectedTargets;
        }

        public void SimulateWindResistance(Transform target, float force)
        {
            var rb = target.GetComponent<Rigidbody2D>() ?? null;
            if (rb != null)
            {
                rb.drag = force;
            }
            else
                Debug.Log("There was no rigid body found when trying to add Wind Resistance.");

        }

        

        public IEnumerator ProcessKnockback2D(Transform assailant, Transform target)
        {
            //Disable controls if manabu
            bool targetIsManabu = target.GetComponent<Manabu>() != null;
            var previousControls = ControlsManager._instance.GetCurrentControlSchema();
            if (targetIsManabu && previousControls == ControlsManager.ControlSchema.Active)
            {
                ControlsManager._instance.SetLockedControls();
            }
            float power = targetIsManabu  ? 1.5f : 0.7f;
            Vector3 knockback = new Vector3(0f, 0f, 0f);
            var assailantPos = assailant.position;
            var targetPos = target.position;
            float xDiff = Math.Abs(
                assailantPos.x - targetPos.x);
            float yDiff = Math.Abs(
                assailantPos.y - targetPos.y);

            if (xDiff > yDiff)
            {
                if (assailantPos.x > targetPos.x)
                    knockback = new Vector3(-power, 0f, 0f);
                else
                    knockback = new Vector3(power, 0f, 0f);
            }
            else
            {
                if (assailantPos.y < targetPos.y)
                    knockback = new Vector3(0f, power, 0f);
                else
                    knockback = new Vector3(0f, -power, 0f);
            }

            //var knockbackVector = target.position + knockback;
            float rate = 0.0f;
            float timer = 0.3f;
            while (timer > 0f)
            {
                rate += 4f * Time.deltaTime;
                //target.position = Vector3.Lerp(targetPos, knockbackVector, rate);
                target.GetComponent<Rigidbody2D>().velocity = knockback;
                timer -= Time.deltaTime;
                yield return null;
            }
            target.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            if (targetIsManabu && previousControls == ControlsManager.ControlSchema.Active)
            {
                ControlsManager._instance.SetControls(previousControls);
            }
        }
    }
}

