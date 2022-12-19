using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Admin.Utility
{
    public class VideoFrame : MonoBehaviour
    {
        [SerializeField] public string _videoUrl;
        [SerializeField] [Range(0f, 1f)] private float _volume = 0.2f;
        [SerializeField]private VideoPlayer _videoPlayer;

        void Start()
        {
            /*videoPlayer = GetComponent<VideoPlayer>();*/
            _videoPlayer.url = _videoUrl;
            _videoPlayer.Play();
            _videoPlayer.SetDirectAudioVolume(0, _volume);
        }
    }
}