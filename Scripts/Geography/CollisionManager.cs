using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Controllers;

namespace Geography
{
    public class CollisionManager : MonoBehaviour
    {
        private void OnCollisionExit2D(Collision2D collision)
        {
            Character c;
            if (c = collision.transform.GetComponent<Character>())
            {
                Debug.Log(c.name);
                if (c.GetCharacterType() == CharacterTypes.Enemy)
                {
                    Debug.Log(collision.transform.name);
                    c.GetComponent<EnemyAI>()._currentState = EnemyAI._states.Rest;
                }
            }
        }
    }
}

