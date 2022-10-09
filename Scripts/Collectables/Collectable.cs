using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using Managers;

namespace Collectables
{
    [Serializable]
    public class CollectableDrop
    {
        public PhysicalCollectable _collectable;
        public float _dropRate = 0.5f;
    }
    public abstract class Collectable
    {
        public string GetName()
        {
            var path = FileManagement.MessagesCollectablesDirectory;
            var name = Resources.Load($"{path}/{GetStringName()}/name") as TextAsset;
            return name.text;
        }
        public string GetDescription()
        {
            var path = FileManagement.MessagesCollectablesDirectory;
            var description = Resources.Load($"{path}/{GetStringName()}/description") as TextAsset;
            return description.text;
        }

        public abstract CollectableNames GetReferenceName();

        public string GetStringName() { return GetReferenceName().ToString(); }
    }
}

