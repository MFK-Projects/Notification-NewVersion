using MFKianNotificationApi.Interfaces;
using MFKianNotificationApi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MFKianNotificationApi.Impelementions
{
    public class MFKianImpelemention : IMfkianApiSencondVersion
    {

        private readonly ILogger<MFKianImpelemention> _logger;
        private bool _disposed = false;
        private ApplicationSettingModel _applicationSetting;

        public MFKianImpelemention(ILogger<MFKianImpelemention> logger)
        {
            _logger = logger;
        }


        public ApplicationSettingModel ApplicationSetting
        {
            get
            {
                if (_applicationSetting == null)
                {
                    _logger.LogError($"{typeof(ApplicationSettingModel)} can not created or get data from the setting.xml");
                    throw new NullReferenceException("the application Setting is null");
                }

                return _applicationSetting;
            }
        }



        public List<TasksModel> GetMultipuleRows(RequestModel model)
        {
            throw new NotImplementedException();
        }

        public List<UserModel> GetSingleRow(RequestModel model)
        {
            throw new NotImplementedException();
        }

        public void SendDefaultNotification(NotificationCreationModel model)
        {
            throw new NotImplementedException();
        }

        public Task SendNotification(List<TasksModel> dataModel, NotificationFilterModel filterModel)
        {
            throw new NotImplementedException();
        }

        public ApplicationSettingModel SetApiSetting(MFKianNotificationApi.Models.ApplicationSettingModel model)
        {
            if (model == null)
                throw new ArgumentException($"{typeof(ApplicationSettingModel)} dose not have value!");


            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ApplicationSettingModel));
                TextWriter writer = new StreamWriter(GetSettingFilePath());
                
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }

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

            }

            _disposed = true;
        }

        private async Task<ApplicationSettingModel> GetSetting()
        {
            try
            {
                if (System.IO.File.Exists(GetSettingFilePath()))
                {
                    using var xmlReader = XmlReader.Create(GetSettingFilePath());
                }
                else
                    return null;
            }
            catch (Exception)
            {

                throw;
            }


        }

        private static string GetSettingFilePath()
        {
            return Environment.CurrentDirectory + @"\appsetting.xml";
        }

        bool IMfkianApiSencondVersion.SetApiSetting(ApplicationSettingModel model)
        {
            throw new NotImplementedException();
        }
    }
}
