using System;
using System.Collections;
using System.Collections.Generic;
using Admin.Utility;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;

namespace Admin.Edit
{
    public class EditDecoration : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _uiGroup;
        [SerializeField] private TMP_Dropdown _decorationsDropdown;
        
        public void ShowMedia(HallContent hallContent)
        {
            _uiGroup.SetActive(true);
            _decorationsDropdown.value = Int32.Parse(hallContent.title);
        }
    }
}