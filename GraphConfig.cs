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

    /// <summary>
    /// Adds a variable to the config. 
    /// </summary>
    /// <param name="varName">variable that will be replaced</param>
    /// <param name="value">the value that will replace the variable</param>
    /// <param name="shouldOverwrite">Whether the variable should be overwritten if it already exists. Defaults to false</param>
    /// <returns>False if the variable already exists. True if the variable was created or overwritten</returns>
    public bool AddVariable(String varName, String value, bool shouldOverwrite = false)
    {
        try
        {
            findAndReplace.Add(varName, value);
        }
        catch (Exception e)
        {
            if (e is ArgumentException && shouldOverwrite)
            {
                findAndReplace[varName] = value;
                return true;
            }
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Removes a variable from the config
    /// </summary>
    /// <param name="varName">variable to be removed</param>
    /// <returns>False if variable was not found. True if the variable was removed</returns>
    public bool RemoveVariable(String varName)
    {
        try
        {
            findAndReplace.Remove(varName);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }
    
    
    /// <summary>
    /// Replaces all the variables in the node with the values in the config
    /// </summary>
    /// <param name="node">Node that the variables will be replaced in</param>
    public void ReplaceVariables(Node node)
    {
        foreach (var kvp in findAndReplace)
        {
            node.value = node.value.Replace(kvp.Key, kvp.Value);
        }
    }
}