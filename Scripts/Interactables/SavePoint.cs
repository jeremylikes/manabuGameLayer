using Interactables;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    [SerializeField] Menu _saveMenu;

    public void Interact()
    {
        GameManager._instance._mainCharacter.FullyReplenish();
        var sysMessages = GameManager._instance.gameObject.GetComponent<SystemMessageManager>();
        sysMessages._onSystemMessageEnded = _saveMenu.InitMenu;
    }
}
