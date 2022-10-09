using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class SpriteReflection : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteToReflect;
        [SerializeField] private float _spriteAlpha = 0.5f;
        [SerializeField] private float _yOffset;
        private void Awake()
        {
            var sr = GetComponent<SpriteRenderer>();
            sr.flipY = true;
            //sr.sortingOrder = _spriteToReflect.sortingOrder;
            var tempColor = sr.color;
            tempColor.a = _spriteAlpha;
            sr.color = tempColor;
            _yOffset = _yOffset == 0 ? _spriteToReflect.bounds.size.y : _yOffset;
            var targetPos = _spriteToReflect.transform.position;
            targetPos += new Vector3(0f, -_yOffset, 0f);
            transform.position = targetPos;
        }
        private void Update()
        {
            if (_spriteToReflect.gameObject == null)
            {
                Destroy(gameObject);
            }
            GetComponent<SpriteRenderer>().sprite = _spriteToReflect.sprite;
        }
    }

}
