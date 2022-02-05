using MFKianNotificationApi.Impelementions;
using MFKianNotificationApi.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Serilog;
using Serilog.Sinks.File;
using System.Threading;
using System.IO;
using Serilog.Core;
using MFKianNotificationApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Linq;

namespace NotificationUI
{
    internal class Program
    {
        #region Decare the Variables
        private static long WhileCount = default;
        private static string curentUser = default;
        private static IEnumerable<TasksModel> userTasks;
        private static long errorCount = 0;
        private static double timerWaite = 30;
        #endregion

        #region Hide the console application 
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion


        static void Main(string[] args)
        {


            #region Initial Program Requriemnt 

            var logger = new LoggerConfiguration()
              .WriteTo.File(CreateLogFile(), rollingInterval: RollingInterval.Day)
              .MinimumLevel.Verbose()
              .CreateLogger();

            logger.Information("Logger Set Serilog Library Used For Loggin.");



            Task.Factory.StartNew(() =>
            {
                curentUser = GetCurentUserName();
            });

            logger.Information("Curent User retiver form the Active Directory ");

            var services = new ServiceCollection()
                  .AddSingleton<IMFKianApi, MFKianApi>()
                  .BuildServiceProvider();

            logger.Information("services variable is created  from ServiceCollection.");



            logger.Information("mfkianApi Created For this Application");
            var mfkianApi = services.GetService<IMFKianApi>();


            logger.Information("While Loop Is Being Start.");


            while (string.IsNullOrEmpty(curentUser)) { Thread.Sleep(1500);}

            curentUser = CreateUserDomain(curentUser);
            
            #endregion

            try
            {
                while (true)
                {
                    #region Checking For Varibale Data

                    if (logger == null)
                        logger = new LoggerConfiguration()
                          .WriteTo.File(CreateLogFile(), rollingInterval: RollingInterval.Day)
                          .MinimumLevel.Verbose()
                          .CreateLogger();

                    if (services == null)
                    {
                        services = new ServiceCollection()
                                     .AddSingleton<IMFKianApi, MFKianApi>()
                                     .BuildServiceProvider();
                        logger.Information("services created ....");
                    }

                    if (mfkianApi == null)
                        mfkianApi = services.GetService<IMFKianApi>();

                    #endregion

                    #region Get UserTask for Notification
                    if (userTasks == null)
                    {
                        var userdata = mfkianApi.GetUserData(new MFKianNotificationApi.Models.RequestModel
                        {
                            RequestDataModel = new MFKianNotificationApi.Models.RequestDataModel
                            {
                                
                                BaseUrl = @"http://80.210.26.4:8585/MFKIAN/api/data/v9.0/",
                                Count = 3,
                                EnttiyName = "systemusers",
                                Filters = new List<FilterDataModel> { new FilterDataModel { Item = "domainname", Key = "eq", Type = MFKianNotificationApi.Enums.RequestDataFilterType.Content, Value =  curentUser} },
                                SelectItem = new string[] { "domainname", "identityid", "fullname" }
                            },

                            CredentialModel = new CredentialModel
                            {
                                Domain = "KIAN",
                                Password = "r",
                                UserName = "a.moradi"
                            }
                        });

                        var usermodel = userdata.Value.FirstOrDefault();

                        logger.Information("user information was got from the crm api");

                        var tasksdata = mfkianApi.GetUserTasks(new RequestModel
                        {
                            CredentialModel = new CredentialModel { Domain = "KIAN", Password = "r", UserName = "a.moradi" },
                            RequestDataModel = new RequestDataModel
                            {
                                BaseUrl = @"http://80.210.26.4:8585/MFKIAN/api/data/v9.0/",
                                Count = 3,
                                EnttiyName = "tasks",
                                Filters = new List<FilterDataModel> { new FilterDataModel { Item = "_ownerid_value", Key = "eq", Type = MFKianNotificationApi.Enums.RequestDataFilterType.UniqIdentitfire, Value = usermodel.Ownerid } },
                                SelectItem = new string[] { "subject", "prioritycode", "new_task_status", "new_task_type", "_ownerid_value", "new_remained_time_hour", "new_remaining_days" }
                            }
                        });

                        logger.Information("user information was got from the crm api");

                        //  userTasks = CheckTaskStates(tasksdata.Value);
                        userTasks = tasksdata.Value.ToList();
                    }
                    #endregion


                    #region SendNotification Section
                    SendFilteredNotification(userTasks, mfkianApi);
                    #endregion

                    #region Waiting for Duration
                    logger.Information("Application Waited will Waited 30 min One Hour.");
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                    logger.Information($"Waiting Time Is Finished \n while loop start for {WhileCount += 1} time.");
                    logger.Information("--------------------------------------------------------------------------------------------------- end of application logic");
                    #endregion
                }
            }
            catch (Exception ex)
            {
                logger.Error($"erro ouccered :{ex.Message} \n inner exception : {ex.InnerException.Message ?? "no inner exception"}");
            }

        }



        /// <summary>
        /// Create Log file In ApplicationLoggin Direcotroy in bin folder with loggfile 2020,10,20, 10-40-40-4562.text format
        /// </summary>
        /// <returns>Log File Name</returns>
        private static string CreateLogFile()
        {
            string directoryPath = Environment.CurrentDirectory + @"\ApplicationLogging";
            string filename = @"\loggfile" + DateTime.Now.ToString(" yyyy,MM,dd, hh-mm-ss-ffff") + ".txt";


            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);


            string finalfile = directoryPath + filename;

            File.CreateText(finalfile);

            return finalfile;
        }


        /// <summary>
        /// Get Curent User From the Active Directory With System.DirectoryServices.AccountManagement
        /// </summary>
        /// <returns></returns>
        private static string GetCurentUserName()
        {
            return System.DirectoryServices.AccountManagement.UserPrincipal.Current.UserPrincipalName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static List<TasksModel> CheckTaskStates(List<TasksModel> model)
        {

            var list = new List<TasksModel>();
            if (model.Count > 0)
            {
                foreach (var task in model)
                {
                    //Checking for the Expired and Done tasks
                    if (task.new_task_status != 100_000_005 || task.new_task_status != 100_000_003)
                    {
                        //cheking for the task that remaingin just one day
                        if (task.new_remaining_days > 0 && task.new_remaining_days < 2)
                        {
                            list.Add(task);
                        }
                    }
                }
            }


            return list;

        }


        /// <summary>
        /// show notification in woindows 
        /// </summary>
        /// <param name="tasks"></param>
        private static void SendFilteredNotification(IEnumerable<TasksModel> tasks, IMFKianApi mfkianapi)
        {
            foreach (var task in tasks)
            {
                if (task.RemainingTime.HasValue)
                {

                    var time = task.RemainingTime.Value - DateTime.Now;

                    //if (time.TotalHours < 2)
                    //{
                    //    mfkianapi.SendNotification(new NotificationSettingModel
                    //    {
                    //        Text = new string[] { task.subject, $"زمان بازی باقی مانده برای اینجام این تسک {time}" },
                    //        Titel = task.new_task_type.ToString(),
                    //        ToastDuration = Microsoft.Toolkit.Uwp.Notifications.ToastDuration.Long,
                    //        ToastScenario = Microsoft.Toolkit.Uwp.Notifications.ToastScenario.Reminder,
                    //        Url = "http://80.210.26.4:8585/MFKian",
                    //        Button = null
                    //    });
                    //}

                    mfkianapi.SendNotification(new NotificationSettingModel
                    {
                        Text = new string[] { task.subject, $"زمان بازی باقی مانده برای اینجام این تسک {time}" },
                        Titel = task.new_task_type.ToString(),
                        ToastDuration = Microsoft.Toolkit.Uwp.Notifications.ToastDuration.Long,
                        ToastScenario = Microsoft.Toolkit.Uwp.Notifications.ToastScenario.Reminder,
                        Url = "http://80.210.26.4:8585/MFKian",
                        Button = null
                    });
                }
            }
        }


        private static void SetApplicationSetting(double WaitedMinut)
        {

            Console.WriteLine("Application Started!");

            ///******************************************************************************************///

            Console.WriteLine("Set the Time duration for application waiting exm:30 in minute");
            timerWaite = Convert.ToDouble(Console.ReadLine());
        }

        public static string CreateUserDomain(string name)
        {
            var temp = name.Split("@");
           return  @"KIAN\"+temp[0];
        }
    }
}
