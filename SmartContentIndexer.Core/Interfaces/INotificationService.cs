using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Interfaces
{
    public interface INotificationService
    {
        Task ShowNotificationAsync(string title, string message, NotificationType type = NotificationType.Info);
        Task ShowProgressNotificationAsync(string title, int progress, int total);
        Task HideProgressNotificationAsync();

        // Events
        event Action<string, string, NotificationType> NotificationRequested;
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
}
