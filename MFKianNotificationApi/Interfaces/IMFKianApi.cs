﻿using MFKianNotificationApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKianNotificationApi.Interfaces
{
    public interface IMFKianApi
    {
        AppModel ApplicationSetting { get; }
        Task SendNotification(List<TasksModel> dataModel, NotificationFilterModel filterModel);
        bool SetApiSetting(AppModel appModel);
        List<UserModel> GetSingleRow(RequestModel model);
        void SendWellComeNotification();
        List<TasksModel> GetMultipuleRows(RequestModel model);
        void SendErrorNotification(string text);
        bool GetApiSetting();
    }
}
