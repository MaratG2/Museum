using System.Collections.Generic;
using Admin.Auth;
using Admin.Utility;
using TMPro;
using UnityEngine;

namespace Admin.UsersManagement
{
    /// <summary>
    /// Отвечает за создание всех кешированных пользователей на пользовательском интерфейсе.
    /// </summary>
    public class UsersSpawner : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _usernameText;
        [SerializeField] private UserContent _userPrefab;
        [SerializeField] private RectTransform _spawnParent;

        public void RefreshUserList(List<User> users)
        {
            _usernameText.text = $"Вы авторизованы как: {FindObjectOfType<Login>().CurrentUser.name}";
            if (users == null || users.Count == 0)
                return;
            
            ClearAllUserPrefabs();
            CreateUserPrefabs(users);
        }

        private void ClearAllUserPrefabs()
        {
            for (int i = 0; i < _spawnParent.childCount; i++)
                Destroy(_spawnParent.GetChild(i).gameObject);
        }

        private void CreateUserPrefabs(List<User> users)
        {
            foreach (var user in users)
            {
                if (string.IsNullOrWhiteSpace(user.email))
                    continue;
                
                var newUser = Instantiate(_userPrefab, _spawnParent);
                newUser.Initialize(user);
            }
        }
    }
}