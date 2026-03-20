using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLabelControls : MonoBehaviour
{
    private void Update()
    {
        if (ContinuousData.instance.currentSceneName == "CampusGrounds")
        {
            CampusGroundsControls();
        }
        if(ContinuousData.instance.currentSceneName == "PlayerHouse")
        {
            PlayerHouseControls();
        }
        if (ContinuousData.instance.currentSceneName == "Library")
        {
            LibraryControls();
        }
    }

    public void CampusGroundsControls()
    {
        
    }

    public void LibraryControls()
    {
        
    }

    public void PlayerHouseControls()
    {

    }
}
