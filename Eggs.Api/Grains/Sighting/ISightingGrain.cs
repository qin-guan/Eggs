namespace Eggs.Api.Grains.Sighting;

public interface ISightingGrain : IGrainWithGuidKey
{
    public Task CreateAsync(SightingState initialState);
    public Task<SightingState> GetAsync();
}