using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KPPatients.Models;

namespace KPPatients.Controllers
{
    public class KPPatientTreatmentController : Controller
    {
        private readonly PatientsContext _context;

        public KPPatientTreatmentController(PatientsContext context)
        {
            _context = context;
        }

        // GET: KPPatientTreatment
        public async Task<IActionResult> Index(int PatientDiagnosisId, string PatientName, string DiagnosisName)
        {
            if (PatientDiagnosisId > 0)
            {
                //store in cookies or session
                Response.Cookies.Append("PatientDiagnosisId", PatientDiagnosisId.ToString());
            }
            else if (Request.Query["PatientDiagnosisId"].Any())
            {
                //store in cookies or session
                PatientDiagnosisId = Convert.ToInt32(Request.Query["PatientDiagnosisId"]);
            }
            else if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Convert.ToInt32(Request.Cookies["PatientDiagnosisId"]);
            }
            else
            {
                TempData["message"] = "Select patient's diagnosis.";
                return RedirectToAction("Index", "DPPatientDiagnosis");
            }

            var patientsContext = _context.PatientTreatments.Include(p => p.PatientDiagnosis).Include(p => p.Treatment);
            ViewData["DiagnosisName"] = DiagnosisName;
            ViewData["PatientName"] = PatientName;

            return View(await patientsContext.ToListAsync());
        }

        // GET: KPPatientTreatment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PatientTreatments == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatments
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            var patientDignosisContext = _context.PatientDiagnoses.Include(p => p.Diagnosis).Include(p => p.Patient)
                                  .Where(x => x.PatientDiagnosisId == patientTreatment.PatientDiagnosisId).FirstOrDefault();

            ViewData["DiagnosisName"] = patientDignosisContext.Diagnosis.Name;
            ViewData["PatientName"] = patientDignosisContext.Patient.LastName + ", " + patientDignosisContext.Patient.FirstName;

            return View(patientTreatment);
        }

        // GET: KPPatientTreatment/Create
        public IActionResult Create()
        {
            int PatientDiagnosisId = 0;
            if (Request.Query["PatientDiagnosisId"].Any())
            {
                PatientDiagnosisId = Convert.ToInt32(Request.Query["PatientDiagnosisId"]);
            }
            else if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Convert.ToInt32(Request.Cookies["PatientDiagnosisId"]);
            }
            else
            {
                TempData["message"] = "Select a patient’s diagnosis.";
                return RedirectToAction("Index", "DPPatientDiagnosis");
            }

            var patientDignosisContext = _context.PatientDiagnoses.Include(p => p.Diagnosis).Include(p => p.Patient)
                                  .Where(x => x.PatientDiagnosisId == PatientDiagnosisId).FirstOrDefault();

            ViewData["DiagnosisName"] = patientDignosisContext.Diagnosis.Name;
            ViewData["PatientName"] = patientDignosisContext.Patient.LastName + ", " + patientDignosisContext.Patient.FirstName;


            int dignosisId = _context.PatientDiagnoses.Where(x => x.PatientDiagnosisId == PatientDiagnosisId).FirstOrDefault().DiagnosisId;

            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnoses, "PatientDiagnosisId", "PatientDiagnosisId");
            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "TreatmentId");

            PatientTreatment patientTreatment = new PatientTreatment();
            patientTreatment.DatePrescribed = DateTime.Now;
            return View();
        }

        // POST: KPPatientTreatment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            int PatientDiagnosisId = 0;
            if (Request.Query["PatientDiagnosisId"].Any())
            {
                //store in cookies or session
                PatientDiagnosisId = Convert.ToInt32(Request.Query["PatientDiagnosisId"]);
            }
            else if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Convert.ToInt32(Request.Cookies["PatientDiagnosisId"]);
            }

            patientTreatment.PatientDiagnosisId = PatientDiagnosisId;
            if (ModelState.IsValid)
            {
                _context.Add(patientTreatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnoses, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            int dignosisId = _context.PatientDiagnoses.Where(x => x.PatientDiagnosisId == PatientDiagnosisId).FirstOrDefault().DiagnosisId;

            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "TreatmentId", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // GET: KPPatientTreatment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int PatientDiagnosisId = 0;

            if (Request.Query["PatientDiagnosisId"].Any())
            {
                //store in cookies or session
                PatientDiagnosisId = Convert.ToInt32(Request.Query["PatientDiagnosisId"]);
            }
            else if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Convert.ToInt32(Request.Cookies["PatientDiagnosisId"]);
            }

            if (id == null || _context.PatientTreatments == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatments.FindAsync(id);
            if (patientTreatment == null)
            {
                return NotFound();
            }
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnoses, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            int dignosisId = _context.PatientDiagnoses.Where(x => x.PatientDiagnosisId == PatientDiagnosisId).FirstOrDefault().DiagnosisId;

            var patientDignosisContext = _context.PatientDiagnoses.Include(p => p.Diagnosis).Include(p => p.Patient)
                                  .Where(x => x.PatientDiagnosisId == PatientDiagnosisId).FirstOrDefault();

            ViewData["DiagnosisName"] = patientDignosisContext.Diagnosis.Name;
            ViewData["PatientName"] = patientDignosisContext.Patient.LastName + ", " + patientDignosisContext.Patient.FirstName;

            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "TreatmentId", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // POST: KPPatientTreatment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            int PatientDiagnosisId = 0;
            if (Request.Query["PatientDiagnosisId"].Any())
            {
                //store in cookies or session
                PatientDiagnosisId = Convert.ToInt32(Request.Query["PatientDiagnosisId"]);
            }
            else if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Convert.ToInt32(Request.Cookies["PatientDiagnosisId"]);
            }


            patientTreatment.PatientDiagnosisId = PatientDiagnosisId;

            if (id != patientTreatment.PatientTreatmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientTreatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientTreatmentExists(patientTreatment.PatientTreatmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnoses, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            int dignosisId = _context.PatientDiagnoses.Where(x => x.PatientDiagnosisId == PatientDiagnosisId).FirstOrDefault().DiagnosisId;

            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "TreatmentId", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // GET: KPPatientTreatment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PatientTreatments == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatments
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            return View(patientTreatment);
        }

        // POST: KPPatientTreatment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PatientTreatments == null)
            {
                return Problem("Entity set 'PatientsContext.PatientTreatments'  is null.");
            }
            var patientTreatment = await _context.PatientTreatments.FindAsync(id);
            if (patientTreatment != null)
            {
                _context.PatientTreatments.Remove(patientTreatment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientTreatmentExists(int id)
        {
          return _context.PatientTreatments.Any(e => e.PatientTreatmentId == id);
        }
    }
}
