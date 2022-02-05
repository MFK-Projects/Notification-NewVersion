
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKianNotificationApi.Enums
{
    public enum RequestDataFilterType
    {
        Content,
        Number,
        UniqIdentitfire
    }
    public enum TaskType
    {
        //FollowUp = 100_000_006,
        //CrmProblem = 100_000_000,
        //CrmSuggestion = 100_000_001,
        //CrmOthers = 100_000_002,
        //SiteBug = 100_000_003,
        //SiteAddFeature = 100_000_004,
        //Others = 100_000_005

        FollowUp ,
        CrmProblem ,
        CrmSuggestion ,
        CrmOthers ,
        SiteBug ,
        SiteAddFeature,
        Others 
    }

    public enum NTasksStatus
    {
        //PreRegistered = 100_000_000,
        //Seen = 100_000_001,
        //OnProccess = 100_000_002,
        //Done = 100_000_003,
        //Accepted = 100_000_004,
        //Expired = 100_000_005


        PreRegistered,
        Seen,
        OnProccess,
        Done,
        Accepted ,
        Expired 
    }


}
