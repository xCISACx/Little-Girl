using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabBehaviour : MonoBehaviour
{
    public GameObject Panel;

    public List<ReadableProperties> List;

    public enum ListType
    {
        Letters,
        Books,
        Newspapers
    }

    public ListType Type;
    
    public GameObject ReadableMenuItemPrefab;

    public List<GameObject> InstantiatedContainers;

    public bool Toggled;

    // Start is called before the first frame update
    void Start()
    {
        if (!Panel)
        {
            Panel = GetComponentInChildren<GridLayoutGroup>(includeInactive: true).gameObject;  
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (Type)
        {
            case ListType.Letters:
                List = GameManager.instance.CollectedLetters;
                break;
            case ListType.Books:
                List = GameManager.instance.CollectedBooks;
                break;
            case ListType.Newspapers:
                List = GameManager.instance.CollectedNewspapers;
                break;
        }
    }

    public void ToggleTab()
    {
        GameObject activeTab = transform.parent.GetComponent<TabGroupBehaviour>().GetActiveTab();

        //If this tab isn't the active tab
        if(activeTab != Panel)
        {
            //Hide all tabs
            transform.parent.GetComponent<TabGroupBehaviour>().HideAllTabs();

            //Activate this tab
            transform.parent.GetComponent<TabGroupBehaviour>().SetActiveTab(Panel);
        }
        //If this is the active tab
        else
        {
            //Hide all tabs
            transform.parent.GetComponent<TabGroupBehaviour>().SetActiveTab();
        }

        /*if (!Toggled)
        {
            //Hide all tabs.
            foreach (var panel in transform.parent.GetComponent<TabGroupBehaviour>().TabPanels)
            {
                panel.gameObject.SetActive(false);
            }

            transform.parent.GetComponent<TabGroupBehaviour>().HideAllTabs();

            //Show the tab we want.
            Panel.SetActive(true);
            Toggled = true;

            
        }
        else if (Toggled)
        {
            //Hide the tab.
            Panel.SetActive(false);
            Toggled = false;
        }*/
    }
}
