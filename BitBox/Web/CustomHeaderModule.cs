using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitBox.Web
{
    // 헤더 정보삭제

    // 아래와 같이 추가 필요함
    //<system.webServer>
    //  <modules>
    //    <add name="CustomHeaderModule" type="BitBox.Web.CustomHeaderModule, BitBox" />
    //  </modules>
    //</system.webServer>

    // http://stackoverflow.com/questions/4078756/how-to-delete-iis-custom-headers-like-x-powered-by-asp-net-from-response
    // https://ict.ken.be/removing-x-powered-by-aspnet-and-other-version-headers
    public class CustomHeaderModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        public void Dispose() { }

        void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Date");
            HttpContext.Current.Response.Headers.Remove("X-Powered-By");
            HttpContext.Current.Response.Headers.Remove("X-SourceFiles");

            HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");
            HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");

            HttpContext.Current.Response.Headers.Remove("Server");
            //// Or you can set something funny
            //HttpContext.Current.Response.Headers.Remove("Content-Type");
            //HttpContext.Current.Response.Headers.Set("Content-Type", "application/octet-stream");

            //context.HttpContext.Response.Headers.Set("Content-Type", "application/octet-stream");
        }
    }
}