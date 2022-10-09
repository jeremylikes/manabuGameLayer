using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class ObjectScaler : MonoBehaviour
    {
        //bool _grow;
        public bool _terminateAfterMaxScale = false;
        public Vector3 _growIncrement = new Vector3(3f, 3f, 0f);
        public float _timer = 5f;

        // Update is called once per frame
        void Update()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
                transform.localScale += _growIncrement;
            }
            else
                this.enabled = false;
        }
    }
}

