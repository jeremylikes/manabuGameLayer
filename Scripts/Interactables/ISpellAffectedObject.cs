using Spells;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    public interface ISpellAffectedObject
    {
        SpellNames GetSpellAffectedBy();
        void ReactToSpell();
    }

}
