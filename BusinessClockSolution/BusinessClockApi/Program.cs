using BusinessClockApi;
using BusinessClockApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<OnCallDeveloperLookup>();
builder.Services.AddScoped<IProvideTheBusinessClock, StandardBusinessClock>();
builder.Services.AddScoped<ISystemTime, SystemTime>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("issue-tracker/oncall-developer", (OnCallDeveloperLookup service) =>
{


    return Results.Ok(service.GetOnCallDeveloper());
});


// another change

app.Run(); // starts a web server that listens on the network.

public partial class Program { }

