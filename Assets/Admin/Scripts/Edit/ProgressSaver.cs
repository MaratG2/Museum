using TMPro;
using UnityEngine;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечает за визуальное отображение в тексте прогресса сохранения настроек зала.
    /// </summary>
    public class ProgressSaver : MonoBehaviour
    {
        [SerializeField] private string _progressSave = "Прогресс сохранения:";
        [SerializeField] private string _progressDelete = "Прогресс удаления:";
        [SerializeField] private TextMeshProUGUI _progressText;

        public void Clear()
        {
            _progressText.text = "";
        }

        public void UpdateProgress(int current, int max, bool isSave)
        {
            _progressText.text = (isSave? _progressSave : _progressDelete) + $" {current.ToString()}/{max.ToString()}";
        }

        public void UpdateCustomMessage(string message)
        {
            _progressText.text = message;
        }
    }
}