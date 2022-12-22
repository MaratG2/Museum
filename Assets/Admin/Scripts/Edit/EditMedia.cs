using System.Collections;
using System.Collections.Generic;
using Admin.Utility;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;

namespace Admin.Edit
{
    public class EditMedia : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _uiGroup;
        [SerializeField] private TextMeshProUGUI _propertiesHeader;
        [SerializeField] private TMP_InputField _propertiesName;
        [SerializeField] private TMP_InputField _propertiesUrl;
        [SerializeField] private TMP_InputField _propertiesDesc;
        
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