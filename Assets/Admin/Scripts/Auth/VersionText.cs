using TMPro;
using UnityEngine;

namespace Admin.Auth
{
    /// <summary>
    /// На старте приложения отображает версию приложения в виде текста в интерфейсе.
    /// </summary>
    public class VersionText : MonoBehaviour
    {
        private TextMeshProUGUI _versionText;

        private void Awake()
        {
            _versionText = GetComponent<TextMeshProUGUI>();
            _versionText.text = "v." + Application.version;
        }
    }
}