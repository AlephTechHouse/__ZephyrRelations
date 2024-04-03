using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace Common.Logging.Serilog;

public static class Extensions
{
public static IHostBuilder UseSerilogWithElasticsearch(
    this IHostBuilder hostBuilder, 
    IConfiguration configuration)
    {
        var configErrorLogger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var elasticserchUrl = configuration["ElasticSearchUrl"];

        var seqUrl = configuration["SeqUrl"];

        if (string.IsNullOrWhiteSpace(elasticserchUrl) || string.IsNullOrWhiteSpace(seqUrl))
        {
            configErrorLogger.Error("ElasticSearchUrl or SeqUrl is not configured in appsettings.json");
            throw new ArgumentNullException("ElasticSearchUrl or SeqUrl is not configured in appsettings.json");
        }

        try{
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProcessId()
                    .Enrich.WithThreadId()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithEnvironmentName()

                    .WriteTo.Console(
                        theme: AnsiConsoleTheme.Code,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
                    )

                    .WriteTo.Debug(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
                    )

                    .WriteTo.Seq(seqUrl!)

                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticserchUrl!))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                        IndexFormat = $"{hostingContext.HostingEnvironment.ApplicationName.ToLower()}-{DateTime.UtcNow:yyyy.MM}",
                        CustomFormatter = new ElasticsearchJsonFormatter(renderMessage: true, inlineFields: true)
                    });
                });
        }   
        catch (Exception ex)
        {
            configErrorLogger.Error(ex, "Error configuring Serilog with ElasticSearch");
            throw;
        }

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        
        return hostBuilder;
    }
}
           
                   
       



