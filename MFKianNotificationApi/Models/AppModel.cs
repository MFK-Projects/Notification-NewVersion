using MFKianNotificationApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKianNotificationApi.Models
{
    public class AppModel
    {
        public CredentialModel CredentialModel { get; set; }
        public double TimeAwaite { get; set; } = 30;
        public string NotificationReqularMessage { get; set; }
        public string BaseUrl { get; set; } = @"http://crm-srv:8585/MFkian/api/data/v9.0/";
        public string EntityName { get; set; } = "tasks";
        public string LogPath { get; set; } 
        public string TasksType { get; set; }
        public string TaskStatus { get; set; }
        public double SettingTimer { get; set; }
        public string NotificationSystemId { get; set; }
    }

    public class NotificationFilterModel
    {
        public int HourCheck { get;set; }
        public int DayCheck { get; set; }
        public long[] TaskType { get; set; }
        public long[] NTasksStatus { get; set; }
    }

}
