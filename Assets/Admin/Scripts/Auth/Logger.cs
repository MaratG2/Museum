using System.Collections;
using System.Collections.Generic;
using Admin.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.Auth
{
    public class Logger : MonoBehaviour, ILoggable
    {
        public void LogGood(TMP_Text textUI, string message)
        {
            textUI.text = message;
            textUI.color = Color.green;
        }
        public void LogBad(TMP_Text textUI, string message)
        {
            textUI.text = message;
            textUI.color = Color.red;
        }
    }
}