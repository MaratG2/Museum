namespace Admin.Auth
{
    /// <summary>
    /// Интерфейс, содержащий методы Empty (очистить содержимое поля), Trim (обрезать поле с двух сторон).
    /// </summary>
    public interface IFieldsProvider
    {
        void Empty();
        void Trim();
    }
}