using System.Security.Policy;

namespace DialogueSystem;

internal static class AppDomainHandler
{
    public static AppDomain FRDomain;

    static AppDomainHandler()
    {
	    if (FRDomain == null)
	    {
		    FRDomain = CreateLSPDFRAppDomain();
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
