using Characters;
using Collectables;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IceChunk : Item
{
    public IceChunk()
    {
        _isUsable = false;
    }

    public override CollectableNames GetReferenceName()
    {
        return CollectableNames.IceChunk;
    }

    public override void Use(Character target)
    {
        return;
    }
}
