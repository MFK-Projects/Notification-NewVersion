using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;

namespace MFKIanApi.Models
{
    public class NotificationCreationModel
    {
        public string Titel { get; set; }
        public string[] Text { get; set; }
        public ToastScenario ToastScenario { get; set; } = ToastScenario.Default;
        public ToastDuration ToastDuration { get; set; } = ToastDuration.Short;
        public List<ToastButton> Button { get; set; }
        public string TaskUrl { get; set; }

    }


    public class UserModel
    {
        public string Domainname { get; set; }
        public string Fullname { get; set; }
        public int Identityid { get; set; }
        public string Systemuserid { get; set; }
        public string Ownerid { get; set; }
    }


}
