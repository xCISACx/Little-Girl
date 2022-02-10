using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnMouseOverTextColour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //This button's text
    [SerializeField]
    protected Text _Text;

    public Color
        //The normal text colour
        _NormalColour = new Color(0.75f, 0.75f, 0.75f),
        //The highlighted text colour
        _HightlightedColour = new Color(0.9f, 0.9f, 0.725f);
    
    void Awake()
    {
        //Instance creation
        _Text = transform.GetChild(0).GetComponent<Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //When the pointer is on top of this button, highlight the text and change its style to bold
        _Text.color = _HightlightedColour;
        _Text.fontStyle = FontStyle.Bold;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //When the pointer exits this button, set its properties back to default
        _Text.color = _NormalColour;
        _Text.fontStyle = FontStyle.Normal;
    }

    protected void OnEnable()
    {
        //When this button is enabled, reset the text's properties
        _Text.color = _NormalColour;
        _Text.fontStyle = FontStyle.Normal;
    }
}
