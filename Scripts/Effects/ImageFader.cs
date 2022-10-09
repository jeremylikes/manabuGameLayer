using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFader : MonoBehaviour
{
    [SerializeField] private float _startFadeAfter;
    [SerializeField] private float _fadeTimer;
    [SerializeField] private float _startAlpha;
    private enum FadeTypes { fadeIn, fadeOut};
    [SerializeField] FadeTypes _fadeType;

    private void Start()
    {
        StartCoroutine(DisableScript());
        var image = GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = _startAlpha;
        image.color = tempColor;
    }

    private IEnumerator DisableScript()
    {
        yield return new WaitForSeconds(_fadeTimer + _startFadeAfter);
        enabled = false;
    }

    void Update()
    {
        if (_startFadeAfter > 0f)
        {
            _startFadeAfter -= Time.deltaTime;
            return;
        }
        if (_fadeType == FadeTypes.fadeOut && GetComponent<Image>().color.a > 0f)
        {
            var tempColor = GetComponent<Image>().color;
            tempColor.a -= Time.deltaTime / _fadeTimer;
            GetComponent<Image>().color = tempColor;
            
        }
        if (_fadeType == FadeTypes.fadeIn && GetComponent<Image>().color.a < 1f)
        {
            var tempColor = GetComponent<Image>().color;
            tempColor.a += Time.deltaTime / _fadeTimer;
            GetComponent<Image>().color = tempColor;
        }
    }
}
