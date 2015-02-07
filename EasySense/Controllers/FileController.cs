using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Schema;
using EasySense.Models;

namespace EasySense.Controllers
{
    [Authorize]
    public class FileController : BaseController
    {
        // GET: File
        public ActionResult Index(FileCategory? id)
        {
            IEnumerable<FileModel> files = DB.Files;
            if (id != null)
                files = files.Where(x => x.FileCategory == id);
            files = files.OrderByDescending(x => x.Time).ToList();
            return View(files);
        }

        [MinRole(UserRole.Root)]
        public ActionResult Upload(FileCategory FileCategory, bool Public)
        {
            var tmp = new FileModel();
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                tmp.ID = Guid.NewGuid();
                tmp.Time = DateTime.Now;
                tmp.Filename = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                tmp.Extension = System.IO.Path.GetExtension(file.FileName);
                tmp.ContentType = file.ContentType;
                tmp.UserID = CurrentUser.ID;
                tmp.Public = Public;
                var timestamp = Helpers.String.ToTimeStamp(DateTime.Now);
                var filename = timestamp + tmp.Extension;
                var dir = Server.MapPath("~") + @"\Temp\";
                file.SaveAs(dir + filename);
                tmp.FileBlob = System.IO.File.ReadAllBytes(dir + filename);
                System.IO.File.Delete(dir + filename);
                DB.Files.Add(tmp);
                DB.SaveChanges();
            }
            return RedirectToAction("Index", "File");
        }

        [AccessToFile]
        public ActionResult Download(Guid id)
        {
            var file = DB.Files.Find(id);
            return File(file.FileBlob, file.ContentType);
        }

        [HttpGet]
        [MinRole(UserRole.Root)]
        public ActionResult Delete(Guid id)
        {
            var file = DB.Files.Find(id);
            DB.Files.Remove(file);
            DB.SaveChanges();
            return RedirectToAction("Index", "File");
        }
    }
}