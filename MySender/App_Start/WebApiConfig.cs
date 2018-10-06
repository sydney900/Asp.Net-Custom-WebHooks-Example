using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace MySender
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Load basic support for sending WebHooks
            config.InitializeCustomWebHooks();

            // Load Azure Storage or SQL for persisting subscriptions
            //config.InitializeCustomWebHooksAzureStorage();
            // config.InitializeCustomWebHooksSqlStorage();

            // Load Azure Queued Sender for enqueueing outgoing WebHooks to an Azure Storage Queue
            // config.InitializeCustomWebHooksAzureQueueSender();

            // Uncomment the following to set a custom WebHook sender where you can control how you want 
            // the outgoing WebHook request to look.
            // ILogger logger = CommonServices.GetLogger();
            // IWebHookSender sender = new MyWebHookSender(logger);
            // CustomServices.SetSender(sender);

            // Load Web API controllers for managing subscriptions
            config.InitializeCustomWebHooksApis();

        }
    }
}
