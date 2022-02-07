
using MFKianNotificationApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MFKianNotificationApi.Interfaces
{
    public interface IMfkianApiSencondVersion:IDisposable
    {
        AppModel ApplicationSetting { get; }
        Task SendNotification(List<TasksModel> dataModel, NotificationFilterModel filterModel);
        bool SetApiSetting();
        List<UserModel> GetSingleRow(RequestModel model);
        void SendDefaultNotification(NotificationCreationModel model);
        List<TasksModel> GetMultipuleRows(RequestModel model);
        bool GetApiSetting();
    }
}
