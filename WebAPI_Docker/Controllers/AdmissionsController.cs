using WebAPI_Docker.Controllers.Requests;
using WebAPI_Docker.Data;
using WebAPI_Docker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_Docker.Controllers;

[ApiController]
[Route("[controller]")]
public class AdmissionsController : ControllerBase
{
    private readonly WebApiDockerDbContext _dbContext;

    public AdmissionsController(WebApiDockerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Admission>>> Get()
    {
        return await _dbContext.Admissions
            .Include(a => a.Department)
            .Include(a => a.MedicalJournal)
            .Include(a => a.Doctor)
            .ToListAsync();
    }

    [HttpGet("{admissionId:guid}")]
    public async Task<ActionResult<Admission>> GetAdmissionById(Guid admissionId)
    {
        var admission = await _dbContext.Admissions
            .Include(a => a.Department)
            .Include(a => a.MedicalJournal)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == admissionId);

        if (admission is null)
            return NotFound($"No admission with the id: {admissionId}");

        return Ok(admission);
    }

    [HttpPost]
    public async Task<ActionResult<Admission>> CreateAdmission([FromForm] AdmissionRequest admissionRequest)
    {
        if (!AdmissionIsValid(admissionRequest))
            return BadRequest("Admission details required");

        var doctor = await _dbContext.Doctors.FindAsync(admissionRequest.DoctorId);

        var medicalJournal = await _dbContext.MedicalJournals.FindAsync(admissionRequest.MedicalJournalId);

        if (medicalJournal is null)
            return NotFound($"No medical journal with the id: {admissionRequest.MedicalJournalId}");

        var department = await _dbContext.Departments.FindAsync(admissionRequest.DepartmentId);

        if (department is null)
            return NotFound($"No department with the id: {admissionRequest.DepartmentId}");

        var admission = new Admission
        {
            Department = department,
            Doctor = doctor,
            MedicalJournal = medicalJournal
        };


        await _dbContext.Admissions.AddAsync(admission);
        await _dbContext.SaveChangesAsync();

        return Ok(admissionRequest);
    }

    [HttpPatch("{admissionId:guid}")]
    public async Task<ActionResult<Admission>> UpdateAdmission(Guid admissionId, [FromForm] AdmissionRequest admissionRequest)
    {
        if (!AdmissionIsValid(admissionRequest))
            return BadRequest("Admission details required");

        var admissionDb = await _dbContext.Admissions
            .Include(a => a.Department)
            .Include(a => a.MedicalJournal)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == admissionId);

        var doctor = await _dbContext.Doctors
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.Id == admissionRequest.DoctorId);

        var medicalJournal = await _dbContext.MedicalJournals.FindAsync(admissionRequest.MedicalJournalId);

        if (medicalJournal is null)
            return NotFound($"No medical journal with the id: {admissionRequest.MedicalJournalId}");

        var department = await _dbContext.Departments.FindAsync(admissionRequest.DepartmentId);

        if (department is null)
            return NotFound($"No department with the id: {admissionRequest.DepartmentId}");

        if (admissionDb is null)
            return NotFound($"No admission with the id: {admissionId}");

        admissionDb.Department = department;
        admissionDb.Doctor = doctor;
        admissionDb.MedicalJournal = medicalJournal;

        await _dbContext.SaveChangesAsync();

        return Ok(admissionDb);
    }

    [HttpDelete("{admissionId:guid}")]
    public async Task<ActionResult> DeleteAdmission(Guid admissionId)
    {
        var admission = await _dbContext.Admissions.FindAsync(admissionId);

        if (admission is null)
            return NotFound($"No admission with the id: {admissionId}");

        _dbContext.Admissions.Remove(admission);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }


    private static bool AdmissionIsValid(AdmissionRequest? admission) => admission != null && admission.MedicalJournalId is Guid && admission.DepartmentId is Guid;
}
