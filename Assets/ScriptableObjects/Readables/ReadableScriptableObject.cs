using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Readable", menuName = "Readable")]
public class ReadableScriptableObject : ScriptableObject
{
    public string Name;
    public Image Readable;
    public Canvas ReadableCanvas;

    public void Use()
    {
        Readable.gameObject.SetActive(true);
        ReadableCanvas.gameObject.SetActive(true);
    }
}
