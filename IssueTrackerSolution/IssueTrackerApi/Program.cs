using IssueTrackerApi;
using IssueTrackerApi.Services;
using Marten;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dataConnectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("Need A Connection String");
builder.Services.AddMarten(options =>
{
    options.Connection(dataConnectionString);
    options.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.All; // Classroom-ish
});

var businessClockAddress = builder.Configuration.GetValue<string>("business-clock-api") ?? throw new Exception("Need an address for the business clock");
builder.Services.AddHttpClient<BusinessClockApiAdapter>(client =>
{
    client.BaseAddress = new Uri(businessClockAddress);

}).AddPolicyHandler(SrePolicies.GetDefaultRetryPolicy())
  .AddPolicyHandler(SrePolicies.GetDefaultCircuitBreaker());

// Lazy - create the SystemTime only in response to the first request that needs it, then keep it around.
builder.Services.AddSingleton<ISystemTime, SystemTime>();

//var systemTime = new SystemTime(); // Eager!
//builder.Services.AddSingleton<ISystemTime>(systemTime);


// Lazy Factory
//builder.Services.AddSingleton<ISystemTime>(sp =>
//{
//    return new SystemTime(sp.GetRequiredService<ILogger>());
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();



app.MapControllers(); // go find all the controllers, read their attributes [HttpGEt, HttpPost, etc.] 
// and make a "phone directory"
// POST /issues

app.Run(); // The api isn't running until we ge here.
