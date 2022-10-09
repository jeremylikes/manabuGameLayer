using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace Geography
{
    public class Terrain : MonoBehaviour
    {
        public TerrainTypes _terrain;
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.name == "FootCollider")
            {
                var terrainMgr = collision.gameObject.GetComponentInParent<Character>().GetComponent<TerrainManager>();
                if (terrainMgr.GetActiveTerrain() != _terrain)
                {
                    terrainMgr.ChangeTerrain(_terrain);
                    if (_terrain == TerrainTypes.Water)
                    {
                        collision.gameObject.GetComponent<SpriteMask>().enabled = true;
                        collision.transform.parent.Find("RippleEffect").gameObject.SetActive(true);
                    }
                }

            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // let's get just the foot collider since that's what matters for terrain
            if (collision.transform.name == "FootCollider")
            {
                var terrainMgr = collision.gameObject.GetComponentInParent<Character>().GetComponent<TerrainManager>();
                terrainMgr.ChangeTerrain(TerrainTypes.Normal);
                if (_terrain == TerrainTypes.Water)
                {
                    collision.gameObject.GetComponent<SpriteMask>().enabled = false;
                    collision.transform.parent.Find("RippleEffect").gameObject.SetActive(false);
                }
            }
        }

    }


}

