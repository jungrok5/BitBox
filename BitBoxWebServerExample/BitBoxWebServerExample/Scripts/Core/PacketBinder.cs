using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitBoxExample.CSCommon;

namespace BitBoxWebServerExample.Scripts.Core
{
    public class PacketBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.HttpContext.Request;
            if (request == null)
                return null;

            // TODO 여기도 메모리풀 사용 고려

            byte[] buffer = new byte[request.ContentLength];
            request.InputStream.Read(buffer, 0, buffer.Length);
            Packet packet = new Packet(buffer, buffer.Length);
            return packet;
        }
    }
}