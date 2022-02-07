using MFKianNotificationApi.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MFKianNotificationApi.Models
{
    public class AppModel
    {
        public CredentialModel CredentialModel { get; set; }
        public double TimeAwaite { get; set; } = 30;
        public string NotificationReqularMessage { get; set; }
        public string BaseUrl { get; set; } = @"http://80.210.16.4:8585/MFkian/api/data/v9.0/";
        public string EntityName { get; set; } = "tasks";
        public string LogPath { get; set; }
        public string TasksType { get; set; }
        public string TaskStatus { get; set; }
        public double SettingTimer { get; set; }
        public string NotificationSystemId { get; set; }
    }

    public class NotificationFilterModel
    {
        public int HourCheck { get; set; }
        public int DayCheck { get; set; }
        public long[] TaskType { get; set; }
        public long[] NTasksStatus { get; set; }
    }




    [XmlRootAttribute(elementName: "Setting", IsNullable = false)]
    public class ApplicationSettingModel
    {
        public string Path { get; set; }
        public byte SendCount { get; set; }
        public byte TimeCount { get; set; }
        public AppSettingCredentialModel CredentialModel { get; set; }
        public byte TaskPriorityTime { get; set; }

    }

    [XmlRootAttribute(elementName: "Setting", IsNullable = false)]
    public class AppSettingCredentialModel
    {
        public string Domain { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }

}
