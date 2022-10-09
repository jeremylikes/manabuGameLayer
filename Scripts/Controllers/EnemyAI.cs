using Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class EnemyAI : MonoBehaviour
    {
        private Character _thisCharacter;
        public Vector3 _targetPos = Vector3.zero;
        private float _restTime;
        public bool _attackFinished = true;
        [SerializeField] private BoxCollider2D _patrolBounds;
        //Patrol bounding
        public Vector2 _min;
        public Vector2 _max;

        //Aggro
        private Character _aggroTarget;
        [SerializeField] private float _attackRange = 0f;
        public bool _attacking;
        [SerializeField] private float _attackCooldown = 3f;
        /*[SerializeField]*/ private float _attackTimer = 3f;
        private _directions _currentDirection;
        [SerializeField] private bool _fakeCollisionDamage;
        //States
        public enum _states
        {
            Patrol, Aggro, Stun, Rest, Dead
        };

        public _states _currentState;
        public Vector3 _startingPos;

        public enum _directions
        {
            Up, Down, Left, Right
        }

        private void Awake()
        {
            SetPatrolBounds();
            SetNextPatrolSpot();
            SetRestTime();
            _thisCharacter = GetComponent<Character>();
            _currentState = _states.Patrol;
            _startingPos = transform.position;
            _attackTimer = _attackCooldown;
            if (_attackRange == 0f)
                _attackRange = 2f;
        }

        private void Update()
        {
            if (_currentState != _states.Stun)
            {
                if (_currentState != _states.Aggro)
                    SurveyTargets();
                if (_currentState == _states.Patrol)
                    Move();
                if (_currentState == _states.Rest)
                    Rest();
                if (_currentState == _states.Aggro)
                    Aggro();
            }
        }

        public IEnumerator StunEnemyForSeconds(float seconds = 2f)
        {
            ToggleMovement(false);
            var stateBeforeStun = _currentState;
            _currentState = _states.Stun;
            yield return new WaitForSeconds(seconds);
            _currentState = stateBeforeStun;
            ToggleMovement(true);
        }

        private void Move()
        {
            if (!_thisCharacter._isMoving)
                ToggleMovement(true);
            ManageDirection();
            transform.position = Vector2.MoveTowards(transform.position, _targetPos, _thisCharacter.GetCharacterStat(CharacterStats.Speed)._current *
                Time.deltaTime);
            //var rb = GetComponent<Rigidbody2D>();
            //rb.MovePosition(rb.position + (Vector2)GetCurrentDirectionVector() * _thisCharacter.GetCharacterStat(CharacterStats.Speed)._current * Time.fixedDeltaTime);

            if (Vector2.Distance(transform.position, _targetPos) < 0.02f)
            {
                _currentState = _states.Rest;
            }
        }

        private void Rest()
        {
            if (_thisCharacter._isMoving)
                ToggleMovement(false);
            if (_restTime <= 0f)
            {
                SetNextPatrolSpot();
                SetRestTime();
                _currentState = _states.Patrol;
            }
            else
            {
                _restTime -= Time.deltaTime;
            }
        }

        void ToggleMovement(bool value)
        {
            _thisCharacter._isMoving = value;
            GetComponent<Animator>().SetBool("moving", value);
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (collision.tag == "Player")
        //    {
        //        var manabu = collision.gameObject.GetComponent<Manabu>();
        //        manabu.TakeDamage(_thisCharacter.transform, 4);
        //    }
        //}

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var manabu = collision.gameObject.GetComponent<Manabu>() ?? null;

            if (manabu != null)
            {
                manabu.TakeDamage(_thisCharacter.transform, 4, false, _fakeCollisionDamage);
            }
        }

        private void Aggro()
        {
            _targetPos = _aggroTarget.transform.position;
            if (_thisCharacter.HasSpecials())
                _thisCharacter.InitSpecialAttack();
            float delta = Vector2.Distance(transform.position, _targetPos);
            if (delta > _attackRange && !_attacking)
            {
                Move();
            }

            if (delta <= _attackRange)
            {
                if (_thisCharacter._isMoving)
                    ToggleMovement(false);
                ManageDirection();
                if (_attackTimer <= 0f)
                {
                    _attackFinished = false;
                    _thisCharacter.Attack(_aggroTarget);
                    _attackTimer = _attackCooldown;
                }
                else
                {
                    if (_attackFinished)
                        _attackTimer -= Time.deltaTime;
                }

            }

            if (delta > _thisCharacter.Stats[CharacterStats.Sight]._current + 1f && !_attacking)
            {
                _targetPos = _startingPos;
                _aggroTarget = null;
                _currentState = _states.Patrol;
            }
        }

        void SurveyTargets()
        {
            Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position,
                _thisCharacter.Stats[CharacterStats.Sight]._current);
            foreach (Collider2D c in possibleTargets)
            {
                if (c.GetComponent<Manabu>() != null && c.GetComponent<Manabu>().Stats[CharacterStats.HP]._current > 0)
                {
                    SizeupThreat(c.GetComponent<Manabu>());
                }
            }
        }

        void SizeupThreat(Manabu target)
        {
            // Maybe add some kind of threshold here where the enemy won't just aggro blindly
            if (_currentState != _states.Aggro)
                _thisCharacter.PlayAggroSound();
            _targetPos = target.transform.position;
            _aggroTarget = target;
            _currentState = _states.Aggro;
        }

        private void SetRestTime() => _restTime = UnityEngine.Random.Range(2f, 7f);

        private void SetNextPatrolSpot() => _targetPos = new Vector2(UnityEngine.Random.Range(_min.x, _max.x), 
            UnityEngine.Random.Range(_min.y, _max.y));

        private void SetPatrolBounds()
        {
            if (_patrolBounds != null)
            {
                var size = _patrolBounds.size;
                var center = _patrolBounds.bounds.center;
                float halfHeight = size.y / 2f;
                float halfWidth = size.x / 2f;
                _max.x = center.x + halfWidth;
                _min.x = center.x - halfWidth;
                _max.y = center.y + halfHeight;
                _min.y = center.y - halfHeight;

                Destroy(_patrolBounds.gameObject);
            }

        }

        public void FaceCharacter(_directions direction)
        {
            var anim = GetComponent<Animator>();
            switch(direction)
            {
                case _directions.Up:
                    anim.SetFloat("xVect", 0f);
                    anim.SetFloat("yVect", 1f);
                    break;
                case _directions.Down:
                    anim.SetFloat("xVect", 0f);
                    anim.SetFloat("yVect", -1f);
                    break;
                case _directions.Left:
                    anim.SetFloat("yVect", 0f);
                    anim.SetFloat("xVect", -1f);
                    break;
                case _directions.Right:
                    anim.SetFloat("yVect", 0f);
                    anim.SetFloat("xVect", 1f);
                    break;
                default:
                    break;

            }
        }

        public Vector3 GetCurrentDirectionVector()
        {
            switch (_currentDirection)
            {
                case _directions.Up:
                    return Vector3.up;
                case _directions.Down:
                    return Vector3.down;
                case _directions.Left:
                    return Vector3.left;
                case _directions.Right:
                    return Vector3.right;
                default:
                    return Vector3.zero;
            }
        }

        private void ManageDirection()
        {
            // StartCoroutine(ApplyDirectionLock());
            // let's compare the position of the target to the position of the enemy
            Vector3 deltaPos = transform.position - _targetPos;
            float deltaX = Math.Abs(deltaPos.x);
            float deltaY = Math.Abs(deltaPos.y);

            // if diff of x is greater then we need to point the character left or right
            if (deltaX > deltaY)
            {
                if (_targetPos.x > transform.position.x)
                {
                    FaceCharacter(_directions.Right);
                    _currentDirection = _directions.Right;
                }
                else
                {
                    FaceCharacter(_directions.Left);
                    _currentDirection = _directions.Left;
                }
            }
            // if the diff of y is greater than we need to point the character up or down
            else if (deltaY > deltaX)
            {
                if (_targetPos.y > transform.position.y)
                {
                    FaceCharacter(_directions.Up);
                    _currentDirection = _directions.Up;
                }
                else
                {
                    FaceCharacter(_directions.Down);
                    _currentDirection = _directions.Down;
                }
            }

        }

    }
}

