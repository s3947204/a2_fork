namespace Utilities;
public static class Util
{
    public static string ConvertAccountTypeForDisplay(string accountType)
    {
        return accountType == "C" ? "Checkings" : accountType == "S" ? "Savings" : "";
    }

    public static string ConvertTransactionTypeForDisplay(string transactionType)
    {
        if (transactionType == "T")
            return "Transfer";
        else if (transactionType == "W")
            return "Withdrawal";
        else if (transactionType == "D")
            return "Deposit";
        else if (transactionType == "B")
            return "Billpay";
        else if (transactionType == "S")
            return "Service";
        else
            return "";
    }

    public static string ConvertPeriodForDisplay(string period)
    {
        return period == "O" ? "One off" : period == "M" ? "Monthy" : "";
    }

    public static string ConvertStatusForDisplay(string status)
    {
        if (status == "P")
            return "Pending";
        else if (status == "F")
            return "Failed";
        else if (status == "C")
            return "Canceled";
        else
            return "";
    }

}


