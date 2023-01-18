using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Admin.Utility
{
    /// <summary>
    /// Отвечает за загрузку и воспроизведение видео в текстуре из Интернета (GitHub Pages) по ссылке.
    /// </summary>
    public class Video : MonoBehaviour
    {
        [SerializeField] private string _videoUrl;
        [SerializeField] [Range(0f, 1f)] private float _volume = 0.2f;
        private RawImage _rawImage;
        private VideoPlayer _videoPlayer;

        void Start()
        {
            _rawImage = GetComponent<RawImage>();
            _videoPlayer = GetComponent<VideoPlayer>();
            StartCoroutine(PlayVideo());
        }

        IEnumerator PlayVideo()
        {
            if (_videoPlayer == null || _rawImage == null || string.IsNullOrWhiteSpace(_videoUrl))
                yield break;

            _videoPlayer.url = _videoUrl;
            _videoPlayer.renderMode = VideoRenderMode.APIOnly;
            _videoPlayer.Prepare();
            while (!_videoPlayer.isPrepared)
                yield return new WaitForEndOfFrame();

            _rawImage.texture = _videoPlayer.texture;
            _videoPlayer.Play();
            _videoPlayer.SetDirectAudioVolume(0, _volume);
        }
    }
}