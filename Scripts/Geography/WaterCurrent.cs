using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;
using System.Runtime.InteropServices;
using Spells;

namespace Geography
{
    public class WaterCurrent : MonoBehaviour
    {
        [SerializeField] private Managers.CharacterController._directions _forceDirection = Managers.CharacterController._directions.Down;
        //[SerializeField] private bool _inWater = false;
        [SerializeField] private Vector2 _forceVector;
        [SerializeField] private float _thrust;
        [SerializeField] private List<GameObject> _objectsInCurrent;
      
        private void OnValidate()
        {
            ConvertDirectionToVector();
            //_thrust = 0.5f;
        }

        private void ConvertDirectionToVector()
        {
            switch (_forceDirection)
            {
                case Managers.CharacterController._directions.Up:
                    _forceVector = Vector2.up;
                    break;
                case Managers.CharacterController._directions.Down:
                    _forceVector = Vector2.down;
                    break;
                case Managers.CharacterController._directions.Left:
                    _forceVector = Vector2.left;
                    break;
                case Managers.CharacterController._directions.Right:
                    _forceVector = Vector2.right;
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var candidate = collision.gameObject;
            // We just want Manabu's foot collider to trigger this
            if (candidate.GetComponent<Manabu>() != null)
                return;
            bool isManabuFeet = collision.name == "FootCollider";
            if (isManabuFeet)
                candidate = candidate.transform.parent.gameObject;
            if (isManabuFeet || candidate.GetComponent<Stone>() != null)
            {
                if (!_objectsInCurrent.Contains(candidate))
                {
                    _objectsInCurrent.Add(candidate);
                    ToggleSplashEffects(candidate.transform, true);
                }
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var candidate = collision.gameObject;
            bool isManabuFeet = collision.name == "FootCollider";
            if (isManabuFeet)
                candidate = candidate.transform.parent.gameObject;
            // We just want Manabu's foot collider to trigger this
            if (_objectsInCurrent.Contains(candidate))
            {
                _objectsInCurrent.Remove(candidate);
                candidate.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                ToggleSplashEffects(candidate.transform, false);
            }
        }

        private void ToggleSplashEffects(Transform parent, bool setting)
        {
            if (setting)
            {
                var particles = Instantiate(Resources.Load<GameObject>($@"Prefabs/Effects/SplashParticles"), parent.position, Quaternion.identity);
                particles.transform.parent = parent;
            }
            else
            {
                foreach(Transform child in parent)
                {
                    if (child.name.Contains("SplashParticles"))
                        Destroy(child.gameObject);
                }
            }
        }

        private bool CheckForManabu(Collider2D collision)
        {
            return collision.GetComponent<Manabu>() != null;
        }
        
        private void ApplyCurrentForce()
        {
            if (_objectsInCurrent.Count > 0)
            {
                foreach (var obj in _objectsInCurrent)
                {
                    obj.GetComponent<Rigidbody2D>().AddForce(_forceVector * _thrust);
                }
            }
            //var manabu = GameManager._instance._mainCharacter;
            //// with unity physics
            //manabu.GetComponent<Rigidbody2D>().AddForce(_forceVector * _thrust);

            // with transform manipulation
            // manabu.transform.position += new Vector3(_forceVector.x * _thrust, _forceVector.y * _thrust, 0f);
        }

        void Update()
        {
            //if (_inWater)
            //{
                ApplyCurrentForce();
            //}
        }
    }

}
