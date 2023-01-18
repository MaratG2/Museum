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
        }

         public void Interact()
        {
            if (_isPlayed) videoPlayer.Pause();
            else videoPlayer.Play();
            _isPlayed = !_isPlayed;
        }
    }
}