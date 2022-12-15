using TMPro;

namespace Admin.Auth
{
    public interface ILoggable
    {
        void LogGood(TMP_Text textUI, string message);
        void LogBad(TMP_Text textUI, string message);
    }
}