using MFKianNotificationApi.Interfaces;
using System;
using System.Xml;

namespace CrmNotificationXmlUi
{
    internal class Program
    {
        static void Main(string[] args)
        {

            IMfkianApiSencondVersion api = new MFKianNotificationApi.Impelementions.MFKianImpelemention();


            try
            {

                api.SetApiSetting(new MFKianNotificationApi.Models.ApplicationSettingModel
                {
                    CredentialModel = new MFKianNotificationApi.Models.CredentialModel
                    {
                        Domain = "KIAN",
                        Password = "r",
                        UserName = "a.moradi"
                    },
                   SendCount =3,
                   TaskPriorityTime  = 3,
                   TimeCount = 3
                });
                Console.Write("Done");
                Console.ReadLine();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
