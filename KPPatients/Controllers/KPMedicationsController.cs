using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KPPatients.Models;
using System.Xml.Linq;

namespace KPPatients.Controllers
{
    public class KPMedicationsController : Controller
    {
        private readonly PatientsContext _context;

        public int MedicationTypeId { get; private set; }

        public KPMedicationsController(PatientsContext context)
        {
            _context = context;
        }

        // GET: KPMedications
        public async Task<IActionResult> Index(int MedicationTypeId)
        {
            if (MedicationTypeId > 0)
            {
                //store in cookies or session
                Response.Cookies.Append("MedicationTypeId", MedicationTypeId.ToString());
            }
            else if (Request.Query["MedicationTypeId"].Any())
            {
                //store in cookies or session
                MedicationTypeId = Convert.ToInt32(Request.Query["MedicationTypeId"]);
            }
            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Convert.ToInt32(Request.Cookies["MedicationTypeId"]);
            }
            else
            {
                TempData["message"] = "Select medication type";
                return RedirectToAction("Index", "KPMedicationType");
            }

            var patientsContext = _context.Medications.Include(m => m.ConcentrationCodeNavigation).Include(m => m.DispensingCodeNavigation)
                                  .Include(m => m.MedicationType)
                                  .Where(m => m.MedicationTypeId == MedicationTypeId)
                                  .OrderBy(m => m.Name)
                                  .ThenBy(m => m.Concentration);

            var medicationType = _context.MedicationTypes.Where(x => x.MedicationTypeId == MedicationTypeId).FirstOrDefault();

            ViewData["MedicationTypeName"] = medicationType.Name;

            return View(await patientsContext.ToListAsync());
        }


        // GET: KPMedications/Details/5
        public async Task<IActionResult> Details(string id)
        {
            int MedicationTypeId = 0;
            if (Request.Query["MedicationTypeId"].Any())
            {
                //store in cookies or session
                MedicationTypeId = Convert.ToInt32(Request.Query["MedicationTypeId"]);
            }
            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Convert.ToInt32(Request.Cookies["MedicationTypeId"]);
            }

            var medicationType = _context.MedicationTypes.Where(x => x.MedicationTypeId == MedicationTypeId).FirstOrDefault();

            ViewData["MedicationTypeName"] = medicationType.Name;

            if (id == null || _context.Medications == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications.Include(m => m.ConcentrationCodeNavigation).Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }

            ViewData["MedicationTypeName"] = medicationType.Name;
            return View(medication);
        }

        // GET: KPMedications/Create
        public IActionResult Create()
        {
            int MedicationTypeId = 0;
            if (Request.Query["MedicationTypeId"].Any())
            {
                //store in cookies or session
                MedicationTypeId = Convert.ToInt32(Request.Query["MedicationTypeId"]);
            }
            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Convert.ToInt32(Request.Cookies["MedicationTypeId"]);
            }
            else
            {
                TempData["message"] = "Select medication type to see its medications.";
                return RedirectToAction("Index", "KPMedicationTypes");
            }

            var medicationType = _context.MedicationTypes.Where(x => x.MedicationTypeId == MedicationTypeId).FirstOrDefault();

            ViewData["MedicationTypeName"] = medicationType.Name;

            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationTypes, "MedicationTypeId", "MedicationTypeId");
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnits.OrderBy(x => x.ConcentrationCode), "ConcentrationCode", "ConcentrationCode");
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnits.OrderBy(x => x.DispensingCode), "DispensingCode", "DispensingCode");

            ViewData["MedicationTypeName"] = medicationType.Name;
            return View();
        }


        // POST: KPMedications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            int MedicationTypeId = 0;
            if (Request.Query["MedicationTypeId"].Any())
            {
                //store in cookies or session
                MedicationTypeId = Convert.ToInt32(Request.Query["MedicationTypeId"]);
            }
            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Convert.ToInt32(Request.Cookies["MedicationTypeId"]);
            }

            medication.MedicationTypeId = MedicationTypeId;

            var isExistDuplicate = _context.Medications.Where(x => x.Name == medication.Name
                                    && x.Concentration == medication.Concentration
                                    && x.ConcentrationCode == medication.ConcentrationCode);

            if (isExistDuplicate.Any())
            {
                ModelState.AddModelError("", "Already exists");
            }

            if (ModelState.IsValid)
            {
                _context.Add(medication);
                await _context.SaveChangesAsync();
                TempData["message"] = "Medication saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnits.OrderBy(x => x.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnits.OrderBy(x => x.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationTypes, "MedicationTypeId", "MedicationTypeId", medication.MedicationTypeId);

            return View(medication);
        }

        // GET: KPMedications/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            int MedicationTypeId = 0;
            if (Request.Query["MedicationTypeId"].Any())
            {
                //store in cookies or session
                MedicationTypeId = Convert.ToInt32(Request.Query["MedicationTypeId"]);
            }
            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Convert.ToInt32(Request.Cookies["MedicationTypeId"]);
            }

            if (id == null || _context.Medications == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications.FindAsync(id);

            var medicationType = _context.MedicationTypes.Where(x => x.MedicationTypeId == MedicationTypeId).FirstOrDefault();

            if (medication == null)
            {
                return NotFound();
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnits, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnits, "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationTypes, "MedicationTypeId", "MedicationTypeId", medication.MedicationTypeId);

            ViewData["MedicationTypeName"] = medicationType.Name;
            return View(medication);
        }
        // POST: KPMedications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            int MedicationTypeId = 0;
            if (Request.Query["MedicationTypeId"].Any())
            {
                //store in cookies or session
                MedicationTypeId = Convert.ToInt32(Request.Query["MedicationTypeId"]);
            }
            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Convert.ToInt32(Request.Cookies["MedicationTypeId"]);
            }

            medication.MedicationTypeId = MedicationTypeId;

            if (id != medication.Din)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.Din))
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
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnits, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnits, "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationTypes, "MedicationTypeId", "MedicationTypeId", medication.MedicationTypeId);
            
            return View(medication);
        }
        // GET: KPMedications/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            int MedicationTypeId = 0;
            if (Request.Query["MedicationTypeId"].Any())
            {
                //store in cookies or session
                MedicationTypeId = Convert.ToInt32(Request.Query["MedicationTypeId"]);
            }
            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Convert.ToInt32(Request.Cookies["MedicationTypeId"]);
            }

            var medicationType = _context.MedicationTypes.Where(x => x.MedicationTypeId == MedicationTypeId).FirstOrDefault();

            ViewData["MedicationTypeName"] = medicationType.Name;

            if (id == null || _context.Medications == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }
            
            ViewData["MedicationTypeName"] = medicationType.Name;
            return View(medication);
        }

        // POST: KPMedications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Medications == null)
            {
                return Problem("Entity set 'PatientsContext.Medications'  is null.");
            }
            var medication = await _context.Medications.FindAsync(id);
            if (medication != null)
            {
                _context.Medications.Remove(medication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationExists(string id)
        {
            return _context.Medications.Any(e => e.Din == id);
        }
    }
}
