using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Schema;
using EasySense.Models;
using System.Data.Entity.Validation;

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
            //
            foreach (FileModel file in files)
            {
                if (!file.CanPreview)
                {
                    continue;
                }
                //
                try
                {
                    if (file.FileBlob != null)
                    {
                        var ext = file.Extension;
                        if (ext != null && ext.ToLower().Equals(".txt"))
                        {
                            ext = ".doc";
                        }
                        var filename = file.ID + ext;
                        var dir = Server.MapPath("/Preview");
                        if (!System.IO.Directory.Exists(dir))
                        {
                            System.IO.Directory.CreateDirectory(dir);
                        }
                        var filepath = dir + @"\" + filename;
                        System.IO.File.WriteAllBytes(filepath, file.FileBlob);
                    }
                }
                catch
                {
                }
            }
            //
            return View(files);
        }

        [MinRole(UserRole.Root)]
        public ActionResult Upload(FileCategory FileCategory)
        {
            var file = Request.Files[0];
            try
            {
                var tmp = new FileModel();
                if (file.ContentLength > 0)
                {
                    tmp.ID = Guid.NewGuid();
                    tmp.Time = DateTime.Now;
                    tmp.Filename = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    tmp.Extension = System.IO.Path.GetExtension(file.FileName);
                    tmp.ContentType = file.ContentType;
                    tmp.UserID = CurrentUser.ID;
                    var timestamp = Helpers.String.ToTimeStamp(DateTime.Now);
                    var filename = timestamp + tmp.Extension;
                    var dir = Server.MapPath("~") + @"\Temp\";
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    file.SaveAs(dir + filename);
                    tmp.FileBlob = System.IO.File.ReadAllBytes(dir + filename);
                    DB.Files.Add(tmp);
                    DB.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                string msg = string.Format("ContentType:{0}\r\n", file.ContentType);
                foreach (var item in ex.EntityValidationErrors)
                {
                    foreach (var item2 in item.ValidationErrors)
                    {
                        msg += string.Format("{0}:{1}\r\n", item2.PropertyName, item2.ErrorMessage);
                    }
                }
                return RedirectToAction("Message", "Shared", new { msg = msg });
            }
            catch (Exception e)
            {
                return RedirectToAction("Message", "Shared", new { msg = e.ToString() });
            }
            return RedirectToAction("Index", "File");
        }

        [AccessToFile]
        public ActionResult Download(Guid id)
        {
            var file = DB.Files.Find(id);
            return File(file.FileBlob, file.ContentType, file.Filename + file.Extension);
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