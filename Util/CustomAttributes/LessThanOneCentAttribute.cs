using System.ComponentModel.DataAnnotations;

namespace Utilities.CustomAttributes;
public class LessThanOneCentAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        var amount = (decimal)value;
        if (amount.LessThanOneCent())
        {
            return false;
        }

        return true;
    }
}

