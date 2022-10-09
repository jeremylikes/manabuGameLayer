
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectables
{
    public enum CollectableNames
    {
        LiquidRed, Dax, IceChunk, CrystalHelmet, GrappleHook, 
        LiquidBlue, WaterKey
    }
    public static class CollectableManager
    {

        public static Collectable GetCollectableByName(CollectableNames name)
        {
            //return _masterCollectableList.Find(x => x._referenceName == name)._parentCollectable;
            switch (name)
            {
                case CollectableNames.LiquidRed:
                    return new LiquidRed();
                case CollectableNames.Dax:
                    return new Dax();
                case CollectableNames.IceChunk:
                    return new IceChunk();
                case CollectableNames.CrystalHelmet:
                    return new CrystalHelmet();
                case CollectableNames.GrappleHook:
                    return new GrappleHook();
                case CollectableNames.LiquidBlue:
                    return new LiquidBlue();
                case CollectableNames.WaterKey:
                    return new WaterKey();
                default:
                    return null;
            }
        }
    }
}
