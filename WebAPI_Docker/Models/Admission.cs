namespace Test_Dias.Models;

public class Admission
{
    public Guid Id { get; set; }
    public Department Department { get; set; }
    public Doctor? Doctor { get; set; }
    public MedicalJournal MedicalJournal { get; set; }
}
