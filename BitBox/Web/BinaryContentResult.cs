using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitBox.Web
{
    //http://weblogs.asp.net/andrewrea/archive/2010/02/16/a-binarycontentresult-for-asp-net-mvc.aspx

    public class BinaryContentResult : ActionResult
    {
        public byte[] Data { get; set; }
        public NameValueCollection Headers { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            //foreach (string s in Headers.Keys)
            //{
            //    context.HttpContext.Response.AddHeader(s, Headers[s]);
            //}

            context.HttpContext.Response.ContentType = "binary/octet-stream";
            //context.HttpContext.Response.BinaryWrite(Data);
            context.HttpContext.Response.OutputStream.Write(Data, 0, Data.Length);
            context.HttpContext.Response.OutputStream.Flush();
            //context.HttpContext.Response.End();
        }
    }
}