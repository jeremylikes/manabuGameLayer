using Controllers;
using Effects;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ScriptedEvents
{
    public class DemoClose : MonoBehaviour
    {
        [SerializeField] private Sprite _stepBackSprite;
        [SerializeField] private GameObject _manabuTitle;
        [SerializeField] private AudioClip _titleRevealSound;
        [SerializeField] private Transform _statPanel;

        void Start()
        {
            StartCoroutine(InitDemoClose());
        }

        private IEnumerator InitDemoClose()
        {
            _manabuTitle.transform.position = transform.position;
            var mainCam = Camera.main;
            mainCam.gameObject.GetComponent<CameraMovement>().enabled = false;
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, mainCam.transform.position.z);
            while (Vector3.Distance(mainCam.transform.position, targetPos) > 0.001f)
            {
                mainCam.transform.position = Vector3.MoveTowards(mainCam.transform.position, targetPos, Time.deltaTime);
                yield return null;
            }
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<SpriteFade>().enabled = true;
            yield return new WaitForSeconds(4f);
            GetComponent<SpriteRenderer>().sprite = _stepBackSprite;
            yield return new WaitForSeconds(1f);
            _statPanel.gameObject.SetActive(false);
            _manabuTitle.gameObject.SetActive(true);
            AudioManager._instance.PlaySong(_titleRevealSound, false);
            yield return new WaitForSeconds(4f);
            _manabuTitle.GetComponent<SpriteFade>().enabled = true;
            yield return new WaitForSeconds(4f);
            GameStateManager._instance.ReturnToTile();
        }
    }

}
