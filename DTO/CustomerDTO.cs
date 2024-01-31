namespace DTO;
public class CustomerDTO
{
    public int CustomerID { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public int? PostCode { get; set; }
    public List<AccountDTO> Accounts { get; set; }
    public LoginDTO Login { get; set; }

}
