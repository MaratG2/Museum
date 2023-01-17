using UnityEngine;

#pragma warning disable 0649
namespace InProject
{
    public class ViewObject : MonoBehaviour, IInteractive
    {
        [SerializeField]
        public string Name;
        private bool _isActivate;
        public string Title { get; set; }

        public void Interact()
        {
            if (!_isActivate)
            {
                Open();
                _isActivate = true;
            }
            else
            {
                Close();
                _isActivate = false;
            }
        }
        void Open()
        {
            State.View(true);
            InteractiveLabel.Instance.ChangeTextLabel(Name);
            ActiveObjectView.Instance.CreateObj(gameObject);
        }
        void Close()
        {        
            State.View(false);
            InteractiveLabel.Instance.SetDefaultText();
            ActiveObjectView.Instance.DestroyObj();
        }
    }
}
#pragma warning restore 0649