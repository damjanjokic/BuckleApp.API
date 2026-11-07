namespace BuckleApp.Core.Entities;

public class Organisation : BaseModel
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
} 