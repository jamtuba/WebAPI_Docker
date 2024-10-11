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

        var departments = AddDepartment(testDepartment);

        var testDoctor = new Faker<Doctor>()
            .StrictMode(true)
            .RuleFor(d => d.Name, (f, d) => f.Name.FullName())
            .RuleFor(d => d.Department, (f, d) => departments[randomNumber.Next(0, 9)])
            .RuleFor(d => d.Id, (f, d) => Guid.NewGuid());

        var doctors = AddDoctors(testDoctor);

        var testMedicalJournal = new Faker<MedicalJournal>()
            .StrictMode(true)
            .RuleFor(mj => mj.Name, (f, mj) => f.Name.FullName())
            .RuleFor(mj => mj.SSN, (f, mj) => f.Person.Cpr())
            .RuleFor(mj => mj.Id, (f, mj) => Guid.NewGuid());

        var medicalJournals = AddMedicalJournals(testMedicalJournal);

        var testAdmission = new Faker<Admission>()
            .StrictMode(true)
            .RuleFor(a => a.Doctor, (f, a) => doctors[randomNumber.Next(0, 9)])
            .RuleFor(a => a.MedicalJournal, (f, a) => medicalJournals[randomNumber.Next(0, 9)])
            .RuleFor(a => a.Department, (f, a) => a.Doctor.Department)
            .RuleFor(a => a.Id, (f, a) => Guid.NewGuid());

        var admissions = AddAdmissions(testAdmission);

        _context.Departments.AddRange(departments);
        _context.Doctors.AddRange(doctors);
        _context.MedicalJournals.AddRange(medicalJournals);
        _context.Admissions.AddRange(admissions);

        await _context.SaveChangesAsync();
    }

    private static List<Department> AddDepartment(Faker<Department> department)
    {
        var departmentList = new List<Department>();
        for (int i = 0; i < NUMBER_OF_ITEMS; i++)
        {
            departmentList.Add(department.Generate());
        }

        return departmentList;
    }

    private static List<Doctor> AddDoctors(Faker<Doctor> doctor)
    {
        var docList = new List<Doctor>();
        for (int i = 0; i < NUMBER_OF_ITEMS; i++)
        {
            docList.Add(doctor.Generate());
        }

        return docList;
    }
    private static List<MedicalJournal> AddMedicalJournals(Faker<MedicalJournal> medicalJournal)
    {
        var medicalJournalList = new List<MedicalJournal>();
        for (int i = 0; i < NUMBER_OF_ITEMS; i++)
        {
            medicalJournalList.Add(medicalJournal.Generate());
        }

        return medicalJournalList;
    }

    private static List<Admission> AddAdmissions(Faker<Admission> admission)
    {
        var admissionList = new List<Admission>();
        for (int i = 0; i < NUMBER_OF_ITEMS; i++)
        {
            admissionList.Add(admission.Generate());
        }

        return admissionList;
    }   
}