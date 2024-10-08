using Microsoft.EntityFrameworkCore;
using WebAPI_Docker.Models;

namespace WebAPI_Docker.Data;

public class WebApiDockerDbContext : DbContext
{
    public WebApiDockerDbContext(DbContextOptions<WebApiDockerDbContext> options) : base(options) { }

    public DbSet<Admission> Admissions { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<MedicalJournal> MedicalJournals { get; set; }

}