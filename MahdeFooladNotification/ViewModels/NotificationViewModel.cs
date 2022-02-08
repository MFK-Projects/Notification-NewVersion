using MFKIanApi.Interfaces;
using MFKIanApi.Models;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;


namespace MahdeFooladNotification.ViewModels
{
    public class NotificationViewModel : ViewModelBase
    {

        private readonly INotificationApi _notificationApi;

        public NotificationViewModel(INotificationApi notificationApi)
        {
            _notificationApi = notificationApi;
            _taskModel = notificationApi.GetMultipuleRow(new RequestModel { Count = 0, RequestEntityName = "tasks", SelectItem = new string[] { "", "", "" } }).FirstOrDefault();
        }


        public ICommand SetSetting { get; }
        public ICommand ChangeTask { get; }

        private TasksModel _taskModel;
        public TasksModel TasksModel
        {
            get
            {
                return _taskModel;
            }
            set
            {
                if (TasksModel.activityid != value.activityid)
                {
                    _taskModel = value;
                    OnPropertyChanged(nameof(TasksModel));
                }
            }

        }

    }
}
