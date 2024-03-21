namespace DialogueSystem;

public class GraphConfig
{
    private Dictionary<String, String> findAndReplace;
    
    public GraphConfig()
    {
        findAndReplace = new Dictionary<string, string>();
    }
    
    public GraphConfig(Dictionary<String, String> findAndReplace)
    {
        this.findAndReplace = findAndReplace;
    }

    public bool AddVariable(String varName, String value)
    {
        try
        {
            findAndReplace.Add(varName, value);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }
    
    public bool RemoveVariable(String varName)
    {
        try
        {
            findAndReplace.Remove(varName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }
    
    public void ReplaceVariables(Node node)
    {
        foreach (var kvp in findAndReplace)
        {
            node.value = node.value.Replace(kvp.Key, kvp.Value);
        }
    }
}