using Eggs.Api.Core;

namespace Eggs.Api.Options;

public class ChargeOptions
{
    public Dictionary<string, Dictionary<DayOfWeek, Dictionary<VehicleType, List<ChargeRule>>>> Rules { get; set; }

    public Dictionary<DayOfWeek, Dictionary<VehicleType, List<ChargeRule>>> Default => Rules["Default"];
}

public class ChargeRule
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public double Amount { get; set; }
};