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
        private static IntPtr _curentWindow = IntPtr.Zero;
        private static int _hideWindow = 1;
        private static bool _firstLantch = true;
        #endregion

        #region Hide the console application 
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion


        static void Main(string[] args)
        {

            Console.WriteLine("Application is started!..");
            Console.WriteLine("Initializing the Requirement");
            #region Initial Program Requirement 

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


            while (string.IsNullOrEmpty(curentUser))
            {
                var wileCount = 0;
                Thread.Sleep(1500);
                WhileCount += 1;

                if (wileCount > 10)
                {
                    Console.WriteLine("You Stuck in the Geting UserNameWhile And Application Will be ShutDown..");
                    Environment.Exit(0);
                }
            }

            curentUser = CreateUserDomain(curentUser);

            Console.WriteLine("Application Successfuly Initialized the Requirement!");
            #endregion

            try
            {
                if (_firstLantch == true)
                {
                    Console.WriteLine("Well Come Notification going to be send");
                    mfkianApi.SendWellComeNotification();
                    _firstLantch = false;
                    Console.WriteLine("WellCome Notification Sent");
                }

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

                    if (mfkianApi.ApplicationSetting == null)
                    {
                        mfkianApi.SetApiSetting(new AppModel
                        {
                            CredentialModel = new CredentialModel { Domain = "KIAN", Password = "r", UserName = "a.moradi" },
                            TimeAwaite = 30,
                            BaseUrl = @"http://crm-srv:8585/MFkian/api/data/v9.0/",
                            NotificationReqularMessage = "مدت زمان انجام این تشک"
                        });
                    }

                    #endregion

                    #region Get UserTask for Notification
                    if (userTasks == null)
                    {
                        var curentuser = mfkianApi.GetSingleRow(new RequestModel
                        {
                            CredentialModel = mfkianApi.ApplicationSetting.CredentialModel,
                            RequestDataModel = new RequestDataModel
                            {
                                BaseUrl = mfkianApi.ApplicationSetting.BaseUrl,
                                Count = 3,
                                EnttiyName = "systemusers",
                                Filters = new List<FilterDataModel>
                                {
                                    new FilterDataModel
                                    {
                                        Item = "domainname",
                                        Key = "eq",
                                        Type = MFKianNotificationApi.Enums.RequestDataFilterType.Content,
                                        Value = curentUser
                                    }
                                },
                                SelectItem = new string[] { "fullname", "domainname", "identityid", "systemuserid" }
                            }
                        }).FirstOrDefault();

                        userTasks = mfkianApi.GetMultipuleRows(new RequestModel
                        {
                            CredentialModel = mfkianApi.ApplicationSetting.CredentialModel,
                            RequestDataModel = new RequestDataModel
                            {
                                BaseUrl = mfkianApi.ApplicationSetting.BaseUrl,
                                EnttiyName = "tasks",
                                Filters = new List<FilterDataModel> { new FilterDataModel {
                                    Item = "_ownerid_value",
                                    Key = "eq",
                                    Type = MFKianNotificationApi.Enums.RequestDataFilterType.UniqIdentitfire,
                                    Value = curentuser.Ownerid
                                }},
                                SelectItem = new string[] { "activityid", "new_remained_time_hour", "new_remaining_days", "new_task_status", "new_task_type" }
                            }
                        });

                        logger.Information("user information was got from the crm api");
                    }
                    #endregion


                    #region SendNotification Section
                    mfkianApi.SendNotification(userTasks.ToList(), new NotificationFilterModel
                    {
                        DayCheck = 3,
                        HourCheck = 2,
                        NTasksStatus = new long[] { 100_000_005, 100_000_003 },
                        TaskType = default,
                    });
                    #endregion


                    #region Waiting for Duration
                    logger.Information("Application Waited will Waited 30 min One Hour.");
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                    logger.Information($"Waiting Time Is Finished \n while loop start for {WhileCount += 1} time.");
                    logger.Information("------------------------------------------------------------------------------------------------------------------- end of application logic");
                    #endregion


                    #region Hide the console Application

                    if (_hideWindow == 1)
                    {
                        _curentWindow = GetConsoleWindow();
                        ShowWindow(_curentWindow, 0);
                    }

                    #endregion

                }

            }
            catch (Exception ex)
            {
                if (_hideWindow == 0)
                {
                    _curentWindow = GetConsoleWindow();
                    ShowWindow(_curentWindow, 1);
                }
                logger.Error($"erro ouccered :{ex?.Message} \n inner exception : {ex?.InnerException?.Message ?? "no inner exception"}");
                Console.WriteLine(ex.Message);
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



        public static string CreateUserDomain(string name)
        {
            var temp = name.Split("@");
            return @"KIAN\" + temp[0];
        }
    }
}
