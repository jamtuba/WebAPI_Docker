using WebAPI_Docker.Data;
using WebAPI_Docker.Models;

namespace WebAPI_Docker.StartUpHelpers;

public interface IDatabaseSeeder
{
    Task Seed();
}

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly WebApiDockerDbContext _context;

    public DatabaseSeeder(WebApiDockerDbContext context)
    {
        _context = context;
    }

    public async Task Seed()
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        await SeedTestData();

        await transaction.CommitAsync();
    }

    private async Task SeedTestData()
    {
        var department1 = new Department()
        {
            Id = new Guid("f4b5d7a7-af57-49c7-a26e-95e9ccfbfe7b"),
            Name = "Department 1"
        };

        var department2 = new Department()
        {
            Id = new Guid("6349bb62-2fd5-4eb5-a483-f230a3b5dbe9"),
            Name = "Department 2"
        };

        var doctor1 = new Doctor()
        {
            Id = new Guid("65555e7b-cfc2-4234-908e-25c333318104"),
            Name = "Doctor 1",
            Department = department1
        };

        var doctor2 = new Doctor()
        {
            Id = new Guid("e25fa903-4274-483b-baba-be4433a6cb19"),
            Name = "Doctor 2",
            Department = department2
        };

        var medicalJournal1 = new MedicalJournal()
        {
            Id = new Guid("0b388da3-5f1e-4fe4-904f-e17aadc9583a"),
            Name = "Jens Hansen",
            SSN = "101084-1233"
        };

        var medicalJournal2 = new MedicalJournal()
        {
            Id = new Guid("de19ee5e-5731-40f5-a214-2dd11eb5b972"),
            Name = "Jenny Olsen",
            SSN = "111280-1234"
        };

        var admission1 = new Admission()
        {
            Id = new Guid("68c5da57-082c-40f5-a2fc-1c12acd5795a"),
            Doctor = doctor1,
            MedicalJournal = medicalJournal1,
            Department = department1,
        };

        var admission2 = new Admission()
        {
            Id = new Guid("e783c9d9-d262-4c96-aae7-21b5c75c2f30"),
            MedicalJournal = medicalJournal2,
            Department = department2,
        };

        _context.Departments.AddRange([department1, department2]);
        _context.Doctors.AddRange([doctor1, doctor2]);
        _context.MedicalJournals.AddRange([medicalJournal1, medicalJournal2]);
        _context.Admissions.AddRange([admission1, admission2]);

        await _context.SaveChangesAsync();
    }
}

