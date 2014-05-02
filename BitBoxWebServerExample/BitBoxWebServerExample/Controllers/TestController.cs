using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitBox.Web;

namespace BitBoxWebServerExample.Controllers
{
    public class TestController : Controller
    {
        public BinaryContentResult TestBinary()
        {
            //string temp = Encoding.UTF8.GetString(bytedata);

            byte[] buffer = new byte[HttpContext.Request.ContentLength];
            HttpContext.Request.InputStream.Read(buffer, 0, buffer.Length);

            return new BinaryContentResult() { Data = buffer, Headers = HttpContext.Response.Headers };
        }
    }
}
