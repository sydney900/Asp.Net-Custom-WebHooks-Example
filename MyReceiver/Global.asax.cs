using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;


namespace MyReceiver
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static string ApplicationURL = null;
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (ApplicationURL == null)
            {
                // Only assign the ApplicationURL once.


                // Remove the page path information.
                Regex regex = new Regex("(" + HttpContext.Current.Request.Url.AbsolutePath + ")$");

                ApplicationURL = regex.Replace(HttpContext.Current.Request.Url.AbsoluteUri, string.Empty);

                SubscribeWebHooks();
            }
        }

        private bool registered;
        private static string MyToken;
        private static Registration SubscribeWebHooks()
        {
            using (HttpClient client = new HttpClient())
            {
                string senderURL = ConfigurationManager.AppSettings["SenderURL"];
                Uri baseAddress = new Uri(senderURL);
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = baseAddress;

                var username = "sydney900@hotmail.com";
                var password = "Web@H00ks";

                // for the basic auth
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //var byteArray = new UTF8Encoding().GetBytes(String.Format("{0}:{1}", username, password));
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                #region for the asp.net identity user

                var formContent = new FormUrlEncodedContent(new[]
                 {
                     new KeyValuePair<string, string>("Email", username),
                     new KeyValuePair<string, string>("Password", password),
                 });

                //send request                
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));

                HttpResponseMessage responseMessage = client.PostAsync(senderURL + "/Account/Login", formContent).Result;

                ////get access token from response body
                //var responseJson = responseMessage.Content.ReadAsStringAsync().Result;
                //var jObject = Newtonsoft.Json.Linq.JObject.Parse(responseJson);
                //MyToken = jObject.GetValue("access_token").ToString();

                //var authValue = new AuthenticationHeaderValue("Bearer", MyToken);
                //client.DefaultRequestHeaders.Authorization = authValue;

                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var data = new Registration();
                    data.WebHookUri = new Uri(new Uri(WebApiApplication.ApplicationURL), "api/webhooks/incoming/custom").ToString();
                    data.Secret = ConfigurationManager.AppSettings["MS_WebHookReceiverSecret_Custom"];
                    data.Description = "MyReceiver";


                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Uri postAddress = new Uri(baseAddress, "/api/webhooks/registrations");
                    var response = client.PostAsJsonAsync<Registration>(postAddress, data).Result;
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        //WebHook postedWebHook = response.Content.ReadAsAsync<WebHook>().Result;
                    }
                }

                #endregion
                return null;
            }

        }
    }


    public class Registration
    {
        public string WebHookUri { get; set; }
        public string Secret { get; set; }
        public string Description { get; set; }
        public List<string> Filters { get; set; }
    }

}
