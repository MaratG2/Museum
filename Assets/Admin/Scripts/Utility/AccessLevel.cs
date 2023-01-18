namespace Admin.Utility
{
    /// <summary>
    /// Перечисление, описывающее роли, возможные у пользователя.
    /// </summary>
    public enum AccessLevel
    {
        Registered,
        Guest,
        Editor,
        SuperEditor,
        Administrator
    }
}