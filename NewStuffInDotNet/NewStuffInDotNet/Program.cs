



using NewStuffInDotNet;
using NewStuffInDotNet.Hr;
using NewStuffInDotNet.Sales;
using System.Net.Sockets;

Console.WriteLine("Hello, World!");


var jill = new Employee()
{
    Id = 1,
   Name = "Jill Jones"
   
};

jill.Address = "8080 Bear Rd";

//var jill2 = new Employee()
//{
//    Id = jill.Id,
//    Name = jill.Name,
//    Address = "8080 Bluebird"
//};

var jill2 = jill with { Address = "8080 Bluebird" };

Console.WriteLine($"Jill and Jill2 are the same? {jill == jill2}");

var fullName = Utils.FormatName("Han", "Solo");
Console.WriteLine(fullName.FormattedName);
Console.WriteLine(new string('*', fullName.LengthOfFormattedName));

var fullName2 = Utils.FormatName("Luke", "Skywalker");
Console.WriteLine(fullName2.FormattedName);
Console.WriteLine(new string('*', fullName2.LengthOfFormattedName ));


//fullName2.FormattedName = "Organa, Leiah";


