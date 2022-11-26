using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Video : MonoBehaviour
{
    [SerializeField] private string _videoUrl;
    private const string CHerokuURL = "https://unity-youtube-dl-server.herokuapp.com";
    private const string CHerokuEndURL = "&cli=yt-dlp";
    private RawImage _rawImage;
    private VideoPlayer _videoPlayer;
    
    // Start is called before the first frame update
    
    void Start()
    {
        _rawImage = GetComponent<RawImage>();
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoUrl = GenerateUnityYoutubePlayerUrl(_videoUrl);
        StartCoroutine(PlayVideo());
    }

    private string GenerateUnityYoutubePlayerUrl(string url)
    {
        int indexLastSlash = url.LastIndexOf('/');
        string ytUrl = url.Substring(indexLastSlash);
        if (ytUrl.Length > 0 && ytUrl[0] != 'w')
            ytUrl = string.Concat("/watch?v=", ytUrl.Substring(1));
        
        return CHerokuURL + ytUrl + CHerokuEndURL;
    }

    IEnumerator PlayVideo()
    {
        if (_videoPlayer == null || _rawImage == null || string.IsNullOrWhiteSpace(_videoUrl))
            yield break;
 
        _videoPlayer.url = _videoUrl;
        _videoPlayer.renderMode = VideoRenderMode.APIOnly;
        _videoPlayer.Prepare();
        while (!_videoPlayer.isPrepared)
            yield return new WaitForSeconds(1);
 
        _rawImage.texture = _videoPlayer.texture;
        _videoPlayer.Play();
        _videoPlayer.SetDirectAudioVolume(0, 0.05f);
    }
}
