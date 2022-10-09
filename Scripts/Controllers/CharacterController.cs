using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Characters;
using Items;
using Managers;
using UI;
using KanjiUnity;
using Interactables;
using System;
using Controllers;
using PhysicalObjects;
using Spells;


namespace Managers
{
    public class CharacterController : MonoBehaviour
    {
        protected bool _overrideControls = false;
        public Animator _anim;
        protected bool _npcMovementStarted = false;
        protected float _moveTime;
        protected float _waitTime;
        protected bool _animationSet;
        //public float _walkSpeedFactor = 0.004f;
        protected bool _keyWasPressed = false;
        public bool _isCasting = false;
        public bool _disableMovement = false;
        public bool _castingOverride;
        public Manabu _thisCharacter;
        public GameObject _moneyPanel;
        public StatPanelManager _statPanelManager;
        protected IEnumerator _currentCoroutine;
        public Vector3 _startingPosition;
        public bool _enableDebugControls;
        public bool _stunModeActive;
        public bool _attackDelay;
        public GameObject _rangedDax;
        public GameObject _animController;
        public GameObject _kanjiPanel;
        private Rigidbody2D _rb;
        public Coroutine _footstepsCoroutine;
        private Coroutine _attackDelayCoroutine;

        private int _dashInputs = 0; // if this reaches 2 then we dash
        private KeyCode _lastKeyPressed;



        public enum _rayTypes
        {
            Interact,
            Dash,
            Attack
        }

        public enum _directions
        {
            Up,
            Down,
            Left,
            Right
        }

        public enum _states
        {
            Stun,
            Idle,
            Move,
            Battle,
            Falling,
            Dead
        }


        public _directions _currentDirection;
        private Vector2 _lastMovement = new Vector2(0f, 0f);
        public _states _currentState;
        private bool _canDash = true;

        private void OnValidate()
        {
            _thisCharacter = GetComponent<Manabu>();

        }

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _startingPosition = transform.position;
            //_anim = transform.Find("mainSprite").GetComponent<Animator>();
            _currentState = _states.Idle;
            InitAnimDirection();
        }

        public void InterruptAttackDelayRoutine()
        {
            if (_attackDelayCoroutine != null)
            {
                StopCoroutine(_attackDelayCoroutine);
                _anim.SetBool("strike", false);
                _anim.SetBool("moving", false);
                _attackDelay = false;
                if (_thisCharacter._movementDisabled)
                    ControlsManager._instance.SetDisabledMovementControls();
                else
                    ControlsManager._instance.SetActiveControls();
            }

        }

        public void PerformDaxGrapple()
        {
            var hook = _thisCharacter.GetDaxGrappleHook();
            if (hook != null)
            {
                var inst = Instantiate(Resources.Load<GrappleHookPhysical>(hook.GetPathToPrefab()), _thisCharacter.transform.position, Quaternion.identity);
                //inst.transform.position = _thisCharacter.transform.position;
                inst.Deploy();
            }
        }

        void InitAnimDirection()
        {
            if (_currentDirection == _directions.Up || _currentDirection == _directions.Down)
            {
                _anim.SetFloat("yPos", _currentDirection == _directions.Up ? 1f : -1f);
                _anim.SetFloat("xPos", 0f);
            }
            else
            {
                _anim.SetFloat("xPos", _currentDirection == _directions.Left ? -1f : 1f);
                _anim.SetFloat("yPos", 0f);
            }

        }

        void Update()
        {
            if (_currentState != _states.Dead &&
                _currentState != _states.Falling &&
                _isCasting == false)
                _disableMovement = false;

        }

        private void CastCircle(_rayTypes type)
        {
            float thickness = 0.2f;
            var origin = transform.position;
            var direction = ConvertCurrentDirectionToVector3();
            var hits = Physics2D.CircleCastAll(origin, thickness, direction);
            foreach (var hit in hits)
            {
                var enemy = hit.collider.gameObject.GetComponent<EnemyAI>() ?? null;
                if (enemy != null)
                {
                    //Let's not stun multiple times if the enemy is already stunned, mmkay?
                    if (type == _rayTypes.Dash && enemy._currentState != EnemyAI._states.Stun)
                    {
                        StartCoroutine(enemy.StunEnemyForSeconds(5f));
                    }
                }
            }

        }

        private Vector3 ConvertCurrentDirectionToVector3()
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

        private void CastRay(_rayTypes type)
        {
            var weapon = _thisCharacter.GetEquippedWeapon();
            float range = 0f;
            switch (type)
            {
                case _rayTypes.Attack:
                    range = weapon.GetRange();
                    break;
                case _rayTypes.Interact:
                    range = 0.3f;
                    break;
                case _rayTypes.Dash:
                    range = 0.5f;
                    break;
                default:
                    break;
            }

            Vector2 direction = ConvertCurrentDirectionToVector3();

            //RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, range);
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.1f, direction, range);
            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    // let's not hit ourselves
                    if (hit.collider.GetComponent<Manabu>() != null)
                        continue;
                    if (type == _rayTypes.Attack)
                    {
                        if (hit.collider.GetComponent<Character>() != null)
                            _thisCharacter.Attack(hit.collider.GetComponent<Character>());
                        if (hit.collider.tag == "immovable")
                            StartCoroutine(StrikeImmovableObject());
                        if (hit.collider.GetComponent<ISpellAffectedObject>() != null)
                        {
                            Spell enchanment = _thisCharacter.GetActiveEnchanment() ?? null;
                            if (enchanment != null)
                            {
                                var obj = hit.collider.GetComponent<ISpellAffectedObject>();
                                if (enchanment.SpellName == obj.GetSpellAffectedBy())
                                    obj.ReactToSpell();
                            }
                        }
                        if (hit.collider.GetComponent<Character>() == null && hit.collider.GetComponent<IAttackInteractable>() != null)
                        {
                            hit.collider.GetComponent<IAttackInteractable>().Interact();
                        }
                        if (hit.collider.GetComponent<IDestructable>() != null)
                        {
                            hit.collider.GetComponent<IDestructable>().TakeDamage((int)_thisCharacter.GetCharacterStat(CharacterStats.Strength)._current);
                        }

                    }
                    else if (type == _rayTypes.Dash)
                    {
                        var character = hit.collider.GetComponent<Character>() ?? null;
                        if (character != null)
                        {

                            if (character.GetCharacterType() == CharacterTypes.Enemy)
                            {
                                StartCoroutine(character.GetComponent<EnemyAI>().StunEnemyForSeconds(5f));
                            }
                        }
                    }
                    else if (type == _rayTypes.Interact)
                    {
                        var interactableObj = hit.collider.GetComponent<IInteractable>();
                        if (interactableObj != null)
                        {
                            interactableObj.Interact();
                            break; // we just want to trigger the interaction with the first object it finds
                        }
                    }
                }
            }

        }

        #region Actions
        public void MoveCharacter()
        {
            if (!_thisCharacter._isMoving)
            {
                _thisCharacter._isMoving = true;
                _footstepsCoroutine = StartCoroutine(_thisCharacter.ProcessFootstepSE());
                _thisCharacter.CreateDust();
            }
            Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            movement.Normalize();
            if (_lastMovement != movement)
            {
                SetCurrentDirection(movement);
                _lastMovement = movement;
            }
            _rb.MovePosition(_rb.position + movement * _thisCharacter.GetCharacterStat(CharacterStats.Speed)._current * Time.fixedDeltaTime);
            if (_anim.GetBool("moving") == false)
                _anim.SetBool("moving", true);

        }

        #region Dash
        public void CheckForDash(KeyCode key)
        {
            if (_canDash)
            {
                _dashInputs++;
                StartCoroutine(MonitorForSuccessfulDash());
                if (key == _lastKeyPressed && _dashInputs >= 2)
                {
                    StartCoroutine(Dash());
                    _dashInputs = 0;
                }
                _lastKeyPressed = key;
            }

        }

        private IEnumerator MonitorForSuccessfulDash()
        {
            float timer = 0.2f;
            yield return new WaitForSeconds(timer);
            _dashInputs = 0;
        }

        private IEnumerator InitDashRechargeTimer()
        {
            _canDash = false;
            float timer = 20f;
            yield return new WaitForSeconds(timer);
            _canDash = true;
        }

        public IEnumerator Dash()
        {
            StartCoroutine(InitDashRechargeTimer());
            _anim.SetTrigger("dash");
            CastCircle(_rayTypes.Dash);

            ControlsManager._instance.SetLockedControls();
            float speed = 2f;
            float duration = 0.5f;
            var dir = ConvertCurrentDirectionToVector3();
            var rb = GetComponent<Rigidbody2D>();

            //_thisCharacter.GetComponent<Rigidbody2D>().MovePosition(rb.transform.position + dir * 0.5f);
            rb.AddForce(dir * 10f, ForceMode2D.Impulse);
            //GetComponent<Rigidbody2D>().velocity = dir * 3f;

            yield return new WaitForSeconds(duration);
            ControlsManager._instance.SetActiveControls();
        }
        #endregion

        public void SetCurrentDirection(Vector2 movement)
        {
            if (movement.x != 0)
                _currentDirection = movement.x > 0 ? _directions.Right : _directions.Left;
            else
                _currentDirection = movement.y > 0 ? _directions.Up : _directions.Down;
            InitAnimDirection();
        }

        public void SetCurrentDirection(_directions direction)
        {
            _currentDirection = direction;
            InitAnimDirection();
        }

        public void StopMovement()
        {

            if (_footstepsCoroutine != null)
            {
                StopCoroutine(_footstepsCoroutine);
                _footstepsCoroutine = null;
            }

            if (_thisCharacter._isMoving)
                _thisCharacter._isMoving = false;
            _anim.SetBool("moving", false);
            _lastMovement = new Vector2(0f, 0f);
        }

        public void PerformRangedAttack()
        {
            Attack();
            if (_currentState == _states.Battle)
            {

            }

        }

        public void Attack()
        {
            if (_thisCharacter.GetEquippedWeapon() != null)
            {
                if (!_attackDelay /*&& _thisCharacter.GetCharacterStat(CharacterStats.Stamina)._current >=
                    requiredStamina*/)
                {
                    AudioManager._instance.PlaySoundEffect(_thisCharacter._attackSound[UnityEngine.Random.Range(0,
                        _thisCharacter._attackSound.Count)]);
                    CastRay(_rayTypes.Attack);
                    //_thisCharacter.AdjustStat(CharacterStats.Stamina, -requiredStamina);
                    _anim.SetTrigger("strike");
                    _attackDelayCoroutine = StartCoroutine(ProcessAttackDelay());
                }
            }
        }

        public void CastSentinelCube()
        {
            //AudioManager._instance.PlaySoundEffect(
            //    (AudioClip)Resources.Load(@"Audio\se\Battle\se_sentinel_cube")
            //    );
            //GameObject sentinelCube = (GameObject)Resources.Load(@"Prefabs\SentinelCube");
            //Instantiate(sentinelCube, _thisCharacter.transform.position,
            //_thisCharacter.transform.rotation);
        }

        public void Cast()
        {
            if (!_thisCharacter._castEnabled)
                return;
            if (!_isCasting)
            {
                _isCasting = true;
                _anim.SetBool("cast", _isCasting);
                GameManager._instance._canvasManager.ToggleKanjiPanel();
                //Time.timeScale *= 0.3f;

                ControlsManager._instance.SetCastControls();
                if (_currentCoroutine != null)
                    StopCoroutine(_currentCoroutine);
                _currentCoroutine = AudioManager._instance.FadeBGM(0.30f, 3f);
                StartCoroutine(_currentCoroutine);
                AudioManager._instance.PlaySoundEffect(Resources.Load<AudioClip>(@"Audio/SE/Spells/castStart"));
                AudioManager._instance.PlayLoopingSoundEffect(Resources.Load<AudioClip>(@"Audio/SE/Spells/cast_rumble"));
            }
            else
            {
                CancelCast();
            }
        }

        public void CancelCast()
        {
            if (_isCasting)
            {
                _isCasting = false;
                _anim.SetBool("cast", _isCasting);
                GameObject.Find("KanjiPanel").GetComponent<DrawKanji>().Reset();
                GameManager._instance._canvasManager.ToggleKanjiPanel();
                if (_currentCoroutine != null)
                    StopCoroutine(_currentCoroutine);
                _currentCoroutine = AudioManager._instance.FadeBGM(1f, 3f);
                StartCoroutine(_currentCoroutine);
                if (_disableMovement)
                    _disableMovement = false;
                AudioManager._instance.PlaySoundEffect(Resources.Load<AudioClip>(@"Audio/SE/Spells/castCancel"));
                AudioManager._instance.StopLoopingSoundEffectTrack();
                if (GameManager._instance._mainCharacter._movementDisabled)
                {
                    ControlsManager._instance.SetDisabledMovementControls();
                    return;
                }

                else
                    ControlsManager._instance.SetActiveControls();
            }

        }

        public void EnterEverpresence()
        {
            ////Hmmm let's not load in a completely new scene
            ////SceneManager.LoadScene("TheChamber", LoadSceneMode.Additive);
            //GameManager._instance._lastScene = SceneManager.GetActiveScene().name;
            //SceneManager.LoadScene("TheChamber");
        }

        public void ExitEverpresence()
        {
            ////Obivously, we want to be able to exit back into state we were in
            //SceneManager.LoadScene(GameManager._instance._lastScene);
        }

        public void CommitKanji()
        {
            var kanjiCanvas = GameObject.Find("KanjiPanel");
            if (kanjiCanvas.GetComponent<DrawKanji>())
            {
                kanjiCanvas.GetComponent<DrawKanji>().Commit();
                Cast(); //toggles the casting effect
            }

        }

        public void LearnKanji()
        {
            var kanjiCanvas = GameObject.Find("KanjiPanel");
            if (kanjiCanvas.GetComponent<DrawKanji>())
            {
                kanjiCanvas.GetComponent<DrawKanji>().Commit();
            }
        }

        public void CastDebugFireEnchantment()
        {
            //List<Weapon> weapons = _thisCharacter.GetEquippedWeapons();
            //if (weapons[0].GetProperties().Contains(Game.ItemProperties.TwoHandedWeapon) &&
            //    weapons[0]._enchantment == null)
            //{
            //    Spell fireball = new Fireball();
            //    AudioManager._instance.PlaySoundEffect(fireball.GetEnchantmentSE());
            //    _thisCharacter.GetEquippedWeapons()[0].AddEnchantment(fireball);
            //    StartCoroutine(fireball.SetEnchantment(_thisCharacter));
            //}
        }

        public void CancelEnchantment()
        {
            //if (_thisCharacter._enchantmentIsActive)
            //{
            //    AudioManager._instance.PlaySoundEffect((AudioClip)
            //    Resources.Load(Game.GlobalValues.PATH_TO_SPELL_SOUND_EFFECTS + "drop_enchantment"));
            //    _thisCharacter.RemoveEnchantmentFromWeapons();

            //}

        }

        #endregion Actions

        public IEnumerator ProcessAttackDelay()
        {
            _attackDelay = true;
            var previousControlSettings = ControlsManager._instance.GetCurrentControlSchema();
            ControlsManager._instance.SetLockedControls();
            _anim.SetBool("strike", true);
            ////Getting the animation clip length felt too slow
            ////float test = _anim.GetCurrentAnimatorClipInfo(0).Length;
            ////yield return new WaitForSeconds(test);
            ////Getting a value based on weapon weight felt too slow
            yield return new WaitForSeconds(Calculators.GlobalCalculator.GetAttackDelay(_thisCharacter));
            //yield return new WaitForSeconds(calc.GetAttackDelay(_thisCharacter.GetEquippedWeapons()));
            _attackDelay = false;
            _anim.SetBool("strike", false);
            ControlsManager._instance.SetControls(previousControlSettings);
            //if (ControlsManager._instance.GetCurrentControlSchema() != ControlsManager.ControlSchema.DisableMovement)
            //    ControlsManager._instance.SetActiveControls();
            //else
            //    ControlsManager._instance.SetDisabledMovementControls();
        }

        void MonitorStunTargets()
        {
            //if (_stunModeActive)
            //{

            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        Debug.Log("Left mouse button clicked");
            //        RaycastHit hitInfo = new RaycastHit();
            //        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(
            //            Input.mousePosition), out hitInfo);
            //        if (hit)
            //        {
            //            Debug.Log("Hit the " + hitInfo.transform.gameObject.name);
            //            if (hitInfo.transform.gameObject.GetComponent<Enemy>() != null)
            //            {
            //                Character target = hitInfo.transform.gameObject.GetComponent<Character>();
            //                Debug.Log("worked");
            //                StartCoroutine(_thisCharacter.BindTarget(target));
            //            }
            //        }
            //    }
            //}
        }

        void ChangeState(_states targetState)
        {
            _currentState = targetState;
        }

        //there are cases when the calling object gets disabled and therefore we need to kickoff the coroutine from here
        //public void KickoffStunCoroutine()
        //{
        //    StartCoroutine(ApplyStun());
        //}

        public void ScanInteractableObjects()
        {
            CastRay(_rayTypes.Interact);
        }

        //public IEnumerator ApplyStun(float time = 5f)
        //{
        //    Vector3 origPos = transform.position;
        //    while (time > 0f)
        //    {
        //        time -= Time.deltaTime;
        //        transform.position = origPos;
        //        yield return null;
        //    }
        //}

        public IEnumerator StrikeImmovableObject()
        {
            _anim.SetBool("strike", true);
            AudioManager._instance.PlaySoundEffect((AudioClip)Resources.Load($"Audio/SE/Battle/se_clank"));
            float time = 0.1f;
            while (time > 0f)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            Debug.Log("Reset");
            _anim.SetBool("strike", false);
            _attackDelay = false;
            ControlsManager._instance.SetActiveControls();
        }

    }
}
