﻿namespace AutomatedBuilding.Entities;

public class IndicatorModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Value { get; set; }
    public string Unit { get; set; }
    public List<string> IndicatorValues { get; set; }
}