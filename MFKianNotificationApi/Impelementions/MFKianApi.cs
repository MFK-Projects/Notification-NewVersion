using MFKianNotificationApi.Expceptions;
using MFKianNotificationApi.Interfaces;
using MFKianNotificationApi.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MFKianNotificationApi.Impelementions
{
    public class MFKianApi : IMFKianApi
    {
        private AppModel _applicationSetting;
        private bool _disposed = false;
        public AppModel ApplicationSetting
        {
            get
            {
                return _applicationSetting;
            }
        }



        #region publiic Methods


        public void SendErrorNotification(string text)
        {
            new ToastContentBuilder()
                .AddText(text)
                .Show();
        }

        /// <summary>
        /// Send Notification For the Tasks remaind
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="dataModel"></param>
        /// <param name="filterModel"></param>
        /// <param name="baseUrl"></param>
        /// <param name="EntityName"></param>
        /// <returns></returns>
        public Task SendNotification(List<TasksModel> dataModel, NotificationFilterModel filterModel)
        {
            foreach (var item in SetNotificationfilter(filterModel, dataModel))
            {
                ToastCreationFilter(new NotificationCreationModel
                {

                    Button = null,
                    TaskUrl = taskUrlBuilder(new CrmTaskUrl { BaseUrl = ApplicationSetting.BaseUrl, EntityId = item.activityid, EntityName = ApplicationSetting.EntityName }),
                    Text = new string[] { item.subject, ApplicationSetting.NotificationReqularMessage ?? "زمان انجام دادن این تسک محدود می باشد", item.RemainingTime.HasValue.ToString() },
                    Titel = item.subject,
                    ToastDuration = ToastDuration.Short,
                    ToastScenario = ToastScenario.Reminder
                });
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// return Signle Row of data 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<UserModel> GetSingleRow(RequestModel model)
        {
            try
            {
                var url = UrlBuilder(model.RequestDataModel);

                var _stringData = SendHttpRequest(model.CredentialModel, url);
                var formatedData = JsonConvert.DeserializeObject<RootModel<UserModel>>(_stringData);

                if (formatedData.Value.Count > 1)
                    throw new MoreThanOneRecordFindException($" there are more than one record for :{model.RequestDataModel.EnttiyName} with this {url}", null);


                return formatedData.Value;
            }
            catch
            {
                throw new Exception("GetUserData thrown an Exception");
            }
        }

        public List<TasksModel> GetMultipuleRows(RequestModel model)
        {
            try
            {

                var url = UrlBuilder(model.RequestDataModel);


                var _stringData = SendHttpRequest(model.CredentialModel, url);

                var formatedData = JsonConvert.DeserializeObject<RootModel<TasksModel>>(_stringData);

                return formatedData.Value;
            }
            catch
            {
                throw new Exception("GetTask Data Thrown an Exception");
            }
        }
        /// <summary>
        /// Set ApiSetting
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        /// <exception cref="SetApplicationException"></exception>
        public bool SetApiSetting(CredentialModel appModel)
        {
            try
            {
                _applicationSetting.CredentialModel = appModel ?? throw new ArgumentNullException($"{typeof(AppModel)} is null while passign argument");
                return true;
            }
            catch (Exception ex)
            {
                throw new SetApplicationException("erro while setting the application settign see inner exception for detailes", ex);
            }
        }

        /// <summary>
        /// Send WellCome notiification to user...
        /// </summary>
        public void SendWellComeNotification()
        {
            var toast = new ToastContentBuilder();

            toast.AddText("نرم افزار ارسال نوتیفیکشن با موفیت ست شد")
                .SetProtocolActivation(new Uri(@"http://80.210.16.4:8585/MFkian/api/data/v9.0//MFkian/main.aspx?"))
                .Show();


            toast = null;

        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Send Http Request to an Server for getting Api Setting..
        /// </summary>
        public bool GetApiSetting()
        {

            try
            {
                var url = @"http://80.210.16.4:8585/MFkian/api/data/v9.0/" + UrlBuilder(new RequestDataModel { Count = 3, EnttiyName = "new_notification_systems", Filters = null, SelectItem = new string[] { "new_entity_name", "new_notification_message", "new_notification_systemid", "new_setting_timer", "new_task_status", "new_task_types", "new_time_awaited" } });


                var _stringdata = SendHttpRequest(new CredentialModel { Domain = "KIAN", Password = "r", UserName = "a.moradi" }, url);

                var formatedData = JsonConvert.DeserializeObject<RootModel<ApiSettignModel>>(_stringdata).Value.FirstOrDefault();

                if (formatedData == null)
                    throw new NullReferenceException("no entites found ", null);


                _applicationSetting = new AppModel
                {
                    NotificationReqularMessage = formatedData.new_notification_message,
                    EntityName = formatedData.new_entity_name,
                    TaskStatus = formatedData.new_task_status,
                    TasksType = formatedData.new_task_types,
                    TimeAwaite = formatedData.new_time_awaited,
                    SettingTimer = formatedData.new_setting_timer,
                    NotificationSystemId = formatedData.new_notification_systemid,
                    CredentialModel = new CredentialModel
                    {
                        Domain = "KIAN",
                        Password = "r",
                        UserName = "a.moradi"
                    }
                };


                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Get api Setting thrown new exception ", ex);
            }

        }

        /// <summary>
        ///Create Url for Sending http Request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string UrlBuilder(RequestDataModel model)
        {
            if (model == null)
                return string.Empty;


            var url = model.BaseUrl + model.Url + model.EnttiyName;

            if (model.SelectItem == null)
                throw new ArgumentException($"{model.SelectItem} is null while passing into the UrlBuilder");

            if (model.SelectItem.Length > 0)
            {
                url += @"?$select=";

                for (int i = 0; i < model.SelectItem.Length; i++)
                {
                    if (i == model.SelectItem.Length - 1)
                        url += model.SelectItem[i];
                    else
                        url += model.SelectItem[i] + ",";
                }
            }

            if (model.Filters != null)
                if (model.Filters.Count > 0)
                {
                    url += @"&$filter=";
                    foreach (var item in model.Filters)
                        if (item.Type == Enums.RequestDataFilterType.Content)
                            url += "  " + item.Item + " " + item.Key + " '" + item.Value + "'";
                        else if (item.Type == Enums.RequestDataFilterType.UniqIdentitfire || item.Type == Enums.RequestDataFilterType.Number)
                            url += "  " + item.Item + " " + item.Key + " " + item.Value;
                }

            return url;
        }

        /// <summary>
        /// Create url to for Toast Notification
        /// </summary>
        /// <param name="crmTaskUrl"></param>
        /// <returns></returns>s
        /// <exception cref="ArgumentNullException">return NullArgumentException</exception>
        private string taskUrlBuilder(CrmTaskUrl crmTaskUrl)
        {
            if (crmTaskUrl == null)
                throw new ArgumentNullException($"{typeof(CrmTaskUrl)} is null while passing to TaskUrlBuilder....");
            if (string.IsNullOrEmpty(crmTaskUrl.EntityName) || string.IsNullOrEmpty(crmTaskUrl.EntityId))
                throw new ArgumentNullException($"{typeof(CrmTaskUrl)} is null while passing to TaskUrlBuilder.... the EntityName or EntityId is Null");


            return @"http://80.210.16.4:8585/MFkian/main.aspx?etn=" + crmTaskUrl.EntityName + "&id={" + crmTaskUrl.EntityId + "}&pagetype=entityrecord#";
        }

        /// <summary>
        /// Create Dynamic Toast
        /// </summary>
        /// <param name="setting">specified the toast </param>
        /// <exception cref="ArgumentNullException"> return the nullArugmentException</exception>
        private void ToastCreationFilter(NotificationCreationModel setting)
        {

            if (setting == null)
                throw new ArgumentNullException($"{typeof(NotificationCreationModel)} is null while passing it to Toast Creation Filter");

            var toast = new ToastContentBuilder();

            if (!string.IsNullOrEmpty(setting.Titel))
                toast.AddHeader(string.Empty, setting.Titel, string.Empty);

            if (setting.Text.Length > 0)
                foreach (var item in setting.Text)
                    toast.AddText(item + System.Environment.NewLine);

            if (setting.Button != null)
                if (setting.Button.Count > 0)
                    foreach (var button in setting.Button)
                        toast.AddButton(button);


            if (!string.IsNullOrEmpty(setting.TaskUrl))
                toast.SetProtocolActivation(new System.Uri(@"http://crm-srv:8585/" + setting.TaskUrl));


            toast.Show();
            Thread.Sleep(1000);

            toast = null;
        }

        /// <summary>
        /// Sending Http Request With WebClient Class return String Data...
        /// </summary>
        /// <param name="model">NTLM Credential while sendign Request</param>
        /// <param name="Url">url which request sent to..</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string SendHttpRequest(CredentialModel model, string Url)
        {

            var _client = new WebClient();

            try
            {
                _client.Credentials = new NetworkCredential(model.UserName, model.Password, model.Domain);
                _client.Headers.Add("OData-Version", "4.0");

                var data = _client.DownloadString(new Uri(Url));

                return data;
            }
            catch
            {
                throw new Exception("SendHttpRequest Thrown Exception");
            }
            finally
            {
                _client?.Dispose();
            }

        }


        /// <summary>
        /// Set the Notification Filer for Creating ToastNotification
        /// /// </summary>
        /// <param name="filterModel"></param>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        private static List<TasksModel> SetNotificationfilter(NotificationFilterModel filterModel, List<TasksModel> dataModel)
        {
            var data = new List<TasksModel>();

            foreach (var item in dataModel)
                if (CheckNtaskStatus(item.New_task_status, filterModel.NTasksStatus) && CheckTaskType(item.New_task_type, filterModel.TaskType))
                    if (item.new_remaining_days <= filterModel.DayCheck)
                        if (item.new_remained_time_hour > 0 && item.new_remained_time_hour <= filterModel.HourCheck)
                            data.Add(item);

            return data;
        }

        /// <summary>
        /// Checking For the TaskStatue
        /// </summary>
        /// <param name="taskStatus"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static bool CheckNtaskStatus(long taskStatus, long[] filter)
        {
            if (filter.Length == 0)
                return true;

            for (int i = 0; i < filter.Length; i++)
                if (filter[i] == taskStatus)
                    return true;

            return false;
        }


        /// <summary>
        /// Checking For the TaskTye
        /// </summary>
        /// <param name="taskStatus"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static bool CheckTaskType(long taskType, long[] filter)
        {
            if (filter.Length == 0) return true;

            for (int i = 0; i < filter.Length; i++)
                if (filter[i] == taskType)
                    return true;

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (_disposed)
                return;

            if (dispose)
            {
                _applicationSetting = null;

            }
            _disposed = true;
        }
        #endregion
    }
}
