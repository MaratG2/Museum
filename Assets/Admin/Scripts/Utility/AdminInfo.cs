using System;
using System.Text;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Admin.Utility
{
    /// <summary>
    /// Отвечает за отображение информации для авторизации в панели администратора.
    /// </summary>
    public class AdminInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private CanvasGroup _infoGroup;
        [SerializeField] private TMP_InputField _infoText;
        [SerializeField] private string _encryptedInfoText;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _infoGroup.SetActive(true);
            _infoText.text = GetAdminInfo();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _infoGroup.SetActive(false);
        }

        private string GetAdminInfo()
        {
            byte[] decodedBytes = Convert.FromBase64String(_encryptedInfoText);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);
            return decodedText;
        }
    }
}