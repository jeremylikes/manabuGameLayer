using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using UnityEngine.UI;
using TMPro;

namespace Managers
{
    public class StatPanelManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI[] _labels;
        [SerializeField]
        private TextMeshProUGUI[] _currents;
        [SerializeField]
        private TextMeshProUGUI[] _adjusteds;
        [SerializeField]
        private Manabu _manabu;

        private void OnValidate()
        {
            _labels = transform.Find("labels").GetComponentsInChildren<TextMeshProUGUI>();
            _currents = transform.Find("current").GetComponentsInChildren<TextMeshProUGUI>();
            _adjusteds = transform.Find("adjusted").GetComponentsInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            _manabu = GameObject.Find("Manabu").GetComponent<Manabu>();
            RefreshStatPanel();
            _manabu.OnStatChanged += RefreshStatPanel;
        }

        public void RefreshStatPanel()
        {
            var stats = _manabu.Stats;
            for (int i = 0; i < _labels.Length; i++)
            {
                foreach(var stat in stats)
                {
                    if (stat.Key.ToString() == _labels[i].text)
                    {
                        _currents[i].text = $"{stat.Value._current} / {stat.Value._max}";
                        break;
                    }
                }
            }
        }
    }
}

