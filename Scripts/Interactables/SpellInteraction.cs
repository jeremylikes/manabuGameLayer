using System.Collections;
using System.Collections.Generic;
using Spells;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SpellInteraction : MonoBehaviour, ISpellAffectedObject
    {
        [SerializeField] SpellNames _affectedBySpell;

        public SpellNames GetSpellAffectedBy()
        {
            return _affectedBySpell;
        }

        public void ReactToSpell()
        {
            Destroy(gameObject);
        }
    }
}
