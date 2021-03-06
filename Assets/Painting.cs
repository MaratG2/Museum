using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Painting : MonoBehaviour
{
    [SerializeField] private RawImage _paintingImage;

    public RawImage PaintingImage => _paintingImage;

    public IEnumerator LoadImage(string webURL)
    {
        WWW www = new WWW(webURL);
        yield return www;
        _paintingImage.texture = www.texture;
    }
}
