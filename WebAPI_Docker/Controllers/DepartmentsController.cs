using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Docker.Data;
using WebAPI_Docker.Models;

namespace WebAPI_Docker.Controllers;
[Route("[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly WebApiDockerDbContext _dbContext;

    public DepartmentsController(WebApiDockerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
    {
        return await _dbContext.Departments.ToListAsync();
    }

    [HttpGet("{departmentId:guid}")]
    public async Task<ActionResult<Department>> GetAdmissionById(Guid departmentId)
    {
        var department = await _dbContext.Departments.FindAsync(departmentId);

        if (!DepartmentIsValid(department))
            return NotFound($"No department with the id: {departmentId}");

        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment([FromForm] string departmentName)
    {
        if (string.IsNullOrWhiteSpace(departmentName))
            return BadRequest("Department details required");

        var department = new Department { Name = departmentName };

        await _dbContext.Departments.AddAsync(department);
        await _dbContext.SaveChangesAsync();

        return Ok(department);
    }

    [HttpPatch("{departmentId:guid}")]
    public async Task<ActionResult<Department>> UpdateDepartment(Guid departmentId, [FromForm] string departmentName)
    {
        if (string.IsNullOrWhiteSpace(departmentName))
            return BadRequest("Department details required");

        var departmentDb = await _dbContext.Departments.FindAsync(departmentId);

        if (departmentDb is null)
            return NotFound($"No department with the id: {departmentId}");

        departmentDb.Name = departmentName;
        await _dbContext.SaveChangesAsync();

        return Ok(departmentDb);
    }

    [HttpDelete("{departmentId:guid}")]
    public async Task<ActionResult> DeleteDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return BadRequest("Department id required");

        var department = await _dbContext.Departments.FindAsync(departmentId);

        if (department is null)
            return NotFound($"No department with the id: {departmentId}");

        _dbContext.Departments.Remove(department);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    private static bool DepartmentIsValid(Department? department) => department is not null && !string.IsNullOrWhiteSpace(department.Name);
}
