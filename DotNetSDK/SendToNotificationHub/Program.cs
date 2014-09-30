using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Notifications;


namespace SendToNotificationHub
{
    class Program
    {
        public const string acsConnectionString ="TODO";

        public const string hubName = "TestBaidu";
        public const string userId = "TODO";
        public const string channelId = "TODO";
        public const string registrationId = userId + "-" + channelId;
        public const string jsonTemplateBody = "{\"title\":\"$(title)\", \"description\":\"$(description)\"}";

        static private NamespaceManager m_namespaceManager;
        static private NotificationHubDescription m_nhDescription;
        static void Main(string[] args)
        {
            m_namespaceManager = NamespaceManager.CreateFromConnectionString(acsConnectionString);
            m_nhDescription = new NotificationHubDescription(hubName);

            m_nhDescription.BaiduCredential = new BaiduCredential()
            {
                BaiduApiKey = "TODO",
                BaiduSecretKey = "TODO",
                //BaiduEndPoint = "https://channel.api.duapp.com/rest/2.0/channel/channel"
            };

            SendBaiduNotificationAsync().Wait();
            // SendBauiduTemplateNotificationAsync().Wait();
            Console.ReadLine();
        }

        private static async Task SendBauiduTemplateNotificationAsync()
        {
            try
            {
                // if not exists.
                if (!m_namespaceManager.NotificationHubExists(hubName))
                {
                    m_namespaceManager.CreateNotificationHub(m_nhDescription);
                }
                else
                {
                    m_namespaceManager.UpdateNotificationHub(m_nhDescription);
                }

                // Create Hub.
                NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(acsConnectionString, hubName, true);

                var regs = (await hub.GetAllRegistrationsAsync(100)).ToList();

                // Create registration
                bool hasRegistration = false;
                if (!hasRegistration)
                {
                    await hub.CreateBaiduTemplateRegistrationAsync(userId, channelId, jsonTemplateBody);
                }

                // For Verification Purpose.
                regs = (await hub.GetAllRegistrationsAsync(100)).ToList();

                // Create template message.
                var notification = new Dictionary<string, string>() {
                        {"title", "myConsoleTitle"},
                        {"description", "myConsoleDescription"}
                };

                var resualt = await hub.SendTemplateNotificationAsync(notification);

                Console.WriteLine("Msg Sent:" + jsonTemplateBody);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task SendBaiduNotificationAsync()
        {
            // Uri endpoint = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "");
            // string acsConnectionString = ServiceBusConnectionStringBuilder.CreateUsingSharedSecret(endpoint, "owner", solutionKey);

            try
            {
                // if not exists.
                if (!m_namespaceManager.NotificationHubExists(hubName))
                {
                    m_namespaceManager.CreateNotificationHub(m_nhDescription);
                }

                // To create the connection to the hub.
                NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(acsConnectionString, hubName, true);

                var regs = (await hub.GetAllRegistrationsAsync(100)).ToList();

                // Create registration
                bool hasRegistration = false;
                if (!hasRegistration)
                {
                    await hub.CreateBaiduNativeRegistrationAsync(userId, channelId);
                }

                string toast = "{\"title\":\"Baidu Titl\",\"description\":\"Msg From Console App to Baidu\"}";

                var resualt =  await hub.SendBaiduNativeNotificationAsync(toast);

                Console.WriteLine("Msg Sent:" + toast);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}