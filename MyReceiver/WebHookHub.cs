using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Business;

namespace MyReceiver
{
    public class WebHookHub : Hub
    {
        public void AddedProduct(Product prod)
        {
            Clients.All.addedProduct(prod);
        }
    }
}