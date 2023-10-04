using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Admin.Utility
{
    /// <summary>
    /// Отвечает за загрузку и воспроизведение видео из Интернета (GitHub Pages) по ссылке при взаимодействии с пользователем.
    /// </summary>
    public class VideoFrame : MonoBehaviour, IInteractive
    {
        [SerializeField] public string videoUrl;
        [SerializeField] [Range(0f, 1f)] private float volume = 0.2f;
        [SerializeField] private VideoPlayer videoPlayer;
         private bool _isPlayed;

         public string Title { get; set; }

         void Start()
        {
            videoPlayer.url = videoUrl;
            videoPlayer.Pause();
            videoPlayer.SetDirectAudioVolume(0, volume);
            StopAllCoroutines();
            StartCoroutine(SetSize());
        }

         public void Interact()
        {
            if (_isPlayed) videoPlayer.Pause();
            else
            {
                videoPlayer.Play();
            }
            _isPlayed = !_isPlayed;
        }

         private IEnumerator SetSize()
         {
             yield return new WaitUntil(() => videoPlayer.isPrepared);
             
             Vector2 initScale = new Vector2(1f, 1.7f);
             initScale /= 1.5f;
             float xMulti = videoPlayer.width / (float)videoPlayer.height;
             Debug.Log(xMulti);
             Mathf.Clamp(xMulti, 0.3f, 3.3f);
             if (xMulti >= 1f)
                 initScale.x *= xMulti;
             else
                 initScale.y /= xMulti;
             transform.localScale = new Vector3(initScale.x, initScale.y, 1f);
         }

         public bool IsOpen()
         {
             return false;
         }
    }
}