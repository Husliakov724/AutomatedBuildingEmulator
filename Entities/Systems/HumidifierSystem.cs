using AutomatedBuilding.Entities.Interfaces;

namespace AutomatedBuilding.Entities.Systems;

public class HumidifierSystem : IControllable
{
    public bool IsOn { get; private set; }

    public void TurnOn()
    {
        IsOn = true;
        Console.WriteLine("Humidifier system turned on.");
    }
    
    public void TurnOff()
    {
        IsOn = false;
        Console.WriteLine("Humidifier system turned off.");
    }
}