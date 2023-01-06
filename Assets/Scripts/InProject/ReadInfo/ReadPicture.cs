using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

#pragma warning disable 0649
public class ReadPicture : ReadText
{
    [SerializeField]
    public Material PictureMaterial;
    [SerializeField]
    private Material FrameMaterial;
    [SerializeField]
    public Texture2D picture;

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
        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (!request.isDone)
        {
            Debug.Log(request.error);
        }
        else
        {
            var newTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            UpdatePicture(newTexture);
        }
    }
    
    private void UpdatePicture(Texture2D newImage)
    {
        picture = newImage;
        var spr= ReadFile.ToSpite(picture);
        var rd = new ReadFile();
        rd.sprite = spr;        
        ListFile.Insert(0,rd);
        Material mat = new Material(PictureMaterial);
        mat.SetTexture("_MainTex", picture);
        Material[] arrM = { FrameMaterial, mat };

        var rend = gameObject.GetComponent<Renderer>();
        rend.materials = arrM;
    }
}
#pragma warning restore 0649