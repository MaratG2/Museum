using TMPro;

namespace Admin.Auth
{
    /// <summary>
    /// Интерфейс, содержащий методы LogGood, LogBad для логгирования хорошего или плохого сообщения.
    /// </summary>
    public interface ILoggable
    {
        void LogGood(TMP_Text textUI, string message);
        void LogBad(TMP_Text textUI, string message);
    }
}