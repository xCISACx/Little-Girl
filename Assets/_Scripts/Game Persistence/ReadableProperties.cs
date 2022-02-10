using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using System.Runtime.InteropServices.WindowsRuntime;

[Serializable]
public class ReadableProperties : MonoBehaviour
{
    //The readable's name
    public string Name;

    //The readable's codename (for persistence purposes)
    public string Codename => gameObject.name;

    //The readable's image
    public Image Readable;

    //The readable canvas
    public Canvas ReadableCanvas;

    //The types of readables found in the game
    public enum ReadableType
    {
        Letter,
        Book,
        Newspaper
    };

    //This readable's type
    public ReadableType Type;
}
