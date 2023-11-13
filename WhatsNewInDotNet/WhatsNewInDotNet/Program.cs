
using WhatsNewInDotNet;


Console.WriteLine("Hello, World!");

var bob = new Customer(18, "Bob Smith")
{
    EmailAddress = "bob@aol.com"
};

var bob2 = new Customer(18, "Bob Smith")
{
    EmailAddress = "bob@aol.com"
};

if (bob == bob2)
{
    Console.WriteLine("They are the same!");
}
else
{
    Console.WriteLine("They are not the same");
}



Console.WriteLine(bob.Name.ToUpper());

var p = bob.GetLastPayment();

Console.WriteLine($"Bob made a payment of {p.Amount:c} on {p.Date:m}");




