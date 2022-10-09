using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

namespace Managers
{
    public enum SceneNames
    {
        ChooseLanguage, DesertTransport, Intro, Practice, Prologue, Splash, SystemRefinement, Title,
        Watergardens, WaterTemple
    }

    public class SceneLoader : MonoBehaviour
    {

        [SerializeField] private Animator _anim;
        [SerializeField] private float _transitionTime = 1f;
        private SceneNames _sceneToLoad;
        
        public void LoadLevel(SceneNames sn, Vector3 spawnPoint, CharacterController._directions playerDirAfterSpawn)
        {
            _sceneToLoad = sn;
            StartCoroutine(ProcessSceneTransition(spawnPoint, playerDirAfterSpawn));
        }

        public IEnumerator ProcessSceneTransition(Vector3 spawnPoint, CharacterController._directions playerDirAfterSpawn)
        {
            _anim.SetTrigger("end");
            yield return new WaitForSeconds(_transitionTime);
            SceneManager.LoadScene(_sceneToLoad.ToString());
            var manabu = GameManager._instance._mainCharacter;
            manabu.transform.position = spawnPoint;
            manabu.GetComponent<CharacterController>().SetCurrentDirection(playerDirAfterSpawn);

        }

    }

}

