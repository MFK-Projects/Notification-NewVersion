using MFKIanApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKIanApi.Interfaces
{
    public interface INotificationApi:IDisposable
    {
        void SetApplicationSetting(ApplicationSetting settingModel);
        Task SendNotification(List<TasksModel> taskModels, NotificationFilterModel filterMode);
    }
}
