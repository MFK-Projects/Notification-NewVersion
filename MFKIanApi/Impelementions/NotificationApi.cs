using MFKIanApi.Exceptions;
using MFKIanApi.Interfaces;
using MFKIanApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MFKIanApi.Impelementions
{
    public class NotificationApi : INotificationApi
    {

        private bool _disposed = false;
        private ILogger<NotificationApi> _logger;
        private ApplicationSetting _applicationSetting;
        public ApplicationSetting ApplicationSettings
        {
            get
            {
                if (_applicationSetting == null)
                    throw new NullReferenceException($"{nameof(_applicationSetting)} variable is can not be null");

                return _applicationSetting;
            }
        }

        public NotificationApi(ILogger<NotificationApi> logger)
        {
            _logger = logger;
        }


        public void SetApplicationSetting(ApplicationSetting settingModel)
        {
            try
            {
                var path = GetSettingFilePath();

                if (settingModel == null)
                    throw new ArgumentNullException($"{typeof(ApplicationSetting).Name} is null while passing to the mehtod.");
                if (!System.IO.File.Exists(path))
                    throw new CreateFileException("setting file can not created for the application", null);

                var jsonModel = JsonConvert.SerializeObject(settingModel);

                File.WriteAllText(path, jsonModel, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while excuting this method {nameof(SetApplicationSetting)} error message :{ex.Message} \n inner Exception : {ex.InnerException?.Message}");
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public Task SendNotification(List<TasksModel> taskModels, NotificationFilterModel filterMode)
        {
            foreach (var item in NotificationFilters(taskModels))
            {
                ToastCreator(new NotificationCreationModel
                {

                    Button = null,
                    TaskUrl = taskUrlBuilder(item.activityid),
                    Text = new string[] { item.subject, ApplicationSettings.NotificationReqularMessage ?? string.Empty, item.RemainingTime.ToString() },
                    Titel = item.subject,
                    ToastDuration = ApplicationSettings.ToastDuration,
                    ToastScenario = ApplicationSettings.ToastScenario
                });
            }

            return Task.CompletedTask;
        }


        public Task SendNotification(NotificationCreationModel creationModel)
        {
            ToastCreator(new NotificationCreationModel
            {

                Button = creationModel.Button,
                TaskUrl =creationModel.TaskUrl,
                Text = creationModel.Text,
                Titel = creationModel.Titel,
                ToastDuration = creationModel.ToastDuration,
                ToastScenario = creationModel.ToastScenario
            });
            return Task.CompletedTask;
        }


        /// <summary>
        /// Create url to for Toast Notification
        /// </summary>
        /// <param name="crmTaskUrl"></param>
        /// <returns></returns>s
        /// <exception cref="ArgumentNullException">return NullArgumentException</exception>
        private string taskUrlBuilder(string entityId)
        {

            if (string.IsNullOrEmpty(ApplicationSettings.BaseTasksUrl) || string.IsNullOrEmpty(ApplicationSettings.TaskEntityName))
                throw new TaskUrlBuilderException($"{nameof(taskUrlBuilder)} is faild for sending null paramter (basetaskurl | taskEntityname) is null or empty", null);
            else if (string.IsNullOrEmpty(entityId))
                throw new TaskUrlBuilderException($"{nameof(taskUrlBuilder)} is faild for sending null paramter (activityId) is null or empty", null);

            return ApplicationSettings.BaseTasksUrl + ApplicationSettings.TaskEntityName + "&id={" + entityId + "}&pagetype=entityrecord#";
        }
        private void ToastCreator(NotificationCreationModel creationModel)
        {
            if (creationModel == null)
                throw new ArgumentNullException($"{typeof(NotificationCreationModel)} is null while passing it to Toast Creation Filter");

            var toast = new ToastContentBuilder();

            if (!string.IsNullOrEmpty(creationModel.Titel))
                toast.AddHeader(string.Empty, creationModel.Titel, string.Empty);

            if (creationModel.Text.Length > 0)
                foreach (var item in creationModel.Text)
                    toast.AddText(item + System.Environment.NewLine);

            if (creationModel.Button != null)
                if (creationModel.Button.Count > 0)
                    foreach (var button in creationModel.Button)
                        toast.AddButton(button);


            if (!string.IsNullOrEmpty(creationModel.TaskUrl))
                toast.SetProtocolActivation(new System.Uri(creationModel.TaskUrl));


            toast.Show();
            Thread.Sleep(3000);

            toast = null;
        }

        private static string GetSettingFilePath()
        {
            try
            {
                var filepath = Environment.CurrentDirectory + @"\settingApp.json";

                if (System.IO.File.Exists(filepath))
                    return filepath;

                _ = System.IO.File.Create(filepath);

                return filepath;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private List<TasksModel> NotificationFilters(List<TasksModel> data)
        {
            List<TasksModel> model = new();

            foreach (var task in data)
            {

                if (CheckExpiredTasks(task.New_task_status, task.Prioritycode) && task.new_remaining_days == -1)
                {
                    model.Add(task);
                    continue;
                }
                else if (CheckHighPriorityTaks(task.New_task_status, task.Prioritycode) && task.new_remaining_days < 4)
                {
                    model.Add(task);
                    continue;
                }
                else if (task.Prioritycode == 3 && task.new_remaining_days < 5 || task.new_remaining_days < -4) /// check if the task is high priority and task remainin day gater than -3 and less than 3
                {
                    model.Add(task);
                    continue;
                }
                else if (CheckTaskStatus(task.New_task_status) && CheckTaskType(task.New_task_type))
                    if (task.new_remaining_days > 0 && task.new_remaining_days <= ApplicationSettings.MaxDaysCheck)
                    {
                        if (BoostsTaskPriority(task.new_remaining_days, task.Prioritycode))
                        {
                            model.Add(task);
                            continue;
                        }
                        else if (task.Prioritycode == 1 && task.new_remained_time_hour <= 2)
                        {
                            model.Add(task);
                            continue;
                        }
                        else if (task.Prioritycode == 2 && task.new_remained_time_hour < 8) { model.Add(task); continue; }
                    }

            }
            return data;

        }

        protected virtual void Dispose(bool dispose)
        {
            if (_disposed) return;
            if (dispose)
            {
                _logger.LogInformation("this object is disposed");
            }

            _disposed = true;
        }


        #region Check Tasks Status Logic

        /// <summary>
        /// check for the task status is there is no filter all the task can be notified
        /// </summary>
        /// <param name="taskStatus"></param>
        /// <returns>return true for the no filter and task contains the filters</returns>
        private bool CheckTaskStatus(long taskStatus)
        {
            if (ApplicationSettings.TaskStatus.Length == 0)
                return true;

            foreach (var status in ApplicationSettings.TaskStatus)
                if (taskStatus == status)
                    return true;


            return false;
        }


        /// <summary>
        /// check for the task type is there is no filter all the task can be notified
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns>return true for the no filter and task contains the filters</returns>
        private bool CheckTaskType(long taskType)
        {
            if (ApplicationSettings.TasksType.Length == 0)
                return true;

            foreach (var type in ApplicationSettings.TasksType)
                if (taskType == type)
                    return true;


            return false;
        }

        private bool BoostsTaskPriority(int remaininghour, int taskPriority)
        {
            if (remaininghour > 0 && remaininghour - ApplicationSettings.HourCheck <= 3 && taskPriority == 2) /// check for the remaining hour for the boost the task priority
                return true;

            return false;

        }

        private bool CheckExpiredTasks(long taskStatus, int priorityCode)
        {
            if (priorityCode == 3 && taskStatus == ApplicationSettings.ExpiredTaskStatus)
                return true;

            return false;
        }

        private bool CheckHighPriorityTaks(long taskStatus, int priorityCode)
        {
            if (priorityCode == 3)
                for (int i = 0; i < ApplicationSettings.TaskStatus.Length; i++)
                    if (taskStatus == ApplicationSettings.TaskStatus[i])
                        return true;

            return false;
        }


        #endregion
    }
}
