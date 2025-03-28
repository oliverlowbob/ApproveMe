using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApproveMe.InternalEvents;
using ApproveMe.Models.Transactions;
using Marten;
using Marten.Events.Projections;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);
    
    // Get all internal event types in the assembly and add them to the event store
    var eventTypes = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => !t.IsAbstract && typeof(BaseEvent).IsAssignableFrom(t));

    foreach (var eventType in eventTypes)
    {
        options.Events.AddEventType(eventType);
    }
    
    options.UseSystemTextJsonForSerialization();

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
    
    options.Schema.For<Transaction>()
        .Identity(t => t.Id);
    
    options.Projections.Snapshot<Transaction>(SnapshotLifecycle.Inline);
})
    .UseLightweightSessions();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();