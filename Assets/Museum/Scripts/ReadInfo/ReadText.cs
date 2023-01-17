using System.Collections;
using System.Collections.Generic;
using InProject;
using UnityEngine;
using UnityEngine.Networking;

namespace Museum.Scripts.ReadInfo
{
    public class ReadText : MonoBehaviour, IInteractive
    {
        private bool _flag;
        public List<ReadFile> ListFile = new();

        public string Title { get; set; }

        public void Interact()
        {
            if (!_flag)
            {
                Open();
                _flag = true;
            }
            else
            {
                Close();
                _flag = false;
            }
        }
        public void AddNewInfo(string url, string description)
        {
            StartCoroutine(LoadImage(url, description));
        }

        private IEnumerator LoadImage(string url, string description)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (!request.isDone)
            {
                Debug.Log(request.error);
            }
            else
            {
                var newTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                var downloadSprite = ReadFile.ToSpite(newTexture);
            
                ListFile.Add(new ReadFile
                {
                    sprite = downloadSprite,
                    type = TypeObj.Image,
                });
            
                ListFile.Add(new ReadFile
                {
                    text = description,
                    type = TypeObj.Text,
                });
            }
        }

        void Open()
        {
            State.View(true);
            SetListForRead();
            StartCoroutine(ReadEvent.Instance.SetForm());
            ReadEvent.Instance.PicturePanel.SetActive(true);
        }

        void Close()
        {
            State.View(false);
            ReadEvent.Instance.DestroyList();
            ReadEvent.Instance.PicturePanel.SetActive(false);
        }

        void SetListForRead()
        {
            ReadEvent.Instance.ListFile = ListFile;
        }
    }
}