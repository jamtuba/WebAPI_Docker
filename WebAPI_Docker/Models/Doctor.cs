namespace Test_Dias.Models;

public class Doctor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Department Department { get; set; }
}