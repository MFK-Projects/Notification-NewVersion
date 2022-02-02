using MFKianNotificationApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKianNotificationApi.Interfaces
{
    public interface IMFKianApi
    {
        Task<int> SendNotification(NotificationSettingModel setting);
        void SetApplicationSetting();
        RootModel<UserModel> GetUserData(RequestModel model);
        RootModel<TasksModel> GetUserTasks(RequestModel model);
    }
}
