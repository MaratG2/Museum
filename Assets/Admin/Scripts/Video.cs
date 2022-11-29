using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Video : MonoBehaviour
{
    [SerializeField] private string _videoUrl;
    private RawImage _rawImage;
    private VideoPlayer _videoPlayer;
    
    // Start is called before the first frame update
    
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
        _videoPlayer.SetDirectAudioVolume(0, 0.2f);
    }
}
