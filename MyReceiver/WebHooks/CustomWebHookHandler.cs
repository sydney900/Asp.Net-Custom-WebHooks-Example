using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;
using Business;
using Microsoft.AspNet.SignalR;

namespace MyReceiver.WebHooks
{
    public class CustomWebHookHandler : WebHookHandler
    {
        public CustomWebHookHandler()
        {
            this.Receiver = CustomWebHookReceiver.ReceiverName;
        }

        public override Task ExecuteAsync(string generator, WebHookHandlerContext context)
        {
            // Get data from WebHook
            CustomNotifications data = context.GetDataOrDefault<CustomNotifications>();

            // Get data from each notification in this WebHook
            foreach (IDictionary<string, object> notification in data.Notifications)
            {
                if (notification == null)
                    continue;

                // Process data
                switch (notification["Action"].ToString())
                {
                    case "AddedProduct":
                        Product product = new Product()
                        {
                            Id = (long)notification["Id"],
                            Name = (string)notification["Name"],
                            Description = (string)notification["Description"]
                        };

                        var hubContext = GlobalHost.ConnectionManager.GetHubContext<WebHookHub>();
                        hubContext.Clients.All.addedProduct(product);      
                        break;

                    default:
                        break;
                };
            }

            context.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Gone);
            return Task.FromResult(true);
        }
    }
}