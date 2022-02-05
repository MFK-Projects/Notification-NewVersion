using MFKianNotificationApi.Enums;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;

namespace MFKianNotificationApi.Models
{
    public class NotificationCreationModel
    {
        public string Titel { get; set; }
        public string[] Text { get; set; }
        public ToastScenario ToastScenario { get; set; } = ToastScenario.Default;
        public ToastDuration ToastDuration { get; set; } = ToastDuration.Short;
        public List<ToastButton> Button { get; set; }
        public string TaskUrl { get; set; }
        
    }

    public class RequestModel
    {
        public RequestDataModel RequestDataModel { get; set; }
        public CredentialModel CredentialModel { get; set; }
    }


    public class ApplicationSettingModel
    {
        public TimeSpan DelayBetweenRequests { get; set; }  
        public string ApplicationName { get; set; }
        public ConsoleColor ConsoleColor { get; set; }
        public bool CloseApp { get; set; }
        public bool ExitFunctionality { get; set; }
    }

    public class RequestDataModel
    {
        public string BaseUrl { get; set; }
        public string Url { get; set; }
        public string[] SelectItem { get; set; }    
        public string EnttiyName { get; set; }
        public int Count { get; set; }
        public List<FilterDataModel> Filters { get; set; }
    }

    public class FilterDataModel
    {
        public string Item { get; set; }
        public object Value { get; set; }
        public string Key { get; set; }
        public RequestDataFilterType Type { get; set; }
    }
    public class CredentialModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }    
        public string Domain { get; set; }
    }

    public class CrmTaskUrl
    {
        public string BaseUrl { get; set; } = @"http://crm-srv:8585/MFkian/main.aspx?etn";
        public string EntityName { get; set; }
        public string EntityId { get; set; }
    }
}
