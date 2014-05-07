using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitBox.Web
{
    //http://weblogs.asp.net/andrewrea/archive/2010/02/16/a-binarycontentresult-for-asp-net-mvc.aspx

    public class BinaryResult : ActionResult
    {
        private static readonly string BINARY_OCTET_STREAM = "binary/octet-stream";

        protected ArraySegment<byte> Data { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = BINARY_OCTET_STREAM;
            context.HttpContext.Response.OutputStream.Write(Data.Array, Data.Offset, Data.Count);
            context.HttpContext.Response.OutputStream.Flush();
        }
    }
}