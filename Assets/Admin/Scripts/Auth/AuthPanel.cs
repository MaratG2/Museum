using Admin.Utility;
using UnityEngine;

namespace Admin.Auth
{
    /// <summary>
    /// Устанавливает полноэкранный режим на старте, а также включает панель авторизации.
    /// </summary>
    /// <remarks>
    /// Либо пропускает панель авторизации при выключенном флаге _isAuthEnabled.
    /// </remarks>
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