using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKIanApi.Models
{
    public class ApplicationSetting
    {
        public CredentialModel CredentialModel { get; set; }
        public double TaskDurationTime { get; set; }
        public string NotificationReqularMessage { get; set; }
        public string BaseUrl { get; set; }
        public string EntityName { get; set; }
        public string LogPath { get; set; }
        public string TasksType { get; set; }
        public string TaskStatus { get; set; }
        public string NotificationSystemId { get; set; }
        public ToastScenario ToastScenario { get; set; }
        public ToastDuration ToastDuration { get; set; } 
    }


}
