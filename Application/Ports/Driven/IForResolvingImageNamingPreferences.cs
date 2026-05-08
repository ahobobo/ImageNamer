using Application.Models;

namespace Application.Ports.Driven;

public interface IForResolvingImageNamingPreferences
{
    ImageNamingPreferences Resolve(ProjectLocalConfig? config, RunOverrides? overrides = null);
}
