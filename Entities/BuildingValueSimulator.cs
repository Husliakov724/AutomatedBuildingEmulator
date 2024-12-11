using AutomatedBuilding.Constants;
using AutomatedBuilding.Entities.Sensors;

namespace AutomatedBuilding;

public class BuildingValueSimulator
{
    public void SimulateValues(List<Sensor> sensors)
    {
        foreach (var sensor in sensors)
        {
            if (sensor is TemperatureSensor)
            {
                sensor.Value = Math.Round(new Random().NextDouble() * 40, SystemConstants.roundValue);
            }
            else if (sensor is HumiditySensor)
            {
                sensor.Value = Math.Round(new Random().NextDouble() * 100, SystemConstants.roundValue);
            }
            else if (sensor is ClockSensor)
            {
                sensor.Value = Math.Round(new Random().NextDouble() * 24, SystemConstants.roundValue);
            }
        }
    }
}