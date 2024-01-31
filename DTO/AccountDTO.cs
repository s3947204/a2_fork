namespace DTO;
public class AccountDTO
{
    public int AccountNumber { get; set; }
    public string AccountType { get; set; }
    public int CustomerID { get; set; }
    public decimal Balance { get; set; }
    public List<TransactionDTO> Transactions { get; set; }

}
