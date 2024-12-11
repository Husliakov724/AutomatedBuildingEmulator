using System.Collections.Concurrent;
using System.Text.Json;
using AutomatedBuilding.Constants;
using AutomatedBuilding.Entities;
using AutomatedBuilding.Entities.Sensors;
using AutomatedBuilding.Entities.Systems;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutomatedBuilding;

class Program
{
    private static readonly ConcurrentDictionary<string, Task> tasks = new();

    private static readonly ConcurrentDictionary<string, CancellationTokenSource> tokens = new();

    private static readonly ConcurrentDictionary<string, double> values = new();

    private static Building building = CreateBuilding();
    private static HubConnection connection;

    static async Task Main(string[] args)
    {
        // Create a service collection
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        // Build the service provider
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Get the logger and use it
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Application started.");
        
        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7177/indicator")
            .Build();
        
        await connection.StartAsync();
        
        HttpClient client = new HttpClient();
        
        client.BaseAddress = new Uri("https://localhost:7177/api/");
        var result = await client.GetAsync("indicator");
        var content = await result.Content.ReadAsStringAsync();
        
        var deserializedResult = JsonSerializer.Deserialize<List<IndicatorModel>>(content, new
            JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var sensorFactories = new Dictionary<string, Func<Sensor>>
        {
            {"InnerTemperature",() => new TemperatureSensor("Temperature", "Temperature Sensor") },
            {"InnerHumidity",() => new HumiditySensor("Humidity", "Humidity Sensor") },
            {"Clock",() => new HumiditySensor("Clock", "Clock") },
        };
        
        foreach(var indicator in deserializedResult)
        {
            if(sensorFactories.TryGetValue(indicator.Name, out var createSensor))
            {
                var sensor = createSensor();
                sensor.Name = indicator.Name;
                sensor.Description = indicator.Description;
                building.Sensors.Add(sensor);
            }
            else 
            {
                logger.LogWarning($"Unknown indicator: {indicator.Name}");
            }
        }
        
        foreach(var model in deserializedResult)
        {
            AddDataProcessTask(model.Id,
                model.Value,
                model.IndicatorValues.LastOrDefault() ?? "0",
                model);
        }
        
        connection.On("UpdateTargetValue", (string id, string value) =>
        {
            tokens.TryGetValue(id, out CancellationTokenSource? token);
            if (tokens == null)
            {
                logger.LogWarning($"No token found for ID: {id}");
                return;
            }

            token.Cancel();
            logger.LogInformation($"CancellingTask with ID: {id} and adding new task");
            AddDataProcessTask(Guid.Parse(id), value, "0", new IndicatorModel());
        });

        connection.Closed += async (error) =>
        {
            logger.LogWarning("Connection closed. Trying to reconnect ...");
            await Task.Delay(new Random().Next(10, 11) * 1000);
            await connection.StartAsync();
        };

        Console.ReadLine();
    }
    
    private static void AddDataProcessTask(Guid id, string value, string lastValue, IndicatorModel indicatorModel)
    {
        var source = new CancellationTokenSource();

        var task = CreateDataProcessingTask(
            id,
            double.Parse(value),
            double.Parse(lastValue),
            source.Token,
            indicatorModel);

        tasks.TryAdd(id.ToString(), task);
        tokens.TryAdd(id.ToString(), source);
    }
    
    private static async Task CreateDataProcessingTask(Guid id, double baseValue, double lastValue,
        CancellationToken token, IndicatorModel indicatorModel)
    {
        while(!token.IsCancellationRequested)
        {
            double value = lastValue;
            values.AddOrUpdate(id.ToString(), lastValue, (name, currentValue) =>
            {
                value = GenerateValue(baseValue, currentValue, indicatorModel);
                return value;
            });

            await connection.InvokeAsync("SendValue", id.ToString(), value.ToString(), token);
            await Task.Delay(3000, token);
        }

        tasks.Remove(id.ToString(), out _);
    }
    
    private static double GenerateValue(double targetValue, double currentValue, IndicatorModel indicatorModel)
    {
        building.Monitor();

        return building.Sensors.FirstOrDefault(x => x.Name == indicatorModel.Name)!.Value;
    }

    private static Building CreateBuilding()
    {
        List<Sensor> sensors = new List<Sensor>();
    
        return new Building(sensors, new HumidifierSystem(), new LightingSystem(), new RadiatorSystem(),
            new VentilationSystem());
    }
    
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.AddDebug();
            })
            .Configure<LoggerFilterOptions>(options =>
            {
                options.MinLevel = LogLevel.Information;
            });
    }
}