using System.Security.Policy;

namespace DialogueSystem.Engine;

internal static class AppDomainHandler
{
    internal static AppDomain FRDomain;
    static AppDomainHandler()
    {
	    if (FRDomain == null)
	    {
		    FRDomain = CreateLSPDFRAppDomain();
		    AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
	    }    
    }

    private static void OnDomainUnload(object sender, EventArgs e)
    {
	    if (FRDomain != null)
	    {
		    AppDomain.Unload(FRDomain);
	    }
    }
    private static AppDomain CreateLSPDFRAppDomain()
	{
		AppDomainSetup info = new AppDomainSetup
		{
			ApplicationBase = AppDomain.CurrentDomain.BaseDirectory + "/plugins/LSPDFR"
		};
		Evidence securityInfo = new Evidence(AppDomain.CurrentDomain.Evidence);
		return AppDomain.CreateDomain($"dialoguesystemfr{Guid.NewGuid()}", securityInfo, info);
	}
}
