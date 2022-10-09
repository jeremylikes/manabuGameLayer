using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.SceneManagement;

public class SetLanguageTransitionManager : MonoBehaviour
{
    public void SetEnglish()
    {
        SetAndTransition(GameStateManager.GameLanguages.English);
    }

    public void SetJapanese()
    {
        SetAndTransition(GameStateManager.GameLanguages.Japanese);
    }

    private void SetAndTransition(GameStateManager.GameLanguages language)
    {
        GameStateManager._instance.SetGameLanguage(language);
        SceneManager.LoadScene(SceneNames.DesertTransport.ToString());
    }

}
