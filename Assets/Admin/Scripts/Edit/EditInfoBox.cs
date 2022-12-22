using System.Collections;
using System.Collections.Generic;
using Admin.Utility;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;

namespace Admin.Edit
{
    public class EditInfoBox : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _uiGroup;
        [SerializeField] private InfoController _infoController;
        [SerializeField] private TMP_InputField _infoBoxName;
        
        public void ShowMedia(HallContent hallContent)
        {
            _uiGroup.SetActive(true);
            _infoBoxName.text = hallContent.title;
            _infoController.Setup(hallContent.image_desc);
        }
    }
}