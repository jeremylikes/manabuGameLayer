using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Characters;
using System.Security;

namespace Effects
{
    public class SpecialEffects : MonoBehaviour
    {

        //public GameObject _castEffectDarkener;
        public Camera _mainCam;
        private float _cameraOriginalSize;
        private float _cameraOriginalYPos;
        private float _zoomModifier = 0.2f;
        private const float _defaultFadeInterval = 2f;
        private List<IEnumerator> _currentEffectCoroutines = new List<IEnumerator>();
        private GameObject _currentEffectObj;

        void Start()
        {
            _cameraOriginalSize = _mainCam.orthographicSize;
            _cameraOriginalYPos = _mainCam.transform.position.y;
        }

        public void KillAllActiveEffects()
        {
            if (_currentEffectCoroutines != null)
            {
                foreach (IEnumerator routine in _currentEffectCoroutines)
                    StopCoroutine(routine);
            }
        }

        public enum FadeOptions { fadeIn, fadeOut }
        public IEnumerator FadeImages(List<Image> targetImages, FadeOptions option, float overTime = 4f)
        {
            foreach (var targetImg in targetImages)
            {
                float increment = 1f / overTime;
                float targetAlpha = option == FadeOptions.fadeIn ? 1f : 0f;
                Color tempColor = targetImg.color;
                while (overTime > 0f)
                {
                    var step = option == FadeOptions.fadeIn ? Time.deltaTime * increment : -(Time.deltaTime * increment);
                    tempColor.a += step;
                    targetImg.color = tempColor;
                    overTime -= Time.deltaTime;
                    yield return null;
                }
            }

        }

        public IEnumerator FadeSprite(SpriteRenderer sr, FadeOptions option, float overTime = 4f)
        {
            float increment = 1f / overTime;
            float targetAlpha = option == FadeOptions.fadeIn ? 1f : 0f;
            var tempColor = sr.color;

            while (overTime > 0f)
            {
                var step = option == FadeOptions.fadeIn ? Time.deltaTime * increment : -(Time.deltaTime * increment);
                tempColor.a += step;
                sr.color = tempColor;
                overTime -= Time.deltaTime;
                yield return null;
            }
        }

        //public void TriggerCastEffect()
        //{
        //    //AudioManager._instance.PlaySoundEffect(Resources.Load());
        //    KillAllActiveEffects();
        //    //TODO: make sure the ann engines aren't being loaded dynamically
        //    //GameManager._instance._kanjiCanvas.gameObject.SetActive(true);
        //    DarkenScreen();
        //    IEnumerator coroutine = ZoomIn();
        //    //IEnumerator coroutine = ZoomInPerspectiveCamera();
        //    StartCoroutine(coroutine);
        //    _currentEffectCoroutines.Add(coroutine);
        //}

        //public void EndCastEffect()
        //{
        //    KillAllActiveEffects();
        //    RemoveDarkener();
        //    IEnumerator coroutine = ZoomOut(_cameraOriginalSize);
        //    //IEnumerator coroutine = ZoomOutPerspectiveCamera();
        //    StartCoroutine(coroutine);
        //    _currentEffectCoroutines.Add(coroutine);
        //}

        //private void DarkenScreen(float targetAlpha = 100f, float tween = _defaultFadeInterval)
        //{
        //    IEnumerator coroutine = FadeObject(_castEffectDarkener, 100f);
        //    _castEffectDarkener.gameObject.SetActive(true);
        //    StartCoroutine(coroutine);
        //    _currentEffectCoroutines.Add(coroutine);
        //}

        //private void RemoveDarkener(float tween = _defaultFadeInterval)
        //{
        //    IEnumerator coroutine = FadeObject(_castEffectDarkener, 0.1f, _defaultFadeInterval, true);
        //    StartCoroutine(coroutine);
        //    _currentEffectCoroutines.Add(coroutine);
        //}

        public void GeneratePortalEffectAtPosition(UnityEngine.Vector3 target)
        {
            _currentEffectObj = Instantiate(Resources.Load<GameObject>(@"Prefabs/Effects/Portal"), target, UnityEngine.Quaternion.identity);
            _currentEffectObj.GetComponent<Portal>().OpenPortal();
            
        }

        public void DestroyActiveEffects()
        {
            Destroy(_currentEffectObj);
            _currentEffectObj = null;
        }

        public IEnumerator FadeObject(GameObject go, float endAlpha = 1f, float tweenTime = _defaultFadeInterval, bool disable = false)
        {
            var targetImg = go.GetComponent<Image>();
            if (targetImg.color.a == 1f && endAlpha == 1f)
            {
                var tempColor = targetImg.color;
                tempColor.a = 0.01f;
                targetImg.color = tempColor;
            }
            targetImg.CrossFadeAlpha(endAlpha, tweenTime, false);

            while (tweenTime > 0f)
            {
                tweenTime -= Time.deltaTime;
                yield return null;
            }
            if (disable)
                go.gameObject.SetActive(false);

        }

        public IEnumerator FadeInCanvasOverlay(float overTime)
        {
            float increment = 1f / overTime;
            var canvasOverlay = GameManager._instance._canvasManager._screenOverlay;
            var img = canvasOverlay.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
            var tempColor = img.color;
            canvasOverlay.SetActive(true);
            while (overTime > 0f)
            {
                var step = Time.deltaTime * increment;
                tempColor.a += step;
                img.color = tempColor;
                overTime -= Time.deltaTime;
                yield return null;
            }
        }

        public IEnumerator FadeOutCanvasOverlay(float overTime)
        {
            float increment = 1f / overTime;
            var canvasOverlay = GameManager._instance._canvasManager._screenOverlay;
            var img = canvasOverlay.GetComponent<Image>();
            if (img.color.a < 1f)
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
            var tempColor = img.color;

            while (overTime > 0f)
            {
                var step = -(Time.deltaTime * increment);
                tempColor.a += step;
                img.color = tempColor;
                overTime -= Time.deltaTime;
                yield return null;
            }
            // reset for future use
            canvasOverlay.SetActive(false);
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
        }

        public IEnumerator FlashSpriteTransparent(SpriteRenderer sr, float timer)
        {
            float flashInterval = 0.2f;
            float flashTimer = 0f;
            var transparentColor = sr.color;
            transparentColor.a = 0f;
            var originalColor = sr.color;
            bool flashActive = true;
            sr.color = transparentColor;
            while (timer >= 0f)
            {
                timer -= Time.deltaTime;
                flashTimer += Time.deltaTime;
                if (flashTimer >= flashInterval)
                {
                    flashTimer = 0f;
                    flashActive = !flashActive;
                    sr.color = flashActive ? transparentColor : originalColor;
                }
                yield return null;
            }
            sr.color = originalColor;
            yield return null;
        }

        public IEnumerator FlashSpriteForTime(SpriteRenderer sr, float timer)
        {
            float flashInterval = 1f;
            var flashMat = Resources.Load<Material>(@"Materials/pureWhite");
            var originalMat = sr.material;
            bool flashActive = false;
            while (timer >= 0f)
            {
                timer -= Time.deltaTime;
                flashInterval -= Time.deltaTime;
                if (flashInterval <= 0f)
                {
                    flashInterval = 1f;
                    flashActive = !flashActive;
                    sr.material = flashActive ? flashMat : originalMat;
                }
                yield return null;
            }

        }

        public IEnumerator DestroyAfterTime(GameObject target, float timer = 2f)
        {
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            Debug.Log("DESTROY");
            Destroy(target);
        }

        public IEnumerator EmitKanjiSpeechBubble(string kanjiString, Vector3 atLocation)
        {
            // Instantiate the medallion
            //atLocation += new Vector3(0f, 0.4f, 0f);
            //var kanjiMedallion = Instantiate(Resources.Load<GameObject>(@"Prefabs/Spells/kanjiMedallion"), atLocation, Quaternion.identity);
            //var defaultKanjiSprite = kanjiMedallion.transform.Find("kanji").GetComponent<SpriteRenderer>().sprite;
            //var kanjiSprite = Resources.Load<Sprite>($@"Sprites/Radiants/anim/{kanjiString}/final") ?? null;
            //kanjiMedallion.transform.Find("kanji").GetComponent<SpriteRenderer>().sprite = kanjiSprite == null ? defaultKanjiSprite : kanjiSprite;

            //yield return new WaitForSeconds(2f);

            //var kanjiTextObj = Instantiate(_kanjiTextBubblePrefab, transform.position, Quaternion.identity);
            var kanjiTextObj = Instantiate(Resources.Load<GameObject>(@"Prefabs/KanjiTextBubble"), atLocation, Quaternion.identity);

            var textObj = kanjiTextObj.GetComponentInChildren<Text>();
            textObj.text = kanjiString;
            float timer = 1f;
            float rand = Random.Range(0f, 1f);
            Vector3 randVect = Vector3.zero;
            if (rand <= 0.25f)
                randVect = new Vector3(-1f, 1f, 0f);
            if (rand > 0.25f && rand <= 0.5f)
                randVect = new Vector3(1f, 1f, 0f);
            if (rand > 0.5f && rand <= 0.75f)
                randVect = new Vector3(-0.5f, 1f, 0f);
            if (rand > 0.75f)
                randVect = new Vector3(0.5f, 1f, 0f);
            while (timer > 0f)
            {
                kanjiTextObj.transform.Translate(randVect * (Time.deltaTime * 0.5f));
                timer -= Time.deltaTime;
                yield return null;

            }
            Destroy(kanjiTextObj);
            //Destroy(kanjiMedallion);
        }

        public IEnumerator FlashSpriteOnce(SpriteRenderer sr, float duration = 0.1f)
        {
            var flashMat = Resources.Load<Material>(@"Materials/pureWhite");
            var originalMat = sr.material;
            sr.material = flashMat;
            yield return new WaitForSeconds(duration);
            sr.material = originalMat;

        }
        public IEnumerator ZoomIn(float zoomTarget = 0f, float speed = _defaultFadeInterval)
        {
            if (zoomTarget == 0f)
                zoomTarget = GameManager._instance._defaultCameraOrthoSize - 2f;
            while (_mainCam.orthographicSize > zoomTarget)
            {
                _mainCam.orthographicSize -= Time.deltaTime * speed;
                yield return null;
            }

        }

        public void ProcessDamageParticles(Character target)
        {
            StartCoroutine(InitDamageParticles(target));
        }

        private IEnumerator InitDamageParticles(Character target)
        {
            var particles = Instantiate(Resources.Load<ParticleSystem>(@"Prefabs/Particles/DamageParticles"));
            var killTimer = particles.main.duration;
            particles.transform.position = target.transform.position;
            yield return new WaitForSeconds(killTimer * 3f);
            Destroy(particles);
        }

        public IEnumerator ZoomOut(float speed = _defaultFadeInterval)
        {
            while (_mainCam.orthographicSize < GameManager._instance._defaultCameraOrthoSize)
            {
                _mainCam.orthographicSize += Time.deltaTime * speed;
                yield return null;
            }
        }

        public IEnumerator ZoomInPerspectiveCamera()
        {
            float timer = 1.5f;
            Vector3 yZoom = new Vector3(0f, Time.deltaTime / 2f, 0f);
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                _mainCam.transform.position -= yZoom;
                yield return null;
            }
        }

        public IEnumerator ZoomOutPerspectiveCamera()
        {
            Vector3 yZoom = new Vector3(0f, Time.deltaTime, 0f);
            while (_mainCam.transform.position.y < _cameraOriginalYPos)
            {
                _mainCam.transform.position += yZoom;
                yield return null;
            }

        }

    }
}

