using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKianNotificationApi.Models
{
    public class RootModel<TEntity> where TEntity : class
    {
        public List<TEntity> Value { get; set; }
    }

    public class UserModel
    {
        public string Domainname { get; set; }
        public string Fullname { get; set; }
        public int Identityid { get; set; }
        public string Systemuserid { get; set; }
        public string Ownerid { get; set; }
    }


    public class ApiSettignModel
    {
        public string new_entity_name { get; set; }
        public string new_notification_message { get; set; }
        public string new_notification_systemid { get; set; }
        public double new_setting_timer { get; set; }
        public string new_task_status { get; set; }
        public string new_task_types { get; set; }
        public double new_time_awaited { get; set; }
    }
    public class TasksModel
    {
        public string subject { get; set; }
        public int Prioritycode { get; set; }
        public long New_task_status { get; set; }
        public long New_task_type { get; set; }
        public string _Ownerid_value { get; set; }
        public int new_remained_time_hour { get; set; }
        public int new_remaining_days { get; set; }
        public string activityid { get; set; }
        public string _new_date_deadline_value { get; set; }

        [JsonIgnore]
        public DateTime? RemainingTime
        {
            get
            {
                if (new_remained_time_hour > 0)
                    return DateTime.Now.AddHours(new_remained_time_hour);
                else
                    return null;
            }
        }
    }

}
