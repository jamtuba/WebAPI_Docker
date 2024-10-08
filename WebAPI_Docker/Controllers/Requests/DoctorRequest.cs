namespace WebAPI_Docker.Controllers.Requests;

public class DoctorRequest
{
    public string Name { get; set; }
    public Guid? DepartmentId { get; set; }
}
