using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VOTACIONES.Models;

namespace VOTACIONES.Controllers
{
    public class VotosController : Controller
    {
        private VOTACIONESEntities1 db = new VOTACIONESEntities1();

        // GET: Votos
        public ActionResult Index()
        {
            var votos = db.Votos.Include(v => v.Aprendiz).Include(v => v.Postulados);
            return View(votos.ToList());
        }

        // GET: Votos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Votos votos = db.Votos.Find(id);
            if (votos == null)
            {
                return HttpNotFound();
            }
            return View(votos);
        }

        // GET: Votos/Create
        public ActionResult Create()
        {
            ViewBag.IdAprendiz = new SelectList(db.Aprendiz, "id_aprendiz", "correo_aprendiz");
            ViewBag.IdPostulado = new SelectList(db.Postulados, "IdPostulado", "Nombre");
            return View();
        }

        // POST: Votos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdVoto,IdAprendiz,IdPostulado,FechaVoto")] Votos votos)
        {
            if (ModelState.IsValid)
            {
                db.Votos.Add(votos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdAprendiz = new SelectList(db.Aprendiz, "id_aprendiz", "correo_aprendiz", votos.IdAprendiz);
            ViewBag.IdPostulado = new SelectList(db.Postulados, "IdPostulado", "Nombre", votos.IdPostulado);
            return View(votos);
        }

        // GET: Votos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Votos votos = db.Votos.Find(id);
            if (votos == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAprendiz = new SelectList(db.Aprendiz, "id_aprendiz", "correo_aprendiz", votos.IdAprendiz);
            ViewBag.IdPostulado = new SelectList(db.Postulados, "IdPostulado", "Nombre", votos.IdPostulado);
            return View(votos);
        }

        // POST: Votos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdVoto,IdAprendiz,IdPostulado,FechaVoto")] Votos votos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(votos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAprendiz = new SelectList(db.Aprendiz, "id_aprendiz", "correo_aprendiz", votos.IdAprendiz);
            ViewBag.IdPostulado = new SelectList(db.Postulados, "IdPostulado", "Nombre", votos.IdPostulado);
            return View(votos);
        }

        // GET: Votos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Votos votos = db.Votos.Find(id);
            if (votos == null)
            {
                return HttpNotFound();
            }
            return View(votos);
        }

        // POST: Votos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Votos votos = db.Votos.Find(id);
            db.Votos.Remove(votos);
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

        [HttpPost]
        public ActionResult RegistrarVoto(int idPostulado)
        {
            try
            {
                // Recuperar el ID del aprendiz desde la sesión
                if (Session["AprendizId"] == null)
                {
                    return Json(new { success = false, message = "Tu sesión ha expirado. Vuelve a iniciar sesión." });
                }

                int idAprendiz = (int)Session["AprendizId"];

                // Verificar si el aprendiz ya votó
                var votoExistente = db.Votos.FirstOrDefault(v => v.IdAprendiz == idAprendiz);

                if (votoExistente != null)
                {
                    // Ya votó → devolver mensaje al modal
                    return Json(new { success = false, message = "Ya realizaste tu voto, no puedes volver a votar." });
                }

                // Crear el nuevo voto
                var voto = new Votos
                {
                    IdAprendiz = idAprendiz,
                    IdPostulado = idPostulado == 0 ? (int?)null : idPostulado, // 0 = voto en blanco
                    FechaVoto = DateTime.Now
                };

                db.Votos.Add(voto);
                db.SaveChanges();

                return Json(new { success = true, message = "¡Tu voto ha sido registrado con éxito!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al registrar el voto: " + ex.Message });
            }
        }




    }
}
