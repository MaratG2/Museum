using System.Runtime.CompilerServices;
using MaratG2.Extensions;
using UnityEngine;

namespace Admin.Utility
{
    /// <summary>
    /// Перечисление Panel, отвечающее за UI-панели в приложении.
    /// </summary>
    public enum Panel
    {
        Auth,
        View,
        Edit,
        New,
        User
    }
    /// <summary>
    /// Переключает UI-панели в приложении.
    /// </summary>
    public class PanelChanger : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _authGroup, _viewGroup, _editGroup, _newGroup, _userGroup;

        public void MoveToAuthGroup()
        {
            MoveToCanvasPanel(Panel.Auth);
        }
        public void MoveToViewGroup()
        {
            MoveToCanvasPanel(Panel.View);
        }
        public void MoveToEditGroup()
        {
            MoveToCanvasPanel(Panel.Edit);
        }
        public void MoveToNewGroup()
        {
            MoveToCanvasPanel(Panel.New);
        }
        public void MoveToUserGroup()
        {
            MoveToCanvasPanel(Panel.User);
        }
        
        public void MoveToCanvasPanel(Panel panel)
        {
            TurnAllCanvasGroupsOff();
            switch (panel)
            {
                case Panel.Auth:
                    _authGroup.SetActive(true);
                    break;
                case Panel.View:
                    _viewGroup.SetActive(true);
                    break;
                case Panel.Edit:
                    _editGroup.SetActive(true);
                    break;
                case Panel.New:
                    _newGroup.SetActive(true);
                    break;
                case Panel.User:
                    _userGroup.SetActive(true);
                    break;
                default:
                    Debug.LogError(new SwitchExpressionException(panel.ToString()));
                    break;
            }
        }

        private void TurnAllCanvasGroupsOff()
        {
            _authGroup.SetActive(false);
            _viewGroup.SetActive(false);
            _editGroup.SetActive(false);
            _newGroup.SetActive(false);
            _userGroup.SetActive(false);
        }
    }
}