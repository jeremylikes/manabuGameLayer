using Characters;
using Collectables;
using Effects;
using GameSystems;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Image _titleImage;
    [SerializeField] private RectTransform _optionsMenu;
    [SerializeField] private RectTransform _mainMenu;
    [SerializeField] private AudioClip _startGameSE;

    private enum FadeOptions
    {
        fadeIn,
        fadeOut
    }

    private FadeOptions _fadeOption;

    private void OnValidate()
    {
        _titleImage = GameObject.Find("ManabuLogo").GetComponent<Image>();
    }

    void Awake()
    {
        var noAlpha = _titleImage.color;
        noAlpha.a = 0f;
        _titleImage.color = noAlpha;

    }

    private void Start()
    {
        InitStartup();
    }

    private void InitStartup()
    {
        _optionsMenu.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(false);
        List<Image> imagesToFade = new List<Image> { _titleImage };
        StartCoroutine(FadeImages(imagesToFade, FadeOptions.fadeIn, 4f));
        StartCoroutine(ExpandOptionsMenu());
    }

    private IEnumerator ExpandOptionsMenu()
    {
        var rect = _optionsMenu.rect;
        var rt = _optionsMenu.GetComponent<RectTransform>();
        float fullHeight = rect.height;
        rt.sizeDelta = new Vector2(rect.width, 0f);
        yield return new WaitForSeconds(1f);
        _optionsMenu.gameObject.SetActive(true);
        float expandTimer = 0.3f;
        float step = fullHeight / expandTimer;
        while (rt.rect.height < fullHeight)
        {
            rt.sizeDelta += new Vector2(0f, Time.deltaTime * step);
            expandTimer -= Time.deltaTime;
            yield return null;
        }
        _mainMenu.gameObject.SetActive(true);
    }

    private IEnumerator CollapseOptionsMenu()
    {
        _mainMenu.gameObject.SetActive(false);
        var rect = _optionsMenu.rect;
        var rt = _optionsMenu.GetComponent<RectTransform>();
        float fullHeight = rect.height;
        _optionsMenu.gameObject.SetActive(true);
        float expandTimer = 0.3f;
        float step = fullHeight / expandTimer;
        while (rt.rect.height > 0)
        {
            rt.sizeDelta -= new Vector2(0f, Time.deltaTime * step);
            expandTimer -= Time.deltaTime;
            yield return null;
        }
        _optionsMenu.gameObject.SetActive(false);

    }

    private IEnumerator FadeImages(List<Image> targetImages, FadeOptions option, float overTime = 4f)
    {
        foreach (var targetImg in targetImages)
        {
            float increment = 1f / overTime;
            float targetAlpha = option == FadeOptions.fadeIn ? 1f : 0f;
            Color tempColor = targetImg.color;
            while (overTime > 0f)
            {
                var step = option == FadeOptions.fadeIn ? Time.deltaTime * increment : -(Time.deltaTime * increment);
                tempColor.a += step;
                targetImg.color = tempColor;
                overTime -= Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator InitSceneTransition(string sceneName)
    {
        float fadeOutTimer = 3f;
        StartCoroutine(CollapseOptionsMenu());
        StartCoroutine(FadeImages(new List<Image>() { _titleImage }, FadeOptions.fadeOut, fadeOutTimer));
        yield return new WaitForSeconds(fadeOutTimer);
        SceneManager.LoadScene(sceneName);
    }

    public void DisplayLoadDataTiles()
    {
        
    }

    public void TransitionToScene(string sceneName)
    {
        var source = GetComponent<AudioSource>();
        source.clip = _startGameSE;
        source.Play();
        StartCoroutine(InitSceneTransition(sceneName));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowSettingsMenu()
    {

    }

    public void OpenSettings()
    {

    }

    public void ReturnToMainMenu()
    {

    }
}
