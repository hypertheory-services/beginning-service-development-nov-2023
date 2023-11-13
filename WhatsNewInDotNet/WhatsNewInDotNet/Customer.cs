
// File Scoped Namespace
namespace WhatsNewInDotNet;

public record Customer
{
    public Customer(int id, string name)
    {
        Id = id;
        Name = name;
    }


    // entities
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public required string EmailAddress { get; init; } = string.Empty;



    public Payment GetLastPayment()
    {
        // go to the database, whatever.
        return new Payment(532.52M, DateTime.Now.AddDays(-3));

    }

}


//public class Payment
//{
//    public Payment(decimal amount, DateTime date)
//    {
//        Amount = amount;
//        Date = date;
//    }
//    public decimal Amount { get; private set; }
//    public DateTime Date { get; private set; }
//}

public record Payment(decimal Amount, DateTime Date);