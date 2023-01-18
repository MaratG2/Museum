using UnityEngine;

namespace Admin.Utility
{
    /// <summary>
    /// Отвечает за взаимодействие с браузером, например открытие ссылок в новой вкладке.
    /// </summary>
    public class WebPage : MonoBehaviour
    {
        public void OpenPage(string url)
        {
            Application.OpenURL(url);
        }
    }
}