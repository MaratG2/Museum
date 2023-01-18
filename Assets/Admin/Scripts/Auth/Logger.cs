using TMPro;
using UnityEngine;

namespace Admin.Auth
{
    /// <summary>
    /// Контракт с интерфейсом ILoggable, дает в использование публичные методы для логгирования в пользовательском интерфейсе.
    /// </summary>
    public class Logger : MonoBehaviour, ILoggable
    {
        [SerializeField] private Color _goodColor = Color.green;
        [SerializeField] private Color _badColor = Color.red;
        public void LogGood(TMP_Text textUI, string message)
        {
            textUI.text = message;
            textUI.color = _goodColor;
        }
        public void LogBad(TMP_Text textUI, string message)
        {
            textUI.text = message;
            textUI.color = _badColor;
        }
    }
}