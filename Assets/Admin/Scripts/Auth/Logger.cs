using TMPro;
using UnityEngine;

namespace Admin.Auth
{
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