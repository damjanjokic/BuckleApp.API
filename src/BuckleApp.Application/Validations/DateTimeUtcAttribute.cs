using System.ComponentModel.DataAnnotations;

namespace BuckleApp.Application.Validations;

public class DateTimeUtcAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dateTime = (DateTime)value;

        return 
            dateTime.Kind != DateTimeKind.Utc ? new ValidationResult("Date has to be UTC") 
                : ValidationResult.Success;
    }
}