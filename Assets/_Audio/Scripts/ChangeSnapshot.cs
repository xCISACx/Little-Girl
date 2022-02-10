using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSnapshot : MonoBehaviour
{
    //The target snapshot's path
    public string SnapshotPath;

    //The target floor type
    public string FloorType = "";

    private FMODLittleGirl _fmodManager;

    private void Awake()
    {
        _fmodManager = FindObjectOfType<FMODLittleGirl>();
    }

    private void OnEnable()
    {
        //Change the snapshot
        _fmodManager.ChangeSnapshot(SnapshotPath);

        //If there is a specific floor type
        if(FloorType != "")
        {
            //Change the floor type (for the characters' step sounds)
            _fmodManager.ChangeFloorType(FloorType);
        }
    }
}
