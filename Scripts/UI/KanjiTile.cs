using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KanjiTile : MonoBehaviour
{
    [SerializeField] string _targetKanji;

    public void SetTargetKanji(string kanji)
    {
        GetComponent<Image>().sprite = Resources.Load<Sprite>($@"Sprites/Radiants/anim/{kanji}/final");
        _targetKanji = kanji;
    }
    public string GetTargetKanji()
    {
        return _targetKanji;
    }
}
