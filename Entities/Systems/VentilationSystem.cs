using AutomatedBuilding.Entities.Interfaces;

namespace AutomatedBuilding.Entities.Systems;

public class VentilationSystem : IControllable
{
    public bool IsOn { get; private set; }

    public void TurnOn()
    {
        IsOn = true;
        Console.WriteLine("Ventilation system turned on.");
    }
    
    public void TurnOff()
    {
        IsOn = false;
        Console.WriteLine("Ventilation system turned off.");
    }
}