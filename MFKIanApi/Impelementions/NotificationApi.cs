using MFKIanApi.Exceptions;
using MFKIanApi.Interfaces;
using MFKIanApi.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKIanApi.Impelementions
{
    public class NotificationApi : INotificationApi
    {

        private bool _disposed = false;
        private ILogger<NotificationApi> _logger;

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
        protected virtual void Dispose(bool dispose)
        {
            if (_disposed) return;
            if (dispose)
            {
                _logger.LogInformation("this object is disposed");
            }

            _disposed = true;
        }
    }
}
