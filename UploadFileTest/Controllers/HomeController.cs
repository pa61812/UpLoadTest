using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UploadFileTest.Models;
using UploadFileTest.Services;

namespace UploadFileTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Upload(FileUpload fileContent)
        {

            FileService.InsertFile(fileContent);
            return RedirectToAction("Contact", "Home", new { Area = "" });
        }


        public FileContentResult Getfile( string fileId)
        {
      
            var entity = FileService.FindFile(fileId);
            byte[] result;
            string contenttype;
            string filename;

            //FileContent fileContent = new FileContent
            //{
            //    REASON = entity[0].REASON,
            //    File_Content = entity[0].File_Content,
            //    File_ContentType = entity[0].File_ContentType,
            //    File_Name = entity[0].File_Name
            //};

            result = entity[0].File_Content;
            result = FileService.ToZip(result, entity[0].File_Name);

            return File(result, "application/zip");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}