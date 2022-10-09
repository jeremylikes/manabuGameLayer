using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class ScrollingBG : MonoBehaviour
    {
        private enum scrollDirections { up, down, left, right };
        [SerializeField] private float _scrollSpeed = 0.3f;
        [SerializeField] private scrollDirections _scrollDirection = scrollDirections.up;

        private Vector2 _startPos;
        private Vector2 _endPos;

        void Start()
        {
            _startPos = transform.position;
            float offset = 0f;
            var bounds = GetComponent<SpriteRenderer>().bounds;
            switch (_scrollDirection)
            {
                case scrollDirections.up:
                    offset = bounds.size.y / 2f;
                    _endPos = new Vector2(transform.position.x, transform.position.y + offset);
                    break;
                case scrollDirections.down:
                    offset = bounds.size.y / 2f;
                    _endPos = new Vector2(transform.position.x, transform.position.y - offset);
                    break;
                case scrollDirections.left:
                    offset = bounds.size.x / 2f;
                    _endPos = new Vector2(transform.position.x - offset, transform.position.y);
                    break;
                case scrollDirections.right:
                    offset = bounds.size.x / 2f;
                    _endPos = new Vector2(transform.position.x + offset, transform.position.y);
                    break;
                default:
                    break;
            }

        }

        void Update()
        {
            switch (_scrollDirection)
            {
                case scrollDirections.up:
                    if (transform.position.y >= _endPos.y)
                        transform.position = _startPos;
                    break;
                case scrollDirections.down:
                    if (transform.position.y <= _endPos.y)
                        transform.position = _startPos;
                    break;
                case scrollDirections.left:
                    if (transform.position.x <= _endPos.x)
                        transform.position = _startPos;
                    break;
                case scrollDirections.right:
                    if (transform.position.x >= _endPos.x)
                        transform.position = _startPos;
                    break;
            }
            transform.position = Vector2.MoveTowards(transform.position, _endPos, Time.deltaTime * _scrollSpeed);
        }
        //    [SerializeField] private float _scrollSpeed = 0.5f;
        //    private Vector2 _startPos;
        //    private Vector2 _endPos = new Vector2(-1.808f, 0.276f);
        //    void Start()
        //    {
        //        _startPos = transform.position;
        //    }

        //    void Update()
        //    {
        //        if (transform.position.x <= _endPos.x)
        //        {
        //            transform.position = _startPos;
        //        }
        //        transform.position = Vector2.MoveTowards(transform.position, _endPos, Time.deltaTime * _scrollSpeed);
        //    }
    }

}
