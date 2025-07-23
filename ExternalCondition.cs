using System.IO;
using System.Reflection;
using Rage;

namespace DialogueSystem;

public class ExternalCondition
{
    public string MethodString { get; set; }
    public MethodInfo MethodInfo { get; set; }

    public ExternalCondition(string str)
    {
        MethodString = str;
        GetMethodInfo();
    }

    public bool Invoke()
    {
        try
        {
            return (bool) MethodInfo.Invoke(null, Array.Empty<object>());
        }
        catch (Exception ex)
        {
            Game.LogTrivial($"Failed to call {MethodString}: {ex.Message}");
        }
        return false;
    }

    private void GetMethodInfo()
    {
        string[] targetParts = MethodString.Split('.');
        if (targetParts.Length < 2)
        {
            throw new ArgumentException("Target format invalid. Expected 'Assembly.Namespace.Class.Method'.");
        }

        string assemblyName = targetParts[0];
        string className = string.Join(".", targetParts.Skip(1).Take(targetParts.Length - 2));
        string methodName = targetParts.Last();

        Type type = null;
        Assembly targetAssembly = null;

        targetAssembly = AppDomain.CurrentDomain.GetAssemblies()
                                    .FirstOrDefault(a => a.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));

        if (targetAssembly == null)
        {
            string assemblyPath = Path.Combine(AppDomainHandler.FRDomain.BaseDirectory, assemblyName + ".dll");
            Game.LogTrivial($"Loading assembly file {assemblyPath}");
            try
            {
                byte[] assemblyBytes = File.ReadAllBytes(assemblyPath);
                targetAssembly = AppDomainHandler.FRDomain.Load(assemblyBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load assembly '{assemblyName}' from '{assemblyPath}': {ex.Message}", ex);
            }
        }
        
        if (targetAssembly == null)
        {
             throw new Exception($"Could not find or load assembly '{assemblyName}'.");
        }

        type = targetAssembly.GetType(className);
        
        if (type == null)
        {
            throw new Exception($"Could not find type '{className}' in assembly '{assemblyName}'. Make sure the namespace is correct.");
        }

        MethodInfo methodInfo = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                                    .FirstOrDefault(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase) &&
                                                         m.GetParameters().Length == 0 && m.ReturnType == typeof(bool));

        if (methodInfo == null)
        {
            throw new Exception($"Cannot find an appropriate static public method '{methodName}' with NO parameters in type '{className}'.");
        }

        MethodInfo = methodInfo;
    }
}
