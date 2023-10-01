using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using InProject;
using UnityEngine;
using UnityEngine.Networking;

namespace Museum.Scripts.ReadInfo
{
    public class ReadText : MonoBehaviour, IInteractive
    {
        private bool _flag;
        public List<ReadFile> ListFile = new();
        private float _timer = 0f;

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

        public bool IsOpen()
        {
            return _flag;
        }

        public void AddNewInfo(string url, string description)
        {
            StartCoroutine(LoadImage(url, description));
        }

        private IEnumerator LoadImage(string url, string description)
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
                if(_timer <= 10f)
                {
                    var newTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
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