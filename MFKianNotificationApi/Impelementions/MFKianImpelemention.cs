using MFKianNotificationApi.Interfaces;
using MFKianNotificationApi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;



namespace MFKianNotificationApi.Impelementions
{
    public class MFKianImpelemention : IMfkianApiSencondVersion
    {

        private readonly ILogger<MFKianImpelemention> _logger;

        public MFKianImpelemention(ILogger<MFKianImpelemention> logger)
        {
            _logger = logger;
        }


        public AppModel ApplicationSetting => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
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

        public bool SetApiSetting(MFKianNotificationApi.Models.ApplicationSettingModel model)
        {
            if (model == null)
                throw new ArgumentException($"{typeof(ApplicationSettingModel)} dose not have value!");


            try
            {

                var filepath = Environment.CurrentDirectory + @"\appsetting.xml";


                using var xmlWriter = XmlWriter.Create(filepath);

                xmlWriter.WriteStartElement("Setting");
                xmlWriter.WriteElementString(nameof(model.TimeCount), model.TimeCount.ToString());
                xmlWriter.WriteElementString(nameof(model.Path), model.Path);
                xmlWriter.WriteElementString(nameof(model.SendCount), model.SendCount.ToString());
                xmlWriter.WriteElementString(nameof(model.TaskPriorityTime), model.TaskPriorityTime.ToString());
                xmlWriter.WriteStartElement("Credential");
                xmlWriter.WriteElementString(nameof(model.CredentialModel.UserName), model.CredentialModel.UserName);
                xmlWriter.WriteElementString(nameof(model.CredentialModel.Password), model.CredentialModel.Password);
                xmlWriter.WriteElementString(nameof(model.CredentialModel.Domain), model.CredentialModel.Domain);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }
    }
}
