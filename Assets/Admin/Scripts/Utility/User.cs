namespace Admin.Utility
{
    /// <summary>
    /// Структура данных, описывающая пользователя. Содержит в себе AccessLevel.
    /// </summary>
    public struct User
    {
        public int uid;
        public string name;
        public string email;
        public string password;
        public AccessLevel access_level;
    }
}