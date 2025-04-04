using ApproveMe.InternalEvents;
using ApproveMe.Models.Transactions;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApproveMe.Repositories;
using Marten;
using Marten.Events.Projections;
using Weasel.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ApproveMe;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        Env = env;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Env { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("DefaultConnection");

        services.AddMarten(options =>
        {
            options.Connection(connectionString);
    
            // Discover and add all internal event types from the current assembly
            var eventTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(BaseEvent).IsAssignableFrom(t));

            foreach (var eventType in eventTypes)
            {
                options.Events.AddEventType(eventType);
            }
    
            options.UseNewtonsoftForSerialization();
            
            // In development, let Marten handle schema creation and patching automatically
            if (Env.IsDevelopment())
            {
                options.AutoCreateSchemaObjects = AutoCreate.All;
            }
    
            options.Schema.For<TransactionAggregate>()
                .Identity(t => t.Id);
    
            options.Projections.Snapshot<TransactionAggregate>(SnapshotLifecycle.Inline);
        })
        .UseLightweightSessions();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!))
                };
            });
        
        services.AddScoped<IRepository, Repository>();

        services.AddAuthorization();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Use developer exception page in development environments
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
