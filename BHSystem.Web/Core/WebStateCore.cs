namespace BHSystem.Web.Core
{
    public interface IWebStateCore //trạng thái web
    {

        event Action OnChange;
        /// <summary>
        /// lấy tên acction
        /// </summary>
        /// <returns></returns>
        public string? ActionName();
        /// <summary>
        /// lấy message
        /// </summary>
        /// <returns></returns>
        public string? Message();
        /// <summary>
        /// tạo 1 action
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="mes"></param>
        public void SetAction(string actionName, string mes);


    }
    public class WebStateCore : IWebStateCore
    {
        public string? NameofAction { get; private set; }

        /// <summary>
        /// lời nhắn
        /// </summary>
        public string? MessageofAction { get; private set; }

        event Action? OnChangeEvent;
        object objectLock = new Object();

        event Action IWebStateCore.OnChange
        {
            add
            {
                lock (objectLock)
                {
                    OnChangeEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    OnChangeEvent -= value;
                }
            }
        }

        public void SetAction(string actionName, string mes)
        {
            NameofAction = actionName;
            MessageofAction = mes;
            OnChangeEvent?.Invoke();
        }

        public string? Message() => MessageofAction;

        public string? ActionName() => NameofAction;
    }
}