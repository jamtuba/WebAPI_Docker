namespace WebAPI_Docker.Models;

public class Admission
{
    public Guid Id { get; set; }
    public required Department Department { get; set; }
    public Doctor? Doctor { get; set; }
    public required MedicalJournal MedicalJournal { get; set; }
}
