
namespace NewStuffInDotNet;

public static class Utils
{
    public static FormattedNameResponse FormatName(string firstName, string lastName)
    {
        var fullName =  $"{lastName}, {firstName}";
        return new FormattedNameResponse(fullName, fullName.Length);

    }
}

public record FormattedNameResponse(string FormattedName, int LengthOfFormattedName);
//{
    //public string FormattedName { get; init; } = string.Empty;
    //public int LengthOfFormattedName { get; init; }
//}