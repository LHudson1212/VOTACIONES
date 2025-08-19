using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VOTACIONES.Models;

namespace VOTACIONES.Controllers
{
    public class PostuladosController : Controller
    {
        private VOTACIONESEntities1 db = new VOTACIONESEntities1();

        // GET: Postulados
        public ActionResult Index()
        {
            return View(db.Postulados.ToList());
        }

        // GET: Postulados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Postulados postulados = db.Postulados.Find(id);
            if (postulados == null)
            {
                return HttpNotFound();
            }
            return View(postulados);
        }

        // GET: Postulados/Create
        // GET: Postulados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Postulados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "IdPostulado,Nombre,Propuestas,Descripcion,Jornada")] Postulados postulados,
            HttpPostedFileBase FotoFile,
            string Aptitud1,
            string Aptitud2,
            string Aptitud3)
        {
            if (ModelState.IsValid)
            {
                // Manejo de foto
                if (FotoFile != null && FotoFile.ContentLength > 0)
                {
                    // Carpeta donde se guardarán las fotos
                    string folderPath = Server.MapPath("~/Content/Img-postulados/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Nombre único para evitar conflictos
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(FotoFile.FileName);
                    string path = Path.Combine(folderPath, fileName);

                    FotoFile.SaveAs(path);

                    // Guardamos la ruta relativa en la BD
                    postulados.Foto = "/Content/Img-postulados/" + fileName;
                }

                // Concatenamos las aptitudes (si quieres guardarlas juntas en 1 campo)
                List<string> aptitudes = new List<string>();
                if (!string.IsNullOrWhiteSpace(Aptitud1)) aptitudes.Add(Aptitud1);
                if (!string.IsNullOrWhiteSpace(Aptitud2)) aptitudes.Add(Aptitud2);
                if (!string.IsNullOrWhiteSpace(Aptitud3)) aptitudes.Add(Aptitud3);

                postulados.Aptitudes = string.Join(", ", aptitudes);

                // Guardamos en BD
                db.Postulados.Add(postulados);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(postulados);
        }


        // GET: Postulados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Postulados postulados = db.Postulados.Find(id);
            if (postulados == null)
            {
                return HttpNotFound();
            }
            return View(postulados);
        }

        // POST: Postulados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPostulado,Nombre,Propuestas,Aptitudes,Descripcion")] Postulados postulados)
        {
            if (ModelState.IsValid)
            {
                db.Entry(postulados).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(postulados);
        }

        // GET: Postulados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Postulados postulados = db.Postulados.Find(id);
            if (postulados == null)
            {
                return HttpNotFound();
            }
            return View(postulados);
        }

        // POST: Postulados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Postulados postulados = db.Postulados.Find(id);
            db.Postulados.Remove(postulados);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
