using BusinessClockApi;
using BusinessClockApi.Models;
using BusinessClockApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<BusinessClock>();
builder.Services.AddSingleton<ISystemTime, SystemTime>();
// Above this line is "configuring" our web application services.
var app = builder.Build();
// After this line is "middleware" - the stuff that receives the HTTP request and makes
// the response. 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/status", (BusinessClock clock) =>
{
    ClockResponseModel response;
    if (clock.IsOpen())
    {
        response = new ClockResponseModel
        {
            IsOpen = true,
            SupportContact = new SupportContactResponseModel
            {
                Name = "Susan",
                Phone = "800 555-1212",
                Email = "Susan@company.com"
            }
        };
    } else
    {
        response = new ClockResponseModel
        {
            IsOpen = false,
            SupportContact = new SupportContactResponseModel
            {
                Name = "Support Pros Inc.",
                Phone = "800 999-1213x23",
                Email = "support@support-pros.com"
            }
        };
    }
    return Results.Ok(response);
});


// this is what starts our web server. This is a blocking call. It will keep running forever.
app.Run();

// Because we aren't using a Static Void Main Method (we ARE using top-level statements)
public partial class Program { }