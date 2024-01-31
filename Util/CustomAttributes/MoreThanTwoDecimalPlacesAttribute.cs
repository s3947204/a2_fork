using System.ComponentModel.DataAnnotations;

namespace Utilities.CustomAttributes;
public class MoreThanTwoDecimalPlacesAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        var amount = (decimal)value;
        if (amount.HasMoreThanTwoDecimalPlaces())
        {
            return false;
        }

        return true;
    }
}

