using PosSelfService.Models;
using PosSelfService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PosSelfService.Controllers
{
    public class LoginController : Controller
    {
        private MainRepository mainRepo = MainRepository.Instance;

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginAuth(string USERNAME, string PASSWORD)
        {
            try
            {
                bool berhasil_login = true;//mainRepo.cekLogin(USERNAME, PASSWORD);
                if (berhasil_login)
                {
                    return Json("Sukses Login");
                }
                else
                {
                    throw new Exception("Gagal Login");
                }
            }
            catch (Exception ex)
            {
                return Json("Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }

            
        }
    }
}