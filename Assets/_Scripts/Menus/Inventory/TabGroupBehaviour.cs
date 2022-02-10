using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupBehaviour : MonoBehaviour
{
    public ReadablePanelManager[] TabPanels;

    //The currently active tab
    private GameObject _activeTab;

    // Start is called before the first frame update
    void Start()
    {
        TabPanels = GetComponentsInChildren<ReadablePanelManager>(true);
    }

    public void HideAllTabs()
    {
        //Hide all tabs
        foreach (var panel in TabPanels)
        {
            panel.gameObject.SetActive(false);
        }
    }

    public GameObject GetActiveTab()
    {
        //Return the currently active tab
        return _activeTab;
    }

    public void SetActiveTab()
    {
        //Hide all tabs
        HideAllTabs();

        //Reset the active tab
        _activeTab = null;
    }

    public void SetActiveTab(GameObject newActiveTab)
    {
        //Set the active tab
        _activeTab = newActiveTab;

        //Activate the tab
        _activeTab.SetActive(true);
    }
}
