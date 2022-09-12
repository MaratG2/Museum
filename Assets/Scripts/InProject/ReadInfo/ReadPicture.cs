using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class ReadPicture : ReadText
{
    [SerializeField]
    private Material PictureMaterial;

    [SerializeField]
    private Material FrameMaterial;
    [SerializeField]
    private Texture2D picture;

    private void Awake()
    {
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