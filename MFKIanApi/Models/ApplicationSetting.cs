using Microsoft.Toolkit.Uwp.Notifications;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKIanApi.Models
{
    public class ApplicationSetting
    {

        /// <summary>
        /// Credential Setting
        /// </summary>
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double TaskDurationTime { get; set; }
        public string NotificationReqularMessage { get; set; }
        public string BaseApiUrl { get; set; }
        public long[] TasksType { get; set; }
        public long[] TaskStatus { get; set; }
        public long ExpiredTaskStatus { get; set; }
        public byte HourCheck { get; set; }
        public byte MaxDaysCheck { get; set; }
        public string NotificationSystemId { get; set; }

        /// <summary>
        /// Toast Setting
        /// </summary>
        public ToastScenario ToastScenario { get; set; }
        public ToastDuration ToastDuration { get; set; }


        /// <summary>
        /// task Setting
        /// </summary>

        public string BaseTasksUrl { get; set; }
        public string TaskEntityName { get; set; }
    }
}
