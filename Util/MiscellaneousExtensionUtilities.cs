namespace Utilities;

// Adapted from day 6 lab, project McbaExampleWithLogin
public static class MiscellaneousExtensionUtilities
{
    public static bool HasMoreThanNDecimalPlaces(this decimal value, int n) => decimal.Round(value, n) != value;
    public static bool HasMoreThanTwoDecimalPlaces(this decimal value) => value.HasMoreThanNDecimalPlaces(2);

    public static bool LessThanOneCent(this decimal value) => decimal.Compare(value, 0.01M) < 0;
}
