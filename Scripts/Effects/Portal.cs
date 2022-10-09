using Interactables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] _scriptsToEnable;
    [SerializeField] private GameObject[] _objectsToEnable;
    private void Start()
    {
        var puzz = GetComponentInParent<Puzzle>() ?? null;
        if (puzz != null)
            puzz._onPuzzleResolved += OpenPortal;
    }

    public void OpenPortal()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        if(_scriptsToEnable.Length > 0)
            foreach (var obj in _scriptsToEnable)
                obj.enabled = true;
        if (_objectsToEnable.Length > 0)
            foreach (var obj in _objectsToEnable)
                obj.SetActive(true);
        GetComponent<AudioSource>().Play();
    }

    public void DissolvePortal()
    {
        GetComponent<SpriteRenderer>().enabled = false;

    }
}
