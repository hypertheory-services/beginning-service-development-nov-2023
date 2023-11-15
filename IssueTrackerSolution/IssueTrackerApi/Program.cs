using IssueTrackerApi.Services;
using Marten;

var builder = WebApplication.CreateBuilder(args); // Kestrel Web Server

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("issues") ?? throw new Exception("Need a connection string");
builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);
}).UseLightweightSessions();
builder.Services.AddScoped<IssuesCatalog>();

var apiUrl = builder.Configuration.GetConnectionString("api") ?? throw new Exception("Need an API Url");
builder.Services.AddHttpClient<ClockApiAdapter>(client =>
{
    client.BaseAddress = new Uri(apiUrl);

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers(); // Use reflection to look for all the controller classes and read the attributes to create the route table.




app.Run();
