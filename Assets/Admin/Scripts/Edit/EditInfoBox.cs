using Admin.Utility;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечает за отображение настроек информационного стенда на пользовательском интерфейсе.
    /// </summary>
    public class EditInfoBox : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _uiGroup;
        [SerializeField] private InfoController _infoController;
        [SerializeField] private TMP_InputField _infoBoxName;
        public InfoController InfoController => _infoController;
        public string Title => _infoBoxName.text;
        
        public void ShowMedia(HallContent hallContent)
        {
            _uiGroup.SetActive(true);
            _infoBoxName.text = hallContent.title;
            _infoController.Setup(hallContent.image_desc);
        }
    }
}