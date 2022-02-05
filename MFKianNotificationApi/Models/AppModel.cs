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
        public double TimeAwaite { get; set; }
        public string NotificationReqularMessage { get; set; }
        public string BaseUrl { get; set; }
        public string EntityName { get; set; } = "tasks";
    }

    public class NotificationFilterModel
    {
        public int HourCheck { get;set; }
        public int DayCheck { get; set; }
        public long[] TaskType { get; set; }
        public long[] NTasksStatus { get; set; }
    }

}
