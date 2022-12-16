using Admin.Utility;
using UnityEngine;

namespace Admin.Auth
{
    public class AuthPanel : MonoBehaviour
    {
        [SerializeField] private bool _isAuthEnabled = true;
        private PanelChanger _panelChanger;

        private void Awake()
        {
            _panelChanger = FindObjectOfType<PanelChanger>();
            Screen.fullScreen = true;
        }

        private void Start()
        {
            if (_isAuthEnabled)
                _panelChanger.MoveToCanvasPanel(Panel.Auth);
            else
                _panelChanger.MoveToCanvasPanel(Panel.View);
        }
    }
}