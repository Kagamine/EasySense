using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasySense.Controllers
{
    public class FileController : BaseController
    {
        // GET: File
        public ActionResult Index()
        {
            return View();
        }
    }
}