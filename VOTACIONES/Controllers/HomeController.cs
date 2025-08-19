using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VOTACIONES.Models;

namespace VOTACIONES.Controllers
{
   
    public class HomeController : Controller
    {

        private VOTACIONESEntities1 db = new VOTACIONESEntities1();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string documento, string contrasena)
        {
            var admin = db.Administrador
                .FirstOrDefault(a => a.correo_admin == documento && a.contraseña_admin == contrasena);

            if (admin != null)
            {
                Session["Administrador"] = admin;
                return RedirectToAction("Admin", "Administradors");
            }

            var aprendiz = db.Aprendiz
                .FirstOrDefault(a => a.correo_aprendiz == documento && a.contraseña_aprendiz == contrasena);

            if (aprendiz != null)
            {
                Session["Aprendiz"] = aprendiz;
                return RedirectToAction("Seleccion", "Aprendizs");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }



    }
}