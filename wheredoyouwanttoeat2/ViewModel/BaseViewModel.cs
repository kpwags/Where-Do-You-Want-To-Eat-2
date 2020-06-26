namespace wheredoyouwanttoeat2.ViewModel
{
    public class BaseViewModel
    {
        public string ErrorMessage { get; set; } = string.Empty;

        public string WarningMessage { get; set; } = string.Empty;

        public string SuccessMessage { get; set; } = string.Empty;

        public void ClearMessages()
        {
            ErrorMessage = string.Empty;
            WarningMessage = string.Empty;
            SuccessMessage = string.Empty;
        }
    }
}