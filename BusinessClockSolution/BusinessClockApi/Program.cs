var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("issue-tracker/oncall-developer", () =>
{
    var response = new OnCallDeveloperResponse("Bob Smith", "bob@aol.com", "555-1212");
    return Results.Ok(response);
});

// another change

app.Run(); // starts a web server that listens on the network.

public partial class Program { }

public record OnCallDeveloperResponse(string Name, string EmailAddress, string PhoneNumber);