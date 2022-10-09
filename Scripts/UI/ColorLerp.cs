using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ColorLerp : MonoBehaviour
    {
        [SerializeField] private Color _color1;
        [SerializeField] private Color _color2;
        [SerializeField] private Color _color3;

        private void Start()
        {
            StartCoroutine(InitColorTransitions());
        }

        private IEnumerator InitColorTransitions()
        {
            //lerpedColor = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));
            while (GetComponent<Image>().color != _color1)
            {
                GetComponent<Image>().color = Color.Lerp(Color.white, _color1, Time.time);
                yield return null;
            }
            while (GetComponent<Image>().color != _color2)
            {
                GetComponent<Image>().color = Color.Lerp(_color1, _color2, Time.time);
                yield return null;
            }
            //yield return new WaitForSeconds(1f); 
            //GetComponent<Image>().color = Color.Lerp(_color1, _color2, Time.time);
            //yield return new WaitForSeconds(1f);
            //GetComponent<Image>().color = Color.Lerp(_color2, _color3, Time.time);
        }
    }
}
