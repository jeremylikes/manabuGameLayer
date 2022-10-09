using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandMask : MonoBehaviour
{
    [SerializeField] private float _lifeTimer = 10f;

    private void Start()
    {
        //var phys = new PhysicsHelpers();
        ////phys.SimulateWindResistance(GameManager._instance._mainCharacter.transform, 20f);
        //// insntatniate sandy air prefab
        //_manabuPos.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-0.001f, 0f));
    }
    // Update is called once per frame
    void Update()
    {
        while (_lifeTimer > 0f)
        {
            _lifeTimer -= Time.deltaTime;
            return;
        }
        Destroy(gameObject);
    }
}
