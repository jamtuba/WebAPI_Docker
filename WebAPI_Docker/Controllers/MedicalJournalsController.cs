using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Docker.Data;
using WebAPI_Docker.Models;

namespace Dias_Backend_CodeChallenge.Controllers;
[Route("[controller]")]
[ApiController]
public class MedicalJournalsController : ControllerBase
{
    private readonly WebApiDockerDbContext _dbContext;

    public MedicalJournalsController(WebApiDockerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicalJournal>>> GetMedicalJournals()
    {
        return await _dbContext.MedicalJournals.ToListAsync();
    }

    [HttpGet("{medicalJournalId:guid}")]
    public async Task<ActionResult<MedicalJournal>> GetMedicalJournalId(Guid medicalJournalId)
    {
        var medicalJournal = await _dbContext.MedicalJournals.FindAsync(medicalJournalId);

        if (!MedicalJournalIsValid(medicalJournal))
            return NotFound($"No medical journal with the id: {medicalJournalId}");

        return Ok(medicalJournal);
    }

    [HttpGet("patient")]
    public async Task<ActionResult<MedicalJournal>> GetMedicalJournalId([FromQuery] string patientSsn, [FromQuery] string doctorId)
    {
        var medicalJournal = await _dbContext.MedicalJournals
            .FirstOrDefaultAsync(m => m.SSN == patientSsn);

        if (!MedicalJournalIsValid(medicalJournal))
            return NotFound($"No medical journal with the ssn: {patientSsn}");

        var admission = await _dbContext.Admissions
            .Include(a => a.Doctor)
            .Include(a => a.MedicalJournal)
            .FirstOrDefaultAsync(a => a.MedicalJournal.Id == medicalJournal!.Id && a.Doctor!.Id == Guid.Parse(doctorId));

        if (admission == null)
            return BadRequest("The doctor is not authorized to view the medical journal");

        return Ok(medicalJournal);
    }

    [HttpPost]
    public async Task<ActionResult<MedicalJournal>> CreateMedicalJournal([FromForm] MedicalJournal medicalJournal)
    {
        if (!MedicalJournalIsValid(medicalJournal))
            return BadRequest("Medical Journal details required");

        await _dbContext.MedicalJournals.AddAsync(medicalJournal);
        await _dbContext.SaveChangesAsync();

        return Ok(medicalJournal);
    }

    [HttpPatch("{medicalJournalId:guid}")]
    public async Task<ActionResult<MedicalJournal>> UpdateMedicalJournal(Guid medicalJournalId, [FromForm] MedicalJournal medicalJournal)
    {
        if (MedicalJournalIsValid(medicalJournal))
            return BadRequest("Medical Journal details required");

        var medicalJournalDb = await _dbContext.MedicalJournals.FindAsync(medicalJournalId);

        if (medicalJournalDb is null)
            return NotFound($"No medical journal with the id: {medicalJournalId}");

        medicalJournalDb.Name = medicalJournal.Name;
        medicalJournalDb.SSN = medicalJournal.SSN;
        await _dbContext.SaveChangesAsync();

        return Ok(medicalJournalDb);
    }

    [HttpDelete("{medicalJournalId:guid}")]
    public async Task<ActionResult> DeleteMedicalJournal(Guid medicalJournalId)
    {
        if (medicalJournalId == Guid.Empty)
            return BadRequest("Medical Journal id required");

        var medicalJournal = await _dbContext.MedicalJournals.FindAsync(medicalJournalId);

        if (medicalJournal is null)
            return NotFound($"No medical journal with the id: {medicalJournalId}");

        _dbContext.MedicalJournals.Remove(medicalJournal);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    private static bool MedicalJournalIsValid(MedicalJournal? medicalJournal) => medicalJournal is not null && !string.IsNullOrWhiteSpace(medicalJournal.Name) && !string.IsNullOrWhiteSpace(medicalJournal.SSN);
}
