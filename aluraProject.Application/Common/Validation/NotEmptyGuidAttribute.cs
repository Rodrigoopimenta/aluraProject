using System.ComponentModel.DataAnnotations;

namespace aluraProject.Application.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public NotEmptyGuidAttribute()
    {
        ErrorMessage = "The field must be a non-empty GUID.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is Guid guid)
        {
            return guid != Guid.Empty;
        }

        return false;
    }
}
