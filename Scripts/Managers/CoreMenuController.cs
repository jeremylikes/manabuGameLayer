using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Managers
{
    public class CoreMenuController : MonoBehaviour
    {
        Dictionary<KeyCode, System.Action> _keyMaps = new Dictionary<KeyCode, System.Action>();
        [SerializeField] private CoreMenu _coreMenu;
        private List<KeyCode> _keys = new List<KeyCode>();

        private void Update()
        {
            if (_keys != null)
            {
                foreach (KeyCode key in _keys)
                {
                    if (Input.GetKeyDown(key))
                        _keyMaps[key]();
                }
            }
        }

        public void SetCoreMenu(CoreMenu menu)
        {
            _coreMenu = menu;
        }

        public void SetCoreMenuControls()
        {
            ToggleControlsManager(false);
            _keyMaps.Clear();
            _keyMaps.Add(KeyCode.W, _coreMenu.MoveCursorUp);
            _keyMaps.Add(KeyCode.S, _coreMenu.MoveCursorDown);
            _keyMaps.Add(KeyCode.A, _coreMenu.MoveCursorLeft);
            _keyMaps.Add(KeyCode.D, _coreMenu.MoveCursorRight);
            _keyMaps.Add(KeyCode.Space, _coreMenu.ExecuteNode);
            _keyMaps.Add(KeyCode.Escape, _coreMenu.CloseMenu);
            _keyMaps.Add(KeyCode.E, _coreMenu.CloseMenu);
            _keys = new List<KeyCode>(_keyMaps.Keys);

        }

        public void ReleaseCoreMenuControls()
        {
            ToggleControlsManager(true);
        }

        private void ToggleControlsManager(bool setting)
        {
            var cm = FindObjectOfType<ControlsManager>() ?? null;
            if (cm != null)
                cm.gameObject.SetActive(setting);
        }
    }
}

