using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadFileTest.Models
{
    public class FileUpload
    {
        public string REASON { get; set; }

        public IEnumerable<HttpPostedFileBase> files { get; set; }
    }
}