using Bogus;
using Eggs.Api.Grains;

namespace Eggs.Api.Extensions;

public static class TrafficCameraStateFakerExtensions
{
    internal static Faker<TrafficCameraState> GetBogusFaker(this TrafficCameraState _) =>
        new Faker<TrafficCameraState>()
            .StrictMode(true)
            .RuleFor(t => t.Id, (f, p) => f.Address.StreetAddress().ToLower().Replace(' ', '-'))
            .RuleFor(t => t.FriendlyName, (f, p) => f.Address.StreetName())
            .RuleFor(t => t.Location, (f, p) => f.Address.FullAddress())
            .RuleFor(t => t.Longitude, (f, p) => f.Address.Longitude())
            .RuleFor(t => t.Latitude, (f, p) => f.Address.Latitude())
            .RuleFor(t => t.FirstSeen, (f, p) => f.Date.PastOffset())
            .RuleFor(t => t.LastSeen, (f, p) => DateTimeOffset.Now);
}