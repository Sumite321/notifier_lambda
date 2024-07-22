namespace Notifier.Lambda.Application.Options.Provider;

public class VonageProviderOptions
{
    public bool Enabled { get; set; }
    public int Priority { get; set; }
    public string? Name { get; set; }
}