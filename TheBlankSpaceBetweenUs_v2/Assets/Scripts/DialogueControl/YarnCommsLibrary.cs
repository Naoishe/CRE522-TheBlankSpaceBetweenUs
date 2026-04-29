using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class YarnCommsLibrary : MonoBehaviour
{
    public InMemoryVariableStorage variableStorage;

    public void Awake()
    {
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
    }
    public void FetchYarnStringVariable(string yarnVar, string unityVar)
    {
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("String Fetched: " + unityVar);
    }
    public void FetchYarnBoolVariable(string yarnVar, bool unityVar)
    {
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("Bool Fetched: " + unityVar);
    }
    public void FetchYarnIntVariable(string yarnVar, int unityVar)
    {
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("Int Fetched: " + unityVar);
    }

    public void SetYarnStringVariable(string yarnVar, string updatedString)
    {
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        variableStorage.SetValue(yarnVar, updatedString);

    }
}
