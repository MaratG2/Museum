using Admin.Utility;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечает за рисование выбранной иконки из палитры на 2D плане зала, а также установление начального состояния нарисованного предмета музея.
    /// </summary>
    public class EditMedia : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _uiGroup;
        [SerializeField] private TextMeshProUGUI _propertiesHeader;
        [SerializeField] private TMP_InputField _propertiesName;
        [SerializeField] private TMP_InputField _propertiesUrl;
        [SerializeField] private TMP_InputField _propertiesDesc;
        public string Title => _propertiesName.text;
        public string Url => _propertiesUrl.text;
        public string Desc => _propertiesDesc.text;

        public void ShowMedia(HallContent hallContent, bool isPhoto)
        {
            _uiGroup.SetActive(true);
            string mediaName = isPhoto ? "фото" : "видео";
            _propertiesHeader.text = $"Редактирование {mediaName}";
            _propertiesName.text = hallContent.title;
            _propertiesUrl.text = hallContent.image_url;
            _propertiesDesc.text = hallContent.image_desc;
        }
    }
}