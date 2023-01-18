using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечает за часть информации для информационного стенда – проверка ввода, сохранение, удаление.
    /// </summary>
    /// <remarks>
    /// Также содержит структуру InfoPartData, описывающую ссылка на картинку и описание.
    /// </remarks>
    public class InfoPart : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _urlInput;
        [SerializeField] private TMP_InputField _descInput;
        [SerializeField] private Button _removeButton;
        private bool _isSetup = false;

        [System.Serializable]
        public struct InfoPartData
        {
            public string url;
            public string desc;
        }

        private InfoPartData _infoData;

        public InfoPartData InfoData
        {
            get => _infoData;
            set => _infoData = value;
        }

        private void Awake()
        {
            _removeButton.onClick.AddListener(EndPart);
        }

        private void EndPart()
        {
            GetComponentInParent<InfoController>().InfoPartsChanged(this);
            Destroy(gameObject);
        }

        public void ApplyNewData()
        {
            _isSetup = true;
            _urlInput.text = InfoData.url;
            _descInput.text = InfoData.desc;
            _isSetup = false;
        }

        public void OnInputUpdated()
        {
            if (_isSetup)
                return;

            InfoPartData newData = new InfoPartData();
            newData.url = _urlInput.text;
            newData.desc = _descInput.text;
            InfoData = newData;
        }
    }
}