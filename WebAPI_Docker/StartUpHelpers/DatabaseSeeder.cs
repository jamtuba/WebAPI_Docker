using Bogus;
using Bogus.Extensions.Denmark;
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

    private const int NUMBER_OF_ITEMS = 10;

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
        var randomNumber = new Random();

        var testDepartment = new Faker<Department>()
            .StrictMode(true)
            .RuleFor(d => d.Name, (f, d) => "Department " + f.IndexGlobal)
            .RuleFor(d => d.Id, (f, d) => Guid.NewGuid());
        var departments = AddList<Department>(testDepartment);

        var testDoctor = new Faker<Doctor>()
            .StrictMode(true)
            .RuleFor(d => d.Name, (f, d) => f.Name.FullName())
            .RuleFor(d => d.Department, (f, d) => departments[randomNumber.Next(0, 9)])
            .RuleFor(d => d.Id, (f, d) => Guid.NewGuid());

        var doctors = AddList<Doctor>(testDoctor);

        var testMedicalJournal = new Faker<MedicalJournal>()
            .StrictMode(true)
            .RuleFor(mj => mj.Name, (f, mj) => f.Name.FullName())
            .RuleFor(mj => mj.SSN, (f, mj) => f.Person.Cpr())
            .RuleFor(mj => mj.Id, (f, mj) => Guid.NewGuid());

        var medicalJournals = AddList<MedicalJournal>(testMedicalJournal);

        var testAdmission = new Faker<Admission>()
            .StrictMode(true)
            .RuleFor(a => a.Doctor, (f, a) => doctors[randomNumber.Next(0, 9)])
            .RuleFor(a => a.MedicalJournal, (f, a) => medicalJournals[randomNumber.Next(0, 9)])
            .RuleFor(a => a.Department, (f, a) => a.Doctor!.Department)
            .RuleFor(a => a.Id, (f, a) => Guid.NewGuid());

        var admissions = AddList<Admission>(testAdmission);

        _context.Departments.AddRange(departments);
        _context.Doctors.AddRange(doctors);
        _context.MedicalJournals.AddRange(medicalJournals);
        _context.Admissions.AddRange(admissions);

        await _context.SaveChangesAsync();
    }

    private static List<T> AddList<T>(Faker<T> fakerObject) where T : class
    {
        var listToReturn = new List<T>();
        for (int i = 0; i < NUMBER_OF_ITEMS; i++)
        {
            listToReturn.Add(fakerObject.Generate());
        }
        return listToReturn;
    }
}