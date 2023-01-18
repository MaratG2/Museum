using Admin.GenerationMap;
using Admin.Utility;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отвечает за визуальное представление иконки на 2D плане Музея.
/// </summary>
public class Tile : MonoBehaviour
{
    public HallContent hallContent;
    private Image _image;

    [SerializeField] private Sprite _doorSprite, _frameSprite, _infoSprite, _cupSprite, _medalSprite, _rubberSprite, _videoSprite, _decorSprite;

    public void Setup()
    {
        _image = GetComponent<Image>();
        _image.sprite = null;
        SelectTool(hallContent.type);
    }

    private void SelectTool(int tool)
    {
        switch (tool)
        {
            case -2:
                _image.sprite = _rubberSprite;
                break;
            case -1:
                _image.sprite = null;
                break;
        }
        if (tool == ExhibitsConstants.Picture.Id)
            _image.sprite = _frameSprite;
        else if (tool == ExhibitsConstants.SpawnPoint.Id)
            _image.sprite = _doorSprite;
        else if (tool == ExhibitsConstants.InfoBox.Id)
            _image.sprite = _infoSprite;
        else if (tool == ExhibitsConstants.Cup.Id)
            _image.sprite = _cupSprite;
        else if (tool == ExhibitsConstants.Medal.Id)
            _image.sprite = _medalSprite;
        else if (tool == ExhibitsConstants.Video.Id)
            _image.sprite = _videoSprite;
        else if (tool == ExhibitsConstants.Decoration.Id)
            _image.sprite = _decorSprite;
    }
}
