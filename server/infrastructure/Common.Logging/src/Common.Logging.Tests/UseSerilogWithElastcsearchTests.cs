using Common.Logging.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Logging.Test;

public class UseSerilogWithElastcsearchTests
{
    [Fact]
    public void UseSerilogWithElasticsearch_WhenCalled_ConfiguresLoggerCorrectly()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
    {
        {"ElasticsearchUrl", "http://localhost:9200"},
        {"SeqUrl", "http://localhost:5341"}
    };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var hostBuilder = Host.CreateDefaultBuilder();

        // Act
        hostBuilder.UseSerilogWithElasticsearch(configuration);
        var host = hostBuilder.Build();

        // Assert
        var logger = host.Services.GetService<ILogger<UseSerilogWithElastcsearchTests>>();
        if (logger is null)
        {
            throw new ArgumentNullException("Logger is null");
        }
        logger.LogInformation("This is a test log message");
        Assert.NotNull(logger);
    }
    [Fact]
    public void UseSerilogWithElasticsearch_WhenElasticsearchUrlNotConfigured_ThrowsArgumentNullException()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
    {
        {"SeqUrl", "http://localhost:5341"}
    };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var hostBuilder = Host.CreateDefaultBuilder();

        Exception ex = Record.Exception(() => hostBuilder.UseSerilogWithElasticsearch(configuration));

        // Assert
        Assert.NotNull(ex);
        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal("Value cannot be null. (Parameter 'ElasticSearchUrl or SeqUrl is not configured in appsettings.json')", ex.Message);
    }
}

