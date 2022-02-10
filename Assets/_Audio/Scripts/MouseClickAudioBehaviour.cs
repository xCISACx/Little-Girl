using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickAudioBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "Globe")
                { print("My object is clicked by mouse"); }
            }


        }

    }
}