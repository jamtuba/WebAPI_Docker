namespace WebAPI_Docker.Models;

public class MedicalJournal
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string SSN { get; set; }
}