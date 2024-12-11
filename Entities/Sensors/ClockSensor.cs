namespace AutomatedBuilding.Entities.Sensors;

public class ClockSensor : Sensor
{
    public ClockSensor(string name, string description) : base(name, description)
    {
    }

    public override void ReadValue()
    {
        Console.WriteLine($"{Name} is: {Value} o'clock");
    }
}