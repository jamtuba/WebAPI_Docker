using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Docker.Controllers.Requests;
using WebAPI_Docker.Data;
using WebAPI_Docker.Models;

namespace Dias_Backend_CodeChallenge.Controllers;
[Route("[controller]")]
[ApiController]
public class DoctorsController : ControllerBase
{
    private readonly WebApiDockerDbContext _dbContext;

    public DoctorsController(WebApiDockerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
    {
        return await _dbContext.Doctors
            .Include(d => d.Department)
            .ToListAsync();
    }

    [HttpGet("{doctorId:guid}")]
    public async Task<ActionResult<Doctor>> GetAdmissionById(Guid doctorId)
    {
        var doctor = await _dbContext.Doctors
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.Id == doctorId);

        if (doctor is null)
            return NotFound($"No doctor with the id: {doctorId}");

        return Ok(doctor);
    }

    [HttpPost]
    public async Task<ActionResult<Doctor>> CreateDoctor([FromForm] DoctorRequest doctorRequest)
    {
        if (!DoctorIsValid(doctorRequest))
            return BadRequest("Doctor details required");

        var department = await _dbContext.Departments.FindAsync(doctorRequest.DepartmentId);

        if (department is null)
            return NotFound($"No department with the id: {doctorRequest.DepartmentId}");

        var doctor = new Doctor
        {
            Name = doctorRequest.Name,
            Department = department
        };

        await _dbContext.Doctors.AddAsync(doctor);
        await _dbContext.SaveChangesAsync();

        return Ok(doctor);
    }

    [HttpPatch("{doctorId:guid}")]
    public async Task<ActionResult<Doctor>> UpdateDoctor(Guid doctorId, [FromForm] DoctorRequest doctorRequest)
    {
        if (DoctorIsValid(doctorRequest))
            return BadRequest("Doctor details required");
        var department = await _dbContext.Departments.FindAsync(doctorRequest.DepartmentId);

        if (department is null)
            return NotFound($"No department with the id: {doctorRequest.DepartmentId}");

        var doctorDb = await _dbContext.Doctors.FindAsync(doctorId);

        if (doctorDb is null)
            return NotFound($"No doctor with the id: {doctorId}");

        doctorDb.Name = doctorRequest.Name;
        doctorDb.Department.Id = doctorRequest.DepartmentId ?? Guid.Empty;
        await _dbContext.SaveChangesAsync();

        return Ok(doctorDb);
    }

    [HttpDelete("{doctorId:guid}")]
    public async Task<ActionResult> DeleteDoctor(Guid doctorId)
    {
        if (doctorId == Guid.Empty)
            return BadRequest("Doctor id required");

        var doctor = await _dbContext.Doctors.FindAsync(doctorId);

        if (doctor is null)
            return NotFound($"No doctor with the id: {doctorId}");

        _dbContext.Doctors.Remove(doctor);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    private static bool DoctorIsValid(DoctorRequest? doctorRequest) => doctorRequest is not null && !string.IsNullOrWhiteSpace(doctorRequest.Name);
}
