using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;

using System.Linq;

namespace ScriptedEvents
{
    public class DemonDog : MonoBehaviour
    {
        bool _isFollowing = false;
        Manabu _manabu;
        Managers.CharacterController _manabuController;
        Animator _anim;
        private float _movementSpeed = 1f;
        [SerializeField] private AudioClip _teleportScareSE;

        private void OnValidate()
        {
            _manabu = FindObjectOfType<Manabu>();
            _manabuController = _manabu.GetComponent<Managers.CharacterController>();
            _anim = GetComponent<Animator>();
            _anim.SetFloat("yPos", -1);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var manabu = collision.GetComponent<Manabu>();
            if (manabu && !_isFollowing)
            {
                StartCoroutine(ProcessTeleport());

            }
        }

        private IEnumerator ProcessTeleport()
        {
            AudioManager._instance.PlaySoundEffect(_teleportScareSE);
            var newPos = new Vector2(_manabu.transform.position.x, _manabu.transform.position.y - 0.5f);
            var newRot = new Quaternion(transform.rotation.x, transform.rotation.y, 0f, 0f);
            yield return new WaitForSeconds(1f);
            transform.position = newPos;
            transform.rotation = newRot;
            _isFollowing = true;
            _anim.SetFloat("yPos", 1);
            // get rid of the glowing eyes since he's facing up
            GetComponentsInChildren<UnityEngine.Rendering.Universal.Light2D>().ToList().ForEach(l => l.enabled = false);
        }

        private void Update()
        {
            if (_manabu.transform.position.y > transform.position.y && _isFollowing && _manabu._isMoving && _manabuController._currentDirection == Managers.CharacterController._directions.Up)
                WalkUp();
            else
                StopWalking();
        }

        private void WalkUp()
        {
            _anim.SetBool("Walking", true);
            transform.position += new Vector3(0f, _movementSpeed * Time.deltaTime, 0f);
        }

        private void StopWalking()
        {
            _anim.SetBool("Walking", false);

        }
    }

}
