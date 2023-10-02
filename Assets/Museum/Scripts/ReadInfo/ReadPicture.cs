using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

#pragma warning disable 0649
namespace Museum.Scripts.ReadInfo
{
    public class ReadPicture : ReadText
    {
        [SerializeField]
        public Material PictureMaterial;
        [SerializeField]
        private Material FrameMaterial;
        [SerializeField]
        public Texture2D picture;
        private float _timer = 0f;
        private bool _hasGot;

        private void Awake()
        {
            UpdatePicture(picture);
        }

        public void SetNewPicture(string url)
        {
            StartCoroutine(LoadImage(url));
        }

        private IEnumerator LoadImage(string url)
        {
            _timer = 0f;
            var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (!request.isDone)
            {
                Debug.Log(request.error);
            }
            else
            {
                while (!request.downloadHandler.isDone)
                {
                    yield return null;
                    _timer += Time.deltaTime;
                    if (_timer > 10f)
                        break;
                }
                if (_timer <= 10f)
                {
                    var newTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    UpdatePicture(newTexture);
                }
            }
        }
    
        private void UpdatePicture(Texture2D newImage)
        {
            if (!_hasGot && ListFile.Count == 1)
            {
                ListFile = new List<ReadFile>();
                _hasGot = true;
            }
            
            picture = newImage;
            var spr= ReadFile.ToSpite(picture);
            var rd = new ReadFile
            {
                sprite = spr
            };
            
            Vector2 initScale = new Vector2(1f, 1.7f);
            float xMulti = spr.texture.width / (float)spr.texture.height;
            Mathf.Clamp(xMulti, 0.3f, 3.3f);
            if (xMulti >= 1f)
                initScale.x *= xMulti;
            else
                initScale.y /= xMulti;
            transform.localScale = new Vector3(initScale.x, initScale.y, 1f);
            
            ListFile.Insert(0,rd);
            var mat = new Material(PictureMaterial);
            mat.SetTexture("_MainTex", picture);
            Material[] arrM = { FrameMaterial, mat };

            var rend = gameObject.GetComponent<Renderer>();
            rend.materials = arrM;
        }
    }
}
#pragma warning restore 0649