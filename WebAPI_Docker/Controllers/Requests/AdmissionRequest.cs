namespace WebAPI_Docker.Controllers.Requests;

public class AdmissionRequest
{
    public Guid DepartmentId { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid MedicalJournalId { get; set; }
}
