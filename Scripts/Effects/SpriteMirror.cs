using Characters;
using Interactables;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Effects
{
    public class SpriteMirror : MonoBehaviour, ITimeInteractable
    {
        [SerializeField] private bool _reflectionActive;
        [SerializeField] private Animator _reflectionAnim;
        [SerializeField] private Animator _targetAnim;
        [SerializeField] private Vector2 _movementVect;
        [SerializeField] private bool _freezeReflection;
        //[SerializeField] private SpriteRenderer _reflectionSprite;
        //[SerializeField] private SpriteRenderer _manabuSprite;
        private void OnValidate()
        {
            _reflectionAnim = transform.Find("Reflection").GetComponent<Animator>();
            _movementVect = Vector2.zero;
        }

        private void Update()
        {

            if (_reflectionActive && !_freezeReflection)
            {
                UpdateReflectionSprite();
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Manabu>() != null && !_freezeReflection)
            {
                if (GetComponent<PuzzleCondition>() != null)
                    GetComponent<PuzzleCondition>()._conditionIsMet = true;
                _reflectionActive = true;
                _targetAnim = collision.GetComponent<Animator>();
                _reflectionAnim.runtimeAnimatorController = collision.GetComponent<Animator>().runtimeAnimatorController;
                _reflectionAnim.transform.position = new Vector3(collision.transform.position.x, _reflectionAnim.transform.position.y, _reflectionAnim.transform.position.z);
                if (GetComponent<PuzzleCondition>() != null)
                    GetComponent<PuzzleCondition>()._conditionIsMet = true;
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //_reflectionActive = false;
            //_reflectionSprite.sprite = null;
            if (collision.GetComponent<Manabu>() != null)
            {
                if (!_freezeReflection)
                {
                    _targetAnim = null;
                    _reflectionActive = false;
                    _reflectionAnim.runtimeAnimatorController = null;
                    _movementVect = Vector2.zero;
                    if (GetComponent<PuzzleCondition>() != null)
                        GetComponent<PuzzleCondition>()._conditionIsMet = false;
                }
            }


        }

        public IEnumerator FreezeReflection()
        {
            _freezeReflection = true;
            yield return new WaitForSeconds(5f);
            CancelTimeStopEffect();
        }

        private void UpdateReflectionSprite()
        {

            //_reflectionSprite.sprite = _manabuSprite.sprite;
            //_reflectionSprite.transform.position = _manabuSprite.gameObject.transform.position;
            if (_targetAnim.GetComponent<Managers.CharacterController>() != null && _reflectionAnim != null)
            {
                _reflectionAnim.transform.position =
                    new Vector3(_targetAnim.transform.position.x, _reflectionAnim.transform.position.y, _reflectionAnim.transform.position.z);
                var cc = _targetAnim.GetComponent<Managers.CharacterController>();
                _reflectionAnim.SetBool("moving", cc._thisCharacter._isMoving);
                if (cc._thisCharacter._isMoving)
                {
                    var currDir = cc._currentDirection;
                    //ManageSpriteInverstion(currDir);
                    switch (currDir)
                    {
                        case Managers.CharacterController._directions.Up:
                            //_reflectionAnim.GetComponent<SpriteRenderer>().flipX = true;
                            _movementVect = Vector2.down;
                            break;
                        case Managers.CharacterController._directions.Down:
                            //_reflectionAnim.GetComponent<SpriteRenderer>().flipX = true;
                            _movementVect = Vector2.up;
                            break;
                        case Managers.CharacterController._directions.Left:
                            _movementVect = Vector2.left;
                            break;
                        case Managers.CharacterController._directions.Right:
                            _movementVect = Vector2.right;
                            break;
                    }
                    _reflectionAnim.SetFloat("xPos", _movementVect.x);
                    _reflectionAnim.SetFloat("yPos", _movementVect.y);
                    var xPos = _reflectionAnim.GetFloat("xPos");
                    var yPos = _reflectionAnim.GetFloat("yPos");
                }

            }

        }

        private void ManageSpriteInverstion(Managers.CharacterController._directions currDir)
        {
            var sr = _reflectionAnim.GetComponent<SpriteRenderer>();
            if (currDir == Managers.CharacterController._directions.Up || currDir == Managers.CharacterController._directions.Down)
            {
                sr.flipX = true;
                // TODO: Let's manage the yPos at some point if we want to
                //sr.transform.position = new Vector3(sr.transform.position.x, _targetAnim.bodyPosition.y - sr.transform.position.y, sr.transform.position.z);
            }
            else
                sr.flipX = false;
        }

        public void ExcecuteTimeStopEffect()
        {
            if (_reflectionActive)
                StartCoroutine(FreezeReflection());
        }

        public void CancelTimeStopEffect()
        {
            _freezeReflection = false;
        }
    }
}

