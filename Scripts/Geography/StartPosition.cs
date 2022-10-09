using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geography
{
    public class StartPosition : MonoBehaviour
    {
        [SerializeField] bool _activateStartPosition;

        void Start()
        {
            if (_activateStartPosition)
            {
                Manabu manabu = FindObjectOfType<Manabu>();
                manabu.transform.position = transform.position;
            }
        }


    }
}

