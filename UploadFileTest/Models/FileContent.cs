using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadFileTest.Models
{
    public class FileContent
    {
        public int Identerty { get; set; }

        public string REASON { get; set; }

        public string File_Name { get; set; }

        public string File_ContentType { get; set; }

        public Byte[] File_Content { get; set; }
    }
}