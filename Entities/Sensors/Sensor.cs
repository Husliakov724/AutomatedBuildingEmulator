namespace AutomatedBuilding.Entities.Sensors;

public abstract class Sensor
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Value { get; set; }

    public Sensor(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public abstract void ReadValue();
}