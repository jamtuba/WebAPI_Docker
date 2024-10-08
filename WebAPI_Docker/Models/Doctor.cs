namespace WebAPI_Docker.Models;

public class Doctor
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required Department Department { get; set; }
} 