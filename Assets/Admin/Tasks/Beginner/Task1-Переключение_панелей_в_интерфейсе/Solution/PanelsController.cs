using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Task1
{
    public class PanelsController : MonoBehaviour
    {
        [SerializeField, NotNull] private CanvasGroup[] _panels;

        public void ChangePanelTo(CanvasGroup panelToEnable)
        {
            for (int i = 0; i < _panels.Length; i++)
                _panels[i].SetActive(_panels[i] == panelToEnable);
        }
    }
}