using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReadableMenuItemBehaviour : MonoBehaviour
{
    public string ReadableID;
    public TextMeshProUGUI NameText;
    public ReadableObjectBehaviour ReadableObject;

    // Start is called before the first frame update
    void Awake()
    {
        NameText = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void Read()
    {
        ReadableObject.GetComponentInChildren<ReadableObjectBehaviour>().Read();
    }

    public void RegisterObject(ReadableObjectBehaviour obj)
    {
        ReadableID = obj.transform.parent.GetComponent<ReadableProperties>().Codename;
        ReadableObject = obj;
        NameText.text = obj.Book.name;
    }
}
