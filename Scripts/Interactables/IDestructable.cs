using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    public interface IDestructable
    {
        void TakeDamage(int amount);
    }
}

