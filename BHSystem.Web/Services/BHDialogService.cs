namespace BHSystem.Web.Services
{
    public class BHDialogService
    {
        public event Action<bool>? OnShow;
        public void ShowDialog(bool pIsShow = true) => OnShow!.Invoke(pIsShow);
    }
}
