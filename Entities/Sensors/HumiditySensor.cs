namespace AutomatedBuilding.Entities.Sensors;

public class HumiditySensor : Sensor
{
    public HumiditySensor(string name, string description) : base(name, description)
    {
    }

    public override void ReadValue()
    {
        Console.WriteLine($"{Name}: {Value} %");
    }
}