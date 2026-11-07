using System.ComponentModel.DataAnnotations;

namespace BuckleApp.Application.Validations;

public class StartTimeBeforeEndTimeAttribute : ValidationAttribute
{
    public string StartTimeProperty { get; }

    public StartTimeBeforeEndTimeAttribute(string startTimeProperty)
    {
        StartTimeProperty = startTimeProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var endTime = (DateTime)value;

        var startTimeProperty = validationContext.ObjectType.GetProperty(StartTimeProperty);
        if (startTimeProperty == null)
            return new ValidationResult($"Unknown property: {StartTimeProperty}");

        var startTime = (DateTime)startTimeProperty.GetValue(validationContext.ObjectInstance);

        if (startTime >= endTime)
            return new ValidationResult("StartTime must be less than EndTime.");

        return ValidationResult.Success;
    }
}