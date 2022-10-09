using Collectables;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private Transform _mainUICanvas;
        [SerializeField] private GameObject _cursorPrefab;
        [SerializeField] private GameObject _cursor;
        [SerializeField] protected List<Transform> _menuNodes;
        [SerializeField] private Transform _currentMenuNode;
        [SerializeField] protected LayoutMatrix _layout;
        [SerializeField] private AudioClip _menuInitSound;
        [SerializeField] private AudioClip _cursorMoveSound;
        [SerializeField] private Menu _parentMenu;
        [SerializeField] private Transform _secondaryDisplay;

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
        }

        public void InitMenu()
        {
            ControlsManager._instance._currentMenu = this;
            if (this is GameOverMenu)
            {
                ControlsManager._instance.SetGameOverControls();
                var gom = this as GameOverMenu;
                gom.SetMenuLayout();
            }
            else
                ControlsManager._instance.SetMenuControls();
            _cursorIndex = new CursorIndex();
            BuildNodeMatrix();
            gameObject.SetActive(true);
            AudioManager._instance.PlaySoundEffect(_menuInitSound);
            if (_secondaryDisplay != null)
                _secondaryDisplay.gameObject.SetActive(true);
            ToggleCursor(true);
        }

        public void CloseMenu()
        {
            _cursorIndex = new CursorIndex();
            ToggleCursor(false);
            if (_parentMenu != null)
            {
                ControlsManager._instance._currentMenu = _parentMenu;
                ControlsManager._instance.SetMenuControls();
            }
            else
                ControlsManager._instance.SetActiveControls();
            if (_secondaryDisplay != null)
                _secondaryDisplay.gameObject.SetActive(false);
            gameObject.SetActive(false);
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
            AudioManager._instance.PlaySoundEffect(_cursorMoveSound);
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

        public void DisplayMenu(Menu menu)
        {

            menu.InitMenu();

        }

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
