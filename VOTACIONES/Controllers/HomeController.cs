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
        // Buscar administrador
        var admin = db.Administrador
            .FirstOrDefault(a => a.DocumentoIdentidad == documento && a.DocumentoIdentidad == contrasena);

        if (admin != null)
        {
            // Guardar objeto completo en sesión
            Session["Administrador"] = admin;
            // Guardar el ID del administrador logueado
            Session["AdminId"] = admin.id;

            return RedirectToAction("Admin", "Administradors");
        }

        // Buscar aprendiz
        var aprendiz = db.Aprendiz
            .FirstOrDefault(a => a.DocumentoIdentidad == documento && a.DocumentoIdentidad == contrasena);

        if (aprendiz != null)
        {
            // Guardar objeto completo en sesión
            Session["Aprendiz"] = aprendiz;
            // Guardar el ID del aprendiz logueado
            Session["AprendizId"] = aprendiz.id_aprendiz;

            return RedirectToAction("Seleccion", "Aprendizs");
        }

        // Si no se encuentra usuario válido
        ViewBag.Error = "Usuario o contraseña incorrectos";
        return View();
    }




    }
}