using MFKianNotificationApi.Interfaces;
using MFKianNotificationApi.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MFKianNotificationApi.Impelementions
{
    public class MFKianApi : IMFKianApi
    {

        private string getCurentUserName;
        private int notificationCount = 0;
        public string GetCurentUserName
        {
            get
            {
                var data = Task<string>.Factory.StartNew(() =>
                {

                    return System.DirectoryServices.AccountManagement.UserPrincipal.Current.Name;
                });


                return data.GetAwaiter().GetResult();
            }
        }
        public int NotificationCount => NotificationCount;



        public void SetApplicationSetting()
        {

        }

        public Task<int> SendNotification(NotificationSettingModel setting)
        {
            ToastCreationFilter(setting);

            return Task.FromResult(notificationCount += 1);
        }

        public RootModel<UserModel> GetUserData(RequestModel model)
        {
            try
            {
                var url = UrlBuilder(model.RequestDataModel);

                var _stringData = SendHttpRequest(model.CredentialModel, url);
                var formatedData = JsonConvert.DeserializeObject<RootModel<UserModel>>(_stringData);


                return formatedData;
            }
            catch
            {
                throw new Exception("GetUserData thrown an Exception");
            }
        }

        public RootModel<TasksModel> GetUserTasks(RequestModel model)
        {
            try
            {

                var url = UrlBuilder(model.RequestDataModel);


                var _stringData = SendHttpRequest(model.CredentialModel, url);

                var formatedData = JsonConvert.DeserializeObject<RootModel<TasksModel>>(_stringData);

                return formatedData;
            }
            catch
            {
                throw new Exception("GetTask Data Thrown an Exception");
            }
        }


        private string UrlBuilder(RequestDataModel model)
        {
            if (model == null)
                return string.Empty;


            var url = model.BaseUrl + model.Url + model.EnttiyName;


            if (model.SelectItem.Length > 0)
            {
                url += @"?$select=";
                for (int i = 0; i < model.SelectItem.Length; i++)
                    if (i == model.SelectItem.Length - 1)
                        url += model.SelectItem[i];
                    else
                        url += model.SelectItem[i] + ",";
            }

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
        private void ToastCreationFilter(NotificationSettingModel setting)
        {
            var toast = new ToastContentBuilder();

            if (!string.IsNullOrEmpty(setting.Titel))
                toast.AddHeader(string.Empty, setting.Titel, string.Empty);

            if (setting.Text.Length > 0)
                foreach (var item in setting.Text)
                    toast.AddText(item + System.Environment.NewLine);

            if (setting.Button.Count > 0)
                foreach (var button in setting.Button)
                    toast.AddButton(button);

            if (!string.IsNullOrEmpty(setting.Url))
                toast.SetProtocolActivation(new System.Uri(setting.Url));

            toast.Show();
        }
        private string SendHttpRequest(CredentialModel model, string Url)
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


    }
}
