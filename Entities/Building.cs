using AutomatedBuilding.Constants;
using AutomatedBuilding.Entities.Sensors;
using AutomatedBuilding.Entities.Systems;

namespace AutomatedBuilding.Entities;

public class Building
{
    public List<Sensor> Sensors { get; set; }
    public HumidifierSystem Humidifier { get; set; }
    public LightingSystem Lighting { get; set; }
    public RadiatorSystem Radiator { get; set; }
    public VentilationSystem Ventilation { get; set; }
    
    private BuildingValueSimulator buildingValuesSimulator = new BuildingValueSimulator();

    public Building(List<Sensor> sensors, HumidifierSystem humidifierSystem, LightingSystem lightingSystem,
        RadiatorSystem radiatorSystem, VentilationSystem ventilationSystem)
    {
        Sensors = sensors;
        Humidifier = humidifierSystem;
        Lighting = lightingSystem;
        Radiator = radiatorSystem;
        Ventilation = ventilationSystem;
    }

    public void Monitor()
    {
        buildingValuesSimulator.SimulateValues(Sensors);
        foreach (var sensor in Sensors)
        {
            sensor.ReadValue();
        }
    }

    public void Control()
    {
        foreach (var sensor in Sensors)
        {
            double sensorValue = sensor.Value;
            if (sensor is ClockSensor)
            {
                if (sensorValue >= SystemConstants.startDayTime && sensorValue <= SystemConstants.endDayTime)
                {
                    Lighting.TurnOff();
                }
                else
                {
                    Lighting.TurnOn();
                }
            }

            if (sensor is TemperatureSensor)
            {
                if (sensorValue > SystemConstants.maxTemperature)
                {
                    Radiator.TurnOff();
                    Ventilation.TurnOn();
                }
                else if (sensorValue < SystemConstants.minTemperature)
                {
                    Radiator.TurnOn();
                    Ventilation.TurnOff();
                }
                else
                {
                    Ventilation.TurnOff();
                    Radiator.TurnOff();
                }
            }

            if (sensor is HumiditySensor)
            {
                if (sensorValue < SystemConstants.minHumidity)
                    Humidifier.TurnOn();
                else
                {
                    Humidifier.TurnOff();
                }
            }
        }
    }
}