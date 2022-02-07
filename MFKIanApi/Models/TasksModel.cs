using Newtonsoft.Json;
using System;

namespace MFKIanApi.Models
{
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
