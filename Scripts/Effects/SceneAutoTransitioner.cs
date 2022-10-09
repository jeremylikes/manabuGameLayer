using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneAutoTransitioner : MonoBehaviour
{
    [SerializeField] private SceneNames _sceneToTransitionTo;
    [SerializeField] private float _transitionAfterSeconds = 2f;
    [SerializeField] private bool _smoothTransition = false;
    [SerializeField] private float _smoothTransitionTime = 2f;

    void Update()
    {
        while (_transitionAfterSeconds > 0f)
        {
            _transitionAfterSeconds -= Time.deltaTime;
            return;
        }
        if (_smoothTransition)
        {
            var img = GetComponent<Image>();
            while (img.color.a < 1f)
            {
                var tempColor = img.color;
                tempColor.a += Time.deltaTime / _smoothTransitionTime;
                img.color = tempColor;
                return;
            }
            SceneManager.LoadScene(_sceneToTransitionTo.ToString());
        }
        else
            SceneManager.LoadScene(_sceneToTransitionTo.ToString());
    }
}
