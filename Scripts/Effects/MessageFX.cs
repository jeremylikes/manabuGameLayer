using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;

namespace Effects
{
    public class MessageFX : MonoBehaviour
    {
        //public enum AnimationTypes
        //{
        //    StatLoss,
        //    StatGain,
        //    AreaIntro

        //}

        //public AnimationTypes _animationType;


        public IEnumerator AnimateHPMPPopup(GameObject damageText)
        {
            float duration = 1f;
            float speed = 0.5f;
            var currPos = damageText.transform.position;
            // Had to move the change to the z axis due to the orthographic camera
            var endPos = new Vector3(currPos.x, currPos.y + 2f, currPos.z);
            while (duration > 0f)
            {
                //Vector3.MoveTowards(transform.position, endPos, _speed * Time.deltaTime);
                // damageText.transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
                damageText.transform.position = Vector3.MoveTowards(damageText.transform.position, endPos, Time.deltaTime * speed);
                duration -= Time.deltaTime;
                yield return null;
            }
            Destroy(damageText.gameObject);
        }

        public void DisplayDamagePopup(int delta, Vector3 atPosition, Quaternion atRotation, bool wasCritical)
        {
            float yDiff = 0.2f;
            atRotation = atRotation == null ? new Quaternion(0.5f, 0f, 0f, 0.9f) : atRotation; 
            Vector3 displayPos = new Vector3(atPosition.x, atPosition.y + yDiff, atPosition.z);
            var popup = (GameObject)Instantiate(Resources.Load("Popups/pfPopupText"), displayPos, atRotation);
            TextMeshPro textMesh = popup.GetComponent<TextMeshPro>();
            DamagePopup damagePopup = popup.GetComponent<DamagePopup>();
            textMesh.color = delta >= 0 ? Color.green : Color.red;
            textMesh.fontSize = wasCritical
                ? textMesh.fontSize * 2
                : textMesh.fontSize;
            damagePopup.Setup(Math.Abs(delta));
            StartCoroutine(AnimateHPMPPopup(popup));
        }

    }

}
