using System.ComponentModel.DataAnnotations;

namespace Utilities.CustomAttributes;
public class DateInFutureToCurrentAttribute : ValidationAttribute
{

    public override bool IsValid(object value)
    {
        var currentDateTime = DateTime.Now;
        var comp = DateTime.Compare((DateTime)value, currentDateTime);

        if (comp < 0)
            return false;

        return true;
    }
}

