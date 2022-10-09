using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timers
{

    public class KillTimer : MonoBehaviour
    {
        [SerializeField]
        private float _killAfterSeconds = 0f;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(StartKillTimer());
        }

        public IEnumerator StartKillTimer()
        {
            yield return new WaitForSeconds(_killAfterSeconds);
            Destroy(gameObject);
        }

    }

}