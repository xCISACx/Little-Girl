using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalScratchBehaviour : MonoBehaviour
{
    //The velocity modifier for this vertical scratch
    private float VelocityModifier = 2f;

    //The scratch's colour
    public Color ScratchColour = Color.white;

    //The transparency range
    public float
        MinimumOpacity = 0.05f,
        MaximumOpacity = 0.15f;

    // Start is called before the first frame update
    void Awake()
    {
        //Randomize the scratch's transparency
        GetComponent<Image>().color = new Color(ScratchColour.r, ScratchColour.g, ScratchColour.b, Random.Range(MinimumOpacity, MaximumOpacity));

        //Randomize the scratch's horizontal scale
        transform.localScale = new Vector2(Random.Range(transform.localScale.x, transform.localScale.x + 2), transform.localScale.y);

        //Randomize the scratch's vertical velocity
        VelocityModifier = Random.Range(2f, 4f);
    }

    private void Update()
    {
        transform.localPosition -= new Vector3(0, Screen.height / 5f, 0) * VelocityModifier;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(transform.localPosition.y < -Screen.height * 2f)
        {
            Destroy(gameObject);
        }
    }
}
