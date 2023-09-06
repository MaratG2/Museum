using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

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
            picture = newImage;
            var spr= ReadFile.ToSpite(picture);
            var rd = new ReadFile
            {
                sprite = spr
            };
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