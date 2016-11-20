using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC.DTO
{
    public class HttpFileStream
    {
        public string Name { get; set; }
        public Stream InputStream { get; set; }
    }
}
