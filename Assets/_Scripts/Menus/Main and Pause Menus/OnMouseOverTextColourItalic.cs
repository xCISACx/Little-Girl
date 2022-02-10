using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseOverTextColourItalic : OnMouseOverTextColour, IPointerEnterHandler, IPointerExitHandler
{
    new public void OnPointerEnter(PointerEventData eventData)
    {
        //When the pointer is on top of this button, highlight the text and change its style to bold and italic
        _Text.color = _HightlightedColour;
        _Text.fontStyle = FontStyle.BoldAndItalic;
    }

    new public void OnPointerExit(PointerEventData eventData)
    {
        //When the pointer exits this button, set its properties back to default
        _Text.color = _NormalColour;
        _Text.fontStyle = FontStyle.Italic;
    }

    new protected void OnEnable()
    {
        //When this button is enabled, reset the text's properties
        _Text.color = _NormalColour;
        _Text.fontStyle = FontStyle.Italic;
    }
}
