using System.IO;
using System.Reflection;
using DialogueSystem.Engine;
using Rage;

namespace DialogueSystem.Core.Logic;

public class ExternalAction
{
    public string MethodString { get; set; }
    public MethodInfo MethodInfo { get; set; }

    public ExternalAction(string str)
    {
        MethodString = str;
        GetMethodInfo();
    }

    public void Invoke()
    {
        try
        {
            MethodInfo.Invoke(null, Array.Empty<object>());
        }
        catch (Exception ex)
        {
            Game.LogTrivial($"Failed to call {MethodString}: {ex.Message}");
        }
    }

    private void GetMethodInfo()
    {
        string[] array = MethodString.Split('.');
        if (array.Length < 2)
        {
            throw new ArgumentException(
                "Target format invalid. Expected 'Assembly.Namespace.Class.Method'.");
        }

        string assemblyName = array[0];
        string methodName = array[array.Length - 1];
        string className = string.Join(".", array.Skip(1).Take(array.Length - 2));

        Type type = null;

        Assembly[] assemblies = AppDomainHandler.FRDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            if (assembly.GetName().Name == assemblyName)
            {
                type = assembly.GetType(className);
                if (type != null)
                {
                    break;
                }
            }
        }

        if (type == null)
        {
            string assemblyPath =
                AppDomainHandler.FRDomain.BaseDirectory + "/" + assemblyName + ".dll";
            Game.LogTrivial($"loading assembly file {assemblyPath}");
            try
            {
                using FileStream fileStream = new FileStream(assemblyPath, FileMode.Open);
                byte[] assemblyBytes = new byte[(int)fileStream.Length];
                fileStream.Read(assemblyBytes, 0, assemblyBytes.Length);
                type = AppDomainHandler.FRDomain.Load(assemblyBytes).GetType(className);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Failed to load assembly '{assemblyName}' from '{assemblyPath}' or find type '{className}': {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "N/A"}",
                    ex);
            }
        }

        if (type == null)
        {
            throw new Exception(
                $"Could not find type '{className}' for assembly '{assemblyName}'. Ensure the assembly is loaded or accessible, and the type name (including namespace and casing) is correct.");
        }

        MethodInfo methodInfo = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase) &&
                                 m.GetParameters().Length == 0 && m.ReturnType == typeof(void));

        if (methodInfo == null)
        {
            throw new Exception(
                $"Cannot find an appropriate static public method '{methodName}' with NO parameters and return type 'bool' in type '{className}'.");
        }

        MethodInfo = methodInfo;
    }
}
