using UnityEngine;
using Yarn.Unity;

public class YarnCommsLibrary : MonoBehaviour
{
    public VariableStorageBehaviour variableStorage;

    public void Awake()
    {
        variableStorage = FindObjectOfType<VariableStorageBehaviour>(true);
    }
    public void FetchYarnStringVariable(string yarnVar, string unityVar)
    {
        variableStorage = FindObjectOfType<VariableStorageBehaviour>(true);
        if (variableStorage == null) return;
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("String Fetched: " + unityVar);
    }
    public void FetchYarnBoolVariable(string yarnVar, bool unityVar)
    {
        variableStorage = FindObjectOfType<VariableStorageBehaviour>(true);
        if (variableStorage == null) return;
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("Bool Fetched: " + unityVar);
    }
    public void FetchYarnIntVariable(string yarnVar, int unityVar)
    {
        variableStorage = FindObjectOfType<VariableStorageBehaviour>(true);
        if (variableStorage == null) return;
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("Int Fetched: " + unityVar);
    }

    public void SetYarnStringVariable(string yarnVar, string updatedString)
    {
        variableStorage = FindObjectOfType<VariableStorageBehaviour>(true);
        if (variableStorage == null) return;
        variableStorage.SetValue(yarnVar, updatedString);

    }
}
