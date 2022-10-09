using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(AudioSource))]

    public class CoreMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _cursorPrefab;
        [SerializeField] private GameObject _cursor;
        [SerializeField] protected List<Transform> _menuNodes;
        [SerializeField] private Transform _currentMenuNode;
        [SerializeField] protected LayoutMatrix _layout;
        [SerializeField] private AudioClip _menuInitSound;
        [SerializeField] private AudioClip _cursorMoveSound;
        [SerializeField] private CoreMenu _parentMenu;
        [SerializeField] private Transform _secondaryDisplay;
        [SerializeField] private CoreMenuController _controller;
        [SerializeField] private AudioSource _audio;
        private Transform[,] _nodeMatrix;
        private CursorIndex _cursorIndex;

        private void OnValidate()
        {
            _menuNodes.Clear();
            var buttons = GetComponentsInChildren<Button>();
            foreach (var b in buttons)
            {
                _menuNodes.Add(b.transform);
            }
            _controller = GetComponent<CoreMenuController>();
            _audio = GetComponent<AudioSource>();
        }

        private void Start()
        {
            //InitMenu();
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(0.2f);
            InitMenu();
        }

        public void InitMenu()
        {
            _controller.SetCoreMenuControls();
            _cursorIndex = new CursorIndex();
            //gameObject.SetActive(true);
            //AudioManager._instance.PlaySoundEffect(_menuInitSound);
            PlaySound(_menuInitSound);
            if (_secondaryDisplay != null)
                _secondaryDisplay.gameObject.SetActive(true);
            BuildNodeMatrix();
            ToggleCursor(true);
        }

        public void CloseMenu()
        {
            _controller.ReleaseCoreMenuControls();
            _cursorIndex = new CursorIndex();
            ToggleCursor(false);
            if (_parentMenu != null)
            {
                _controller.SetCoreMenu(_parentMenu);
                _controller.SetCoreMenuControls();
            }
            if (_secondaryDisplay != null)
                _secondaryDisplay.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        private void PlaySound(AudioClip ac)
        {
            _audio.clip = ac;
            _audio.Play();
        }

        private void BuildNodeMatrix()
        {
            _nodeMatrix = new Transform[_layout._rows, _layout._columns];
            int maxNodeIndex = _menuNodes.Count - 1;
            int currentNodeIndex = 0;
            for (int r = 0; r < _layout._rows; r++)
            {
                if (currentNodeIndex > maxNodeIndex)
                    break;
                for (int c = 0; c < _layout._columns; c++)
                {
                    _nodeMatrix[r, c] = _menuNodes[currentNodeIndex];
                    currentNodeIndex++;
                }
            }
        }

        private void PlayCursorMoveSound()
        {
            PlaySound(_cursorMoveSound);
        }


        private void ToggleCursor(bool setting)
        {
            if (setting)
            {
                _cursor = Instantiate(_cursorPrefab);
                PositionCursor();
            }
            else
            {
                Destroy(_cursor);
            }
        }

        public void ExecuteNode()
        {
            _currentMenuNode.gameObject.GetComponent<Button>().onClick?.Invoke();
        }

        //public void DisplayMenu(Menu menu)
        //{
        //    menu.InitMenu();
        //}

        private void PositionCursor()
        {
            _currentMenuNode = _nodeMatrix[_cursorIndex._rowIndex, _cursorIndex._colIndex];
            Transform target = _currentMenuNode;
            _cursor.transform.SetParent(target);
            var originalPos = target.position;
            var leftEdge = target.GetComponent<RectTransform>().rect.width / 2f;
            var offset = leftEdge * 1.5f;
            var targetPos = new Vector3(originalPos.x - offset, originalPos.y, originalPos.z);
            //var targetPos = originalPos;
            _cursor.transform.position = targetPos;
        }

        public void MoveCursorDown()
        {
            PlayCursorMoveSound();
            int targetRowIndex = _cursorIndex._rowIndex + 1 < _layout._rows ? _cursorIndex._rowIndex + 1 : 0;
            _cursorIndex._rowIndex = targetRowIndex;
            PositionCursor();
        }

        public void MoveCursorUp()
        {
            PlayCursorMoveSound();
            int targetRowIndex = _cursorIndex._rowIndex - 1 >= 0 ? _cursorIndex._rowIndex - 1 : _layout._rows - 1;
            _cursorIndex._rowIndex = targetRowIndex;
            PositionCursor();
        }

        public void MoveCursorLeft()
        {
            PlayCursorMoveSound();
            int targetColIndex = _cursorIndex._colIndex - 1 >= 0 ? _cursorIndex._colIndex - 1 : _layout._columns - 1;
            _cursorIndex._colIndex = targetColIndex;
            PositionCursor();
        }

        public void MoveCursorRight()
        {
            PlayCursorMoveSound();
            int targetColIndex = _cursorIndex._colIndex + 1 < _layout._columns ? _cursorIndex._colIndex + 1 : 0;
            _cursorIndex._colIndex = targetColIndex;
            PositionCursor();
        }

        [Serializable]
        protected class LayoutMatrix
        {
            public int _rows = 1;
            public int _columns = 1;
        }

        protected class CursorIndex
        {
            public int _rowIndex = 0;
            public int _colIndex = 0;
        }
    }
}

