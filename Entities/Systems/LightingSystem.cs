using AutomatedBuilding.Entities.Interfaces;

namespace AutomatedBuilding.Entities.Systems;

public class LightingSystem : IControllable
{
    public bool IsOn { get; private set; }

    public void TurnOn()
    {
        IsOn = true;
        Console.WriteLine("Lighting system turned on.");
    }
    
    public void TurnOff()
    {
        IsOn = false;
        Console.WriteLine("Lighting system turned off.");
    }
}