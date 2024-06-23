namespace Eggs.Api.Grains.User;

/// <summary>
/// Grain key is user email. Should consider shifting to ADO.NET in the future.
/// </summary>
public interface IUserGrain : IGrainWithStringKey
{

}