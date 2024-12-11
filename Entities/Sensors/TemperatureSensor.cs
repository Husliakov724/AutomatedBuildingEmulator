namespace AutomatedBuilding.Entities.Sensors;

public class TemperatureSensor : Sensor
{
    public TemperatureSensor(string name, string description) : base(name, description)
    {
    }

    public override void ReadValue()
    {
        Console.WriteLine($"{Name}: {Value} C");
    }
}