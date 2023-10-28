namespace BHSystem.Web.Core
{
    public interface ILoadingCore //xoay
    {
        event Action OnShow;
        event Action OnHide;
        public void Show();
        public void Hide();
    }

    public class LoadingCore : ILoadingCore
    {
        event Action? OnShowEvent;
        event Action? OnHideEvent;

        object objectLock = new Object();

        event Action ILoadingCore.OnShow
        {
            add
            {
                lock (objectLock)
                {
                    OnShowEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    OnShowEvent -= value;
                }
            }
        }

        event Action ILoadingCore.OnHide
        {
            add
            {
                lock (objectLock)
                {
                    OnHideEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    OnHideEvent -= value;
                }
            }
        }

        public void Show()
        {
            OnShowEvent?.Invoke();
        }

        public void Hide()
        {
            OnHideEvent?.Invoke();
        }
    }
}