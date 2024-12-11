using AutomatedBuilding.Entities.Interfaces;

namespace AutomatedBuilding.Entities.Systems;

public class RadiatorSystem : IControllable
{
    public bool IsOn { get; private set; }

    public void TurnOn()
    {
        IsOn = true;
        Console.WriteLine("Radiator system turned on.");
    }
    
    public void TurnOff()
    {
        IsOn = false;
        Console.WriteLine("Radiator system turned off.");
    }
}