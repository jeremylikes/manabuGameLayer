using Characters;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Collectables
{
    public class GrappleHook : DaxExpansion
    {

        public override string GetPathToPrefab()
        {
            return @"Prefabs/GrappleHook";
        }

        public override CollectableNames GetReferenceName()
        {
            return CollectableNames.GrappleHook;
        }
    }

}
