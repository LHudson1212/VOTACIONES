using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VOTACIONES.Models;

namespace VOTACIONES.Controllers
{
    public class AprendizsController : Controller
    {
        private VOTACIONESEntities1 db = new VOTACIONESEntities1();

        // GET: Aprendizs
        public ActionResult Index()
        {
            return View(db.Aprendiz.ToList());
        }

        // GET: Aprendizs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aprendiz aprendiz = db.Aprendiz.Find(id);
            if (aprendiz == null)
            {
                return HttpNotFound();
            }
            return View(aprendiz);
        }

        // GET: Aprendizs/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Documento")] Aprendiz aprendiz)
        {
            if (ModelState.IsValid)
            {
                db.Aprendiz.Add(aprendiz);
                db.SaveChanges();
                TempData["Mensaje"] = "✅ Aprendiz registrado correctamente.";
               ModelState.Clear(); // Limpiar el formulario después de guardar
                return View(new Aprendiz());
            }

            TempData["Error"] = "⚠ Ocurrió un error al registrar el aprendiz.";
            return View(aprendiz);
        }

        // GET: Aprendizs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aprendiz aprendiz = db.Aprendiz.Find(id);
            if (aprendiz == null)
            {
                return HttpNotFound();
            }
            return View(aprendiz);
        }

        // POST: Aprendizs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_aprendiz,correo_aprendiz,contraseña_aprendiz")] Aprendiz aprendiz)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aprendiz).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aprendiz);
        }

        // GET: Aprendizs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aprendiz aprendiz = db.Aprendiz.Find(id);
            if (aprendiz == null)
            {
                return HttpNotFound();
            }
            return View(aprendiz);
        }

        // POST: Aprendizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Aprendiz aprendiz = db.Aprendiz.Find(id);
            db.Aprendiz.Remove(aprendiz);
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

        public ActionResult Seleccion()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Votar(int postuladoId)
        {
            try
            {
                // 1. Obtener el ID del aprendiz desde la sesión
                var aprendizId = Session["AprendizId"] as int?;
                if (aprendizId == null)
                {
                    TempData["Error"] = "⚠ Debes iniciar sesión para votar.";
                    return RedirectToAction("Login", "Home");
                }

                // 2. Buscar el postulado seleccionado
                var postulado = db.Postulados.Find(postuladoId);
                if (postulado == null)
                {
                    TempData["Error"] = "❌ El postulado no existe.";
                    return RedirectToAction("Diurna");
                }

                // 3. Verificar si el aprendiz ya votó (en cualquier jornada)
                bool yaVoto = db.Votos.Any(v => v.IdAprendiz == aprendizId.Value);

                if (yaVoto)
                {
                    TempData["VotoExistente"] = true;
                    return RedirectToAction(postulado.Jornada); // Redirige a la misma vista donde está
                }

                // 4. Guardar voto
                var voto = new Votos
                {
                    IdAprendiz = aprendizId.Value,
                    IdPostulado = postuladoId,
                    FechaVoto = DateTime.Now
                };

                postulado.CantidadVotos += 1;

                db.Votos.Add(voto);
                db.SaveChanges();

                TempData["VotoExitoso"] = true;
                return RedirectToAction(postulado.Jornada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Error al registrar el voto: " + ex.Message;
                return RedirectToAction("Diurna");
            }
        }


        public ActionResult Diurna()
        {
            // Traemos solo los postulados de la jornada diurna
            var candidatos = db.Postulados
                               .Where(p => p.Jornada == "Diurna")
                               .ToList();

            return View(candidatos);
        }

        public ActionResult Mixta()
        {
            // Traemos solo los postulados de la jornada diurna
            var candidatos = db.Postulados
                               .Where(p => p.Jornada == "Mixta")
                               .ToList();

            return View(candidatos);
        }
        public ActionResult Nocturna()
        {
            // Traemos solo los postulados de la jornada diurna
            var candidatos = db.Postulados
                               .Where(p => p.Jornada == "Nocturna")
                               .ToList();

            return View(candidatos);
        }


        [HttpGet]
        public ActionResult CargarExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargarExcel(HttpPostedFileBase archivoExcel)
        {
            if (archivoExcel != null && archivoExcel.ContentLength > 0)
            {
                // Ruta fija en el PC (ejemplo Escritorio o carpeta Importaciones)
                string rutaCarpeta = @"C:\Importaciones";

                // Crear la carpeta si no existe
                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                // Guardar el archivo en esa carpeta
                string ruta = Path.Combine(rutaCarpeta, Path.GetFileName(archivoExcel.FileName));
                archivoExcel.SaveAs(ruta);

                // Conexión Excel
                string conexionExcel = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={ruta};Extended Properties=\"Excel 12.0 Xml;HDR=YES\";";

                using (OleDbConnection conn = new OleDbConnection(conexionExcel))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Hoja1$]", conn);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        string documento = row["DocumentoIdentidad"].ToString();

                        if (!string.IsNullOrEmpty(documento) && !db.Aprendiz.Any(a => a.DocumentoIdentidad == documento))
                        {
                            var aprendiz = new Aprendiz
                            {
                                DocumentoIdentidad = documento,
                              
                            };

                            db.Aprendiz.Add(aprendiz);
                        }
                    }

                    db.SaveChanges();
                }

                TempData["Mensaje"] = "✅ Importación realizada correctamente.";
                ModelState.Clear(); // Limpiar el formulario después de guardar
                return View(new Aprendiz());
            }

            TempData["Error"] = "⚠ Debes seleccionar un archivo válido.";
            return RedirectToAction("CargarExcel");
        }
    }
}



