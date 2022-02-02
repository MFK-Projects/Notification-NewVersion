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
    public class TasksModel
    {
        public string subject { get; set; }
        public int prioritycode { get; set; }
        public int new_task_status { get; set; }
        public int new_task_type { get; set; }
        public string _ownerid_value { get; set; }
        public int new_remained_time_hour { get; set; }
        public int new_remaining_days { get; set; }
        public string activityid { get; set; }

        [JsonIgnore]
        public DateTime? RemainingTime
        {
            get
            {
                if (new_remained_time_hour > 0)
                    return DateTime.Now.AddHours(new_remained_time_hour);
                else
                    return null ;
            }
        }
    }

}
