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
        private static System.Timers.Timer settingTimer;
        private static System.Timers.Timer appTimer;
        private static Logger logger;
        private static ServiceProvider services;
        #endregion

        #region Hide the console application 
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion


        static void Main(string[] args)
        {
            IMFKianApi mfKianApi;
            try
            {

                if (_hideWindow == 1)
                {
                    _curentWindow = GetConsoleWindow();
                    ShowWindow(_curentWindow, 1);
                }

                Task.Factory.StartNew(() =>
                {
                    curentUser = GetCurentUserName();
                });

                Console.WriteLine("Application is started!..");
                Console.WriteLine("Initializing the Requirement");
                #region Initial Program Requirement 

                logger = new LoggerConfiguration()
                  .WriteTo.File(CreateLogFile(), rollingInterval: RollingInterval.Day)
                  .MinimumLevel.Verbose()
                  .CreateLogger();



                services = new ServiceCollection()
                               .AddScoped<IMFKianApi, MFKianApi>()
                               .BuildServiceProvider();

                logger.Information<ServiceProvider>("the Di Container is created!", services);

                mfKianApi = services.GetService<IMFKianApi>();

                logger.Information<IMFKianApi>("the mfkian api servcice initialzied ", mfKianApi);



                if (mfKianApi.ApplicationSetting == null)
                {
                    var test = mfKianApi.GetApiSetting();


                    if (test)
                    {
                        mfKianApi.SetApiSetting(new CredentialModel { Domain = "KIAN", Password = "r", UserName = "a.moradi" });

                        if (_firstLantch)
                        {
                            mfKianApi.SendWellComeNotification();
                            _firstLantch = false;
                        }
                    }
                }

                #region Settign the timers for infinte interval

                settingTimer = new();

                if (mfKianApi.ApplicationSetting.SettingTimer > 0)
                    settingTimer.Interval = (mfKianApi.ApplicationSetting.SettingTimer * 60_000);
                else
                    settingTimer.Interval = (15 * 60_000);


                settingTimer.AutoReset = false;
                settingTimer.Enabled = true;
                settingTimer.Elapsed += (sender, e) => SettingTimer_Elapsed(sender, e, mfKianApi);

                appTimer = new();

                if (mfKianApi.ApplicationSetting.TimeAwaite > 0)
                    appTimer.Interval = (mfKianApi.ApplicationSetting.TimeAwaite * 60_000);
                else
                    appTimer.Interval = (30 * 60_000);


                appTimer.AutoReset = false;
                appTimer.Enabled = true;
                appTimer.Elapsed += (sender, e) => AppTimer_Elapsed(sender, e, mfKianApi);

                #endregion


                if (_hideWindow == 1)
                {
                    _curentWindow = GetConsoleWindow();
                    ShowWindow(_curentWindow, 0);
                }

                SettingTimer_Elapsed(null, null, mfKianApi);
                AppTimer_Elapsed(null, null, mfKianApi);



                logger.Information("waited for user enter the ex-force for closeing the application");
            infinte: var exitcommand = Console.ReadLine();

                if (exitcommand == "ex-force")
                    Environment.Exit(0);
                else
                    goto infinte;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                appTimer?.Dispose();
                settingTimer?.Dispose();
            }
        }

        private static void AppTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e, IMFKianApi mfkianApi)
        {

            var userwileCount = 0;

            while (string.IsNullOrEmpty(curentUser))
            {
                Thread.Sleep(1500);
                WhileCount += 1;
                if (userwileCount > 10)
                {
                    Console.WriteLine("You Stuck in the Geting UserNameWhile And Application Will be ShutDown..");
                    logger.Error("can not retive the user from the DirectoryApplication");
                    Environment.Exit(0);
                }

            }


            if (!string.IsNullOrEmpty(curentUser))
                curentUser = CreateUserDomain(curentUser);



            Console.WriteLine("Application Successfuly Initialized the Requirement!");
            logger.Information<Program>("Application Successfuly Initialized the Requirement!", null);
            #endregion

            try
            {
                if (_firstLantch)
                {
                    Console.WriteLine("Well Come Notification going to be send");
                    mfkianApi.SendWellComeNotification();
                    _firstLantch = false;
                    Console.WriteLine("WellCome Notification Sent");
                }

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
                    var test = mfkianApi.GetApiSetting();


                    if (test)
                    {
                        mfkianApi.SetApiSetting(new CredentialModel { Domain = "KIAN", Password = "r", UserName = "a.moradi" });
                    }
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
                            SelectItem = new string[] { "activityid", "new_remained_time_hour", "new_remaining_days", "new_task_status", "new_task_type", "subject" }
                        }
                    });

                    if (userTasks.Any())
                        mfkianApi.SendNotification(userTasks.ToList(), new NotificationFilterModel
                        {
                            DayCheck = 3,
                            HourCheck = 2,
                            NTasksStatus = taskStuatuCreator(mfkianApi.ApplicationSetting.TaskStatus),
                            TaskType = CreateTaskTyps(mfkianApi.ApplicationSetting.TasksType),
                        });
                    else
                        mfkianApi.SendErrorNotification("هیچ تسکی برای انجام دادن وجود ندارد");

                    logger.Information("user information was got from the crm api");
                }
                #endregion


                #region SendNotification Section

                #endregion


                #region Waiting for Duration
                logger.Information("Application Waited will Waited 30 min One Hour.");
                Thread.Sleep(TimeSpan.FromMinutes(mfkianApi.ApplicationSetting.TimeAwaite));
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
            catch (Exception ex)
            {
                if (_hideWindow == 0)
                {
                    _curentWindow = GetConsoleWindow();
                    ShowWindow(_curentWindow, 1);
                }
                throw new Exception(ex.Message);
            }

        }

        private static void SettingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e, IMFKianApi mFKianApi)
        {
            try
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

                if (mFKianApi == null)
                    mFKianApi = services.GetService<IMFKianApi>();

                if (string.IsNullOrEmpty(mFKianApi.ApplicationSetting.NotificationReqularMessage))
                {

                    var test = mFKianApi.GetApiSetting();


                    if (test)
                    {
                        mFKianApi.SetApiSetting(new CredentialModel { Domain = "KIAN", Password = "r", UserName = "a.moradi" });


                        if (_firstLantch)
                            mFKianApi.SendWellComeNotification();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (_hideWindow == 0)
                {
                    _curentWindow = GetConsoleWindow();
                    ShowWindow(_curentWindow, 1);
                }
                throw new Exception(ex.Message);
            }
        }



        /// <summary>
        /// Create Log file In ApplicationLoggin Direcotroy in bin folder with loggfile 2020,10,20, 10-40-40-4562.text format
        /// </summary>
        /// <returns>Log File Name</returns>
        private static string CreateLogFile()
        {
            string directoryPath = Environment.CurrentDirectory + @"\ApplicationLogging";

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);


            return directoryPath + @"\log_file.txt";
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
            if (name.Contains(@"KIAN\"))
                return name;


            var temp = name.Split("@");
            return @"KIAN\" + temp[0];
        }

        private static long[] taskStuatuCreator(string status)
        {
            if (string.IsNullOrEmpty(status))
                return default;

            var temp = status.Split(",");
            long[] data = default;


            for (int i = 0; i < temp.Length; i++)
                long.TryParse(temp[i], out data[i]);


            return data;
        }

        private static long[] CreateTaskTyps(string types)
        {
            if (string.IsNullOrEmpty(types))
                return default;

            var temp = types.Split(",");
            long[] data = default;


            for (int i = 0; i < temp.Length; i++)
                long.TryParse(temp[i], out data[i]);

            return data;

        }
    }
}
