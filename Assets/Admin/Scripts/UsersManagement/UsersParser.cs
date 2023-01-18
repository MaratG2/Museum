using System;
using System.Collections;
using System.Collections.Generic;
using Admin.PHP;
using Admin.Utility;
using UnityEngine;

namespace Admin.UsersManagement
{
    /// <summary>
    /// Отвечает за получение всех пользователей из БД, кеширует их в список пользователей (_cachedUsers).
    /// </summary>
    public class UsersParser : MonoBehaviour
    {
        private Action<string> OnResponseCallback;
        private string _response;
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private UsersSpawner _usersSpawner;
        private List<User> _cachedUsers = new();

        private void Start()
        {
            _usersSpawner = GetComponent<UsersSpawner>();
        }

        private void OnEnable()
        {
            OnResponseCallback += response => _response = response;
        }

        private void OnDisable()
        {
            OnResponseCallback -= response => _response = response;
        }

        public void ParseUsers()
        {
            _cachedUsers = new();
            StartCoroutine(GetAllUsersQuery());
        }

        private void DisplayUsers()
        {
            _usersSpawner.RefreshUserList(_cachedUsers);
        }

        private IEnumerator GetAllUsersQuery()
        {
            string phpFileName = "get_all_users.php";
            yield return _queriesToPhp.GetRequest(phpFileName, OnResponseCallback);
            if (string.IsNullOrWhiteSpace(_response))
                yield break;

            var users = _response.Split(";");
            foreach (var user in users)
            {
                if (string.IsNullOrWhiteSpace(user))
                    continue;

                User newUser = new User();
                var userOptions = user.Split("|");
                newUser.uid = Int32.Parse(userOptions[0]);
                newUser.name = userOptions[1];
                newUser.email = userOptions[2];
                newUser.password = userOptions[3];
                newUser.access_level = (AccessLevel)Int32.Parse(userOptions[4]);
                _cachedUsers.Add(newUser);
            }

            DisplayUsers();
        }
    }
}