using Collectables;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipSubMenu : MonoBehaviour
{
    public Button _equipButton;
    public Button _dropButton;

    public void SetEquipText(bool equip)
    {
        _equipButton.GetComponentInChildren<TextMeshProUGUI>().text = equip ? "Equip" : "Unequip";
        
    }

}
