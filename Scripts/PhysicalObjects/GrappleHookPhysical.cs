using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalObjects
{
    public class GrappleHookPhysical : MonoBehaviour
    {
        [SerializeField] private AudioClip _rappelSE;
        [SerializeField] private AudioClip _deploySE;
        [SerializeField] private AudioClip _connectSE;
        [SerializeField] private float _extensionRange;

        private int _collisionIndex = 0;
        public Coroutine _currentCoroutine;
        private Vector3 _offsetVector;

        public void Deploy()
        {
            var manabu = GameManager._instance._mainCharacter;
            AudioManager._instance.PlaySoundEffect(_deploySE);
            ControlsManager._instance.SetLockedControls();
            // AnimaSHON
            Managers.CharacterController controller = GameManager._instance._characterController;
            controller._anim.SetBool("strike", true);

            var currDirection = controller._currentDirection;
            _offsetVector = Vector3.zero;
            Vector2 direction = Vector2.zero;
            float spriteSizeX = manabu.GetComponent<SpriteRenderer>().bounds.size.x / 2f;
            float spriteSizeY = manabu.GetComponent<SpriteRenderer>().bounds.size.y / 2f;
            switch (currDirection)
            {
                case Managers.CharacterController._directions.Up:
                    direction = Vector2.up;
                    _offsetVector = new Vector3(0f, spriteSizeY, 0f);
                    break;
                case Managers.CharacterController._directions.Down:
                    direction = Vector2.down;
                    _offsetVector = new Vector3(0, -spriteSizeY, 0f);
                    break;
                case Managers.CharacterController._directions.Left:
                    direction = Vector2.left;
                    _offsetVector = new Vector3(-spriteSizeX, 0f, 0f);
                    break;
                case Managers.CharacterController._directions.Right:
                    direction = Vector2.right;
                    _offsetVector = new Vector3(spriteSizeX, 0f, 0f);
                    break;
            }
            // Make the grapple shoot in curr direction
           _currentCoroutine = StartCoroutine(ProcessGrappleShot(direction));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "grapple")
            {
                // We don't want to grapple to the platform we're standing on
                _collisionIndex++;
                if (_collisionIndex > 1)
                {
                    Vector3 destinationPoint = collision.transform.position + _offsetVector;
                    StopCoroutine(_currentCoroutine);
                    _currentCoroutine = StartCoroutine(GrappleToDestination(destinationPoint));
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Worked");

        }

        private IEnumerator ProcessGrappleShot(Vector2 direction)
        {
            var vect = Vector3.zero;
            float spriteOffset = _extensionRange - GetComponent<SpriteRenderer>().bounds.size.x;
            if (direction.x != 0)
                vect.x = direction.x > 0 ? spriteOffset : -spriteOffset;
            if (direction.y != 0)
                vect.y = direction.y > 0 ? spriteOffset : -spriteOffset;
            var destination = transform.position + vect;
            while (transform.position != destination)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * 2f);
                yield return null;
            }

            Reset();
        }


        private IEnumerator GrappleToDestination(Vector3 destination)
        {
            //ToggleManabuColliders(false);
            transform.position = destination;
            GetComponent<Animator>().SetTrigger("connect");
            ControlsManager._instance.SetLockedControls();

            // Audio Matters
            yield return new WaitForSeconds(_deploySE.length / 2f);
            AudioManager._instance.PlaySoundEffect(_connectSE);
            yield return new WaitForSeconds(_connectSE.length);
            AudioManager._instance.PlaySoundEffect(_rappelSE);

            var manabu = GameManager._instance._mainCharacter;
            float increment = 0.1f;
            while (manabu.transform.position != destination)
            {
                manabu.transform.position = Vector3.MoveTowards(manabu.transform.position, destination, increment);
                //Vector3.Lerp(manabu.transform.position, destination, increment);
                yield return null;
            }
            Reset();

        }

        private void Reset()
        {
            _currentCoroutine = null;
            GameManager._instance._characterController._anim.SetBool("strike", false);
            ControlsManager._instance.SetActiveControls();
            Destroy(gameObject);
        }

    }

}
