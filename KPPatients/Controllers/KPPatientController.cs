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
    public class KPPatientController : Controller
    {
        private readonly PatientsContext _context;

        public KPPatientController(PatientsContext context)
        {
            _context = context;
        }

        // GET: KPPatient
        public async Task<IActionResult> Index()
        {
            var patientsContext = _context.Patients.Include(p => p.ProvinceCodeNavigation).OrderBy(x => x.LastName).ThenBy(x => x.FirstName); 
            return View(await patientsContext.ToListAsync());
        }

        // GET: KPPatient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: KPPatient/Create
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Provinces, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: KPPatient/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {
            try
            {
                patient.FirstName = patient.FirstName.Trim();
                patient.LastName = patient.LastName.Trim();
                patient.Address = patient.Address.Trim();
                patient.City = patient.City.Trim();
                patient.Gender = patient.Gender.Trim();
                patient.ProvinceCode = patient.ProvinceCode.Trim();
                if (ModelState.IsValid)
                {
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    @TempData["message"] = "Record created successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception occurred while creating patient");
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Provinces, "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // GET: KPPatient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Provinces.OrderBy(x => x.Name).ToList(), "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // POST: KPPatient/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {
            try
            {
                patient.FirstName = patient.FirstName.Trim();
                patient.LastName = patient.LastName.Trim();
                patient.Address = patient.Address.Trim();
                patient.City = patient.City.Trim();
                patient.Gender = patient.Gender.Trim();
                patient.ProvinceCode = patient.ProvinceCode.Trim();
                if (id != patient.PatientId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(patient);
                        @TempData["message"] = "Record updated successfully";
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PatientExists(patient.PatientId))
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
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception occurred while updating patient");
            }

            ViewData["ProvinceCode"] = new SelectList(_context.Provinces, "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // GET: KPPatient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: KPPatient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Patients == null)
                {
                    return Problem("Entity set 'PatientsContext.Patients'  is null.");
                }
                var patients = await _context.Patients.FindAsync(id);
                if (patients != null)
                {
                    _context.Patients.Remove(patients);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                @TempData["message"] = "Error while deleting the patient";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
            @TempData["message"] = "Record deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
          return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
