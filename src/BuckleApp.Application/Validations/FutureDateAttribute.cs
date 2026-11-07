using System.ComponentModel.DataAnnotations;

namespace BuckleApp.Application.Validations;

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime dateTime)
        {
            return dateTime > DateTime.UtcNow;
        }
        return false;
    }
}