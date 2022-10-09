using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Data.SqlTypes;
using UI;

namespace Managers
{
    public class ControlsManager : MonoBehaviour
    {
        public static ControlsManager _instance = null;
        public CharacterController _playerOneController;
        public KanjiDemo _kanjiDemo;
        public Menu _currentMenu;
        [SerializeField] private Menu _mainMenu;

        Dictionary<KeyCode, System.Action> _keyMaps = new Dictionary<KeyCode, System.Action>();
        public KeyCode[] _movementKeys = 
        {
            KeyCode.A,
            KeyCode.S,
            KeyCode.W,
            KeyCode.D
        };
        public enum ControlSchema
        {
            Cutscene,
            SystemMessage,
            Active,
            Pause,
            Cast,
            Locked,
            DisableMovement,
            Everpresence,
            Dialogue,
            Menu,
            GameOver,
            Bond,
            KanjiLearned,
            KanjiDemo,
            NotSet
        }
        public ControlSchema _currentControlSchema;
        public bool _allowDebugControls;
        private List<KeyCode> _keys = new List<KeyCode>();
        private bool _buildKeyList;

        public ControlSchema GetCurrentControlSchema() => _currentControlSchema;
        private void Awake()
        {
            _buildKeyList = true;
            _currentControlSchema = ControlSchema.NotSet;
            if (_instance == null)
                _instance = this;
            //else if (_instance != this)
            //    Destroy(gameObject);
            //DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (_currentControlSchema == ControlSchema.NotSet)
                SetActiveControls();
        }

        // Update is called once per frame
        void Update()
        {
            ManageControls();
        }

        void ManageControls()
        {
            if (_keyMaps != null)
            {
                //We must create a list of keys because changing a dictionary while inside a forearch leads to out of sync error
                //Read here for more deets:
                //https://answers.unity.com/questions/409835/out-of-sync-error-when-iterating-over-a-dictionary.html
                if (_buildKeyList)
                {
                    _keys = new List<KeyCode>(_keyMaps.Keys);
                    _buildKeyList = false;
                }

                if (_keys != null)
                {
                    foreach (KeyCode key in _keys)
                    {
                        if (_currentControlSchema == ControlSchema.Active)
                        {
                            //checking non-movement keys
                            if (!_movementKeys.Contains(key))
                            {
                                if (Input.GetKeyDown(key))
                                {
                                    _keyMaps[key]();
                                }
                            }
                            //here we take care of long presses of movement keys
                            else
                            {
                                if (Input.GetKey(key))
                                    _keyMaps[key]();
                                else if (Input.GetKeyUp(key))
                                {
                                    _playerOneController.StopMovement();
                                    //_playerOneController.CheckForDash(key);
                                }
                            }
                        }
                        else
                        {
                            if (Input.GetKeyDown(key))
                                _keyMaps[key]();
                            else if (Input.GetKeyUp(key))
                                _playerOneController.StopMovement();
                        }
                        
                    }

                }

            }

        }

        public bool CanCast()
        {
            if (_currentControlSchema == ControlSchema.Active || _currentControlSchema == ControlSchema.DisableMovement)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Usueful if you want to return controls to a previous state - that state being ambiguous
        /// </summary>
        /// <param name="schema"></param>
        public void SetControls(ControlSchema schema)
        {
            switch (schema)
            {
                case ControlSchema.Active:
                    SetActiveControls();
                    break;
                case ControlSchema.Bond:
                    SetBondControls();
                    break;
                case ControlSchema.Cast:
                    SetCastControls();
                    break;
                case ControlSchema.Cutscene:
                    SetCutsceneControls();
                    break;
                case ControlSchema.Dialogue:
                    SetDialogueControls();
                    break;
                case ControlSchema.Everpresence:
                    SetEverpressenceControls();
                    break;
                case ControlSchema.KanjiLearned:
                    SetKanjiLearnedControls();
                    break;
                case ControlSchema.Locked:
                    SetLockedControls();
                    break;
                case ControlSchema.NotSet:
                    SetNotSetControls();
                    break;
                case ControlSchema.Pause:
                    SetPauseMenuControls();
                    break;
                case ControlSchema.SystemMessage:
                    SetSystemMessageControls();
                    break;
                case ControlSchema.Menu:
                    SetMenuControls();
                    break;
                case ControlSchema.KanjiDemo:
                    SetKanjiDemoControls();
                    break;
                case ControlSchema.DisableMovement:
                    SetDisabledMovementControls();
                    break;
                default:
                    SetActiveControls();
                    break;
            }
        }

        public void SetNotSetControls()
        {
            _currentControlSchema = ControlSchema.NotSet;
            _keyMaps.Clear();
        }

        public void SetCutsceneControls()
        {

        }

        public void SetActiveControls()
        {
            if (Time.timeScale != 1f)
                Time.timeScale = 1f;
            _currentControlSchema = ControlSchema.Active;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.Space, GameManager._instance.TogglePause);
            _keyMaps.Add(KeyCode.A, _playerOneController.MoveCharacter);
            _keyMaps.Add(KeyCode.S, _playerOneController.MoveCharacter);
            _keyMaps.Add(KeyCode.W, _playerOneController.MoveCharacter);
            _keyMaps.Add(KeyCode.D, _playerOneController.MoveCharacter);
            _keyMaps.Add(KeyCode.Q, _playerOneController.Attack);
            _keyMaps.Add(KeyCode.Mouse1, _playerOneController.Attack);
            _keyMaps.Add(KeyCode.R, _playerOneController.PerformDaxGrapple);
            _keyMaps.Add(KeyCode.E, _mainMenu.InitMenu);
            //_keyMaps.Add(KeyCode.I, _mainMenu.InitMenu);
            _keyMaps.Add(KeyCode.C, _playerOneController.Cast);
            _keyMaps.Add(KeyCode.LeftShift, _playerOneController.ScanInteractableObjects);
            if (_allowDebugControls)
                _keyMaps.Add(KeyCode.F, _playerOneController.CastDebugFireEnchantment);
            _buildKeyList = true;
        }

        public void SetDisabledMovementControls()
        {
            if (Time.timeScale != 1f)
                Time.timeScale = 1f;
            _currentControlSchema = ControlSchema.DisableMovement;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.Space, GameManager._instance.TogglePause);
            _keyMaps.Add(KeyCode.E, _mainMenu.InitMenu);
            _keyMaps.Add(KeyCode.C, _playerOneController.Cast);
            _buildKeyList = true;
        }

        public void SetMenuControls()
        {
            if (Time.timeScale != 0f)
                Time.timeScale = 0f;
            _playerOneController._anim.SetBool("moving", false);
            _currentControlSchema = ControlSchema.Menu;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.W, _currentMenu.MoveCursorUp);
            _keyMaps.Add(KeyCode.S, _currentMenu.MoveCursorDown);
            _keyMaps.Add(KeyCode.A, _currentMenu.MoveCursorLeft);
            _keyMaps.Add(KeyCode.D, _currentMenu.MoveCursorRight);
            _keyMaps.Add(KeyCode.Space, _currentMenu.ExecuteNode);
            _keyMaps.Add(KeyCode.Escape, _currentMenu.CloseMenu);
            _keyMaps.Add(KeyCode.E, _currentMenu.CloseMenu);
            _buildKeyList = true;
        }

        public void SetGameOverControls()
        {
            if (Time.timeScale != 0f)
                Time.timeScale = 0f;
            //_playerOneController._anim.SetBool("moving", false);
            _playerOneController._anim.enabled = false;
            _currentControlSchema = ControlSchema.GameOver;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.W, _currentMenu.MoveCursorUp);
            _keyMaps.Add(KeyCode.S, _currentMenu.MoveCursorDown);
            _keyMaps.Add(KeyCode.A, _currentMenu.MoveCursorLeft);
            _keyMaps.Add(KeyCode.D, _currentMenu.MoveCursorRight);
            _keyMaps.Add(KeyCode.Space, _currentMenu.ExecuteNode);
            _buildKeyList = true;
        }

        public void SetCastControls()
        {
            _playerOneController._anim.SetBool("moving", false);
            _currentControlSchema = ControlSchema.Cast;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.C, _playerOneController.Cast);
            _keyMaps.Add(KeyCode.Escape, _playerOneController.CancelCast);
            _keyMaps.Add(KeyCode.Space, _playerOneController.CommitKanji);
            _buildKeyList = true;
        }


        public void SetDialogueControls()
        {
            _currentControlSchema = ControlSchema.Dialogue;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.Space, DialogueManager._instance.AdvanceDialogue);
            //_keyMaps.Add(KeyCode.Escape, DialogueManager._instance.EndDialogue);
            _buildKeyList = true;
        }

        public void SetSystemMessageControls()
        {
            _currentControlSchema = ControlSchema.SystemMessage;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.Space, SystemMessageManager._instance.AdvanceDialogue);
            _keyMaps.Add(KeyCode.LeftShift, SystemMessageManager._instance.AdvanceDialogue);
            //_keyMaps.Add(KeyCode.Escape, SystemMessageManager._instance.EndDialogue);
            _buildKeyList = true;
        }


        public void SetKanjiLearnedControls()
        {
            _currentControlSchema = ControlSchema.KanjiLearned;
            _keyMaps.Clear();
            //_keyMaps.Add(KeyCode.LeftShift, DialogueManager._instance.EndKanjiLearnedSequence);
            _keyMaps.Add(KeyCode.Space, DialogueManager._instance.EndKanjiLearnedSequence);
            _keyMaps.Add(KeyCode.Escape, DialogueManager._instance.EndKanjiLearnedSequence);
            _buildKeyList = true;
        }

        public void SetBondControls()
        {
            _currentControlSchema = ControlSchema.Bond;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.Space, _playerOneController.LearnKanji);
            _buildKeyList = true;
        }

        public void SetPauseMenuControls()
        {
            _currentControlSchema = ControlSchema.Pause;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.Space, GameManager._instance.TogglePause);
            _buildKeyList = true;
        }

        public void SetKanjiDemoControls()
        {
            _currentControlSchema = ControlSchema.KanjiDemo;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.Escape, _kanjiDemo.StopKanjiDemo);
            _buildKeyList = true;
        }

        public void SetLockedControls()
        {
            _playerOneController.StopMovement();
            _currentControlSchema = ControlSchema.Locked;
            _keyMaps.Clear();
            _buildKeyList = true;
        }

        public void SetEverpressenceControls()
        {
            _currentControlSchema = ControlSchema.Everpresence;
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.Space, GameManager._instance.TogglePause);
            _buildKeyList = true;
        }
    }

}
