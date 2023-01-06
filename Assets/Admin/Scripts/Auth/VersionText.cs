using TMPro;
using UnityEngine;

namespace Admin.Auth
{
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