
namespace NewStuffInDotNet.Hr;

public record class Employee
{
    
    public required int Id { get; init; }
    public required string? Name { get; init; }

    public string Address { get; set; } = string.Empty;

    public decimal Salary { get; private set; } = 5000M;



    public void GiveRaise(decimal amount)
    {
        Salary += amount;
    }
}

public record Retiree { }

