namespace MFKIanApi.Models
{
    public class NotificationFilterModel
    {
        public int HourCheck { get; set; }
        public int DayCheck { get; set; }
        public long[] TaskType { get; set; }
        public long[] NTasksStatus { get; set; }
    }


}
