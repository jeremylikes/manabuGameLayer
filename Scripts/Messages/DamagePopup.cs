using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{

    private TextMeshPro _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
    }
    public void Setup(int damageAmt)
    {
        _textMesh.SetText(damageAmt.ToString());
    }
    // Start is called before the first frame update

}
