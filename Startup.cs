using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BooksApi.Models;
using BooksApi.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Google.Cloud.SecretManager.V1;


namespace BooksApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
	   services.Configure<BookstoreDatabaseSettings>(
           Configuration.GetSection(nameof(BookstoreDatabaseSettings)));

           services.AddSingleton<IBookstoreDatabaseSettings>(sp =>
           sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);

           services.AddSingleton<BookService>();

           services.AddDbContext<PostgreSqlContext>(options => options.UseNpgsql(SocketConnection()));

           services.AddScoped<IAuthorService, AuthorService>();
           
	   services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

	string SocketConnection()
        {
            // [START cloud_sql_postgres_dotnet_ado_connection_socket]
            // Equivalent connection string: 
            // "Server=<dbSocketDir>/<INSTANCE_CONNECTION_NAME>;Uid=<DB_USER>;Pwd=<DB_PASS>;Database=<DB_NAME>"
            String dbSocketDir = Environment.GetEnvironmentVariable("DB_SOCKET_PATH") ?? "/cloudsql";
            String instanceConnectionName = Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME");
	    Console.Out.WriteLine("ICN:"+instanceConnectionName);
            var connectionString = new NpgsqlConnectionStringBuilder()
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance. 
                SslMode = SslMode.Disable,
                // Remember - storing secrets in plaintext is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                Host = String.Format("{0}/{1}", dbSocketDir, instanceConnectionName),
                Username = Environment.GetEnvironmentVariable("DB_USER"), // e.g. 'my-db-user
                Password = GetDBPassword(),
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
                
            };
            connectionString.Pooling = true;
	    Console.Out.WriteLine("CS:"+connectionString.ConnectionString);
            return connectionString.ConnectionString;
        }

	string GetDBPassword()
	{
	    // Create the client.
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName(
			Environment.GetEnvironmentVariable("PROJECT"), 
			Environment.GetEnvironmentVariable("SECRET_ID"), 
			Environment.GetEnvironmentVariable("SECRET_VER"));

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            return result.Payload.Data.ToStringUtf8();
	}

	
    }
}
