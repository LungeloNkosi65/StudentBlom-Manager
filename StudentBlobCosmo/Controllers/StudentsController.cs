using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using StudentBloCosmo.Service;
using StudentBloCosmo.ViewModel;

namespace StudentBlobCosmo.Controllers
{
    public class StudentsController : Controller
    {
        CosmodbBussines cosmodb = new CosmodbBussines();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public async Task<ActionResult> Index(string stName)
        {
            List<Student> students = await cosmodb.StudentsList(stName);
            return View(students);
        }

        public ActionResult Delete(string stName)
        {
            return View(cosmodb.DeleteStudent(stName));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Bind(Exclude = "UserPhoto")] Student student)
        {
            HttpPostedFileBase uploadFile = Request.Files["UserPhoto"];
            if (uploadFile.ContentLength != 0)
            {
               await cosmodb.AddStudent(uploadFile,student);
                return RedirectToAction("Index");
            }
            else
            {

                ModelState.AddModelError(string.Empty, "Please Select Picture");

                return View(student);
            }
        }

        public ActionResult Edit(string DocumentId)
        {
            return View(cosmodb.EditStudent(DocumentId));
        }

        public async Task<ActionResult> De_Activate(string documentId)
        {
            await cosmodb.Deactivate(documentId);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> DeLete(string documentId)
        {
            await cosmodb.DeleteStudent(documentId);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> UpdateAsync(Student Studentz)
        {
            await cosmodb.UpdateAsync(Studentz);
            return RedirectToAction("Index");
        }
    }
}
