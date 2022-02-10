using UnityEngine;

public enum HandStates
{
    Free,
    Holding,
    PickingUp,
    PuttingDown
}

public class CloseupCameraSelectObject : MonoBehaviour
{
    CloseupCameraHandHeightCalculator _heightCalculator;

    public HandStates HandState;

    public GameObject Holding;

    private void Start()
    {
        //Instance creation
        _heightCalculator = GetComponent<CloseupCameraHandHeightCalculator>();
        HandState = HandStates.Free;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(HandState)
        {
            case HandStates.Free:
                if(Input.GetMouseButtonDown(0))
                {
                    PickUpObject();
                }
                break;
            case HandStates.PickingUp:
                _heightCalculator.CorrectHeight(Holding.transform.position);
                break;
            case HandStates.Holding:
                if (Input.GetMouseButtonDown(0))
                {
                    PutDownObject();
                }
                break;
            case HandStates.PuttingDown:
                break;
            default:
                break;
        }
    }

    void PickUpObject()
    {
        if (_heightCalculator.Below != null)
        {
            if (_heightCalculator.Below.CompareTag("ScaleObject"))
            {
                HandState = HandStates.PickingUp;
                Holding = _heightCalculator.Below;
                Holding.GetComponent<SphereCollider>().enabled = true;
                _heightCalculator.ScaleObjects.Remove(Holding);
            }
        }
    }

    void PutDownObject()
    {
        if (Holding != null)
        {
            HandState = HandStates.PuttingDown;
            Holding.GetComponent<SphereCollider>().enabled = false;
            Holding.GetComponent<Rigidbody>().useGravity = true;
            Holding.GetComponent<Rigidbody>().isKinematic = false;
            Holding.GetComponent<ObjectHandPosition>().enabled = false;
            _heightCalculator.ScaleObjects.Add(Holding);
            Holding = null;
            HandState = HandStates.Free;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ScaleObject"))
        {
            Debug.Log("Caught object");
            other.GetComponent<Rigidbody>().useGravity = false;
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.GetComponent<ObjectHandPosition>().enabled = true;
            HandState = HandStates.Holding;
        }
    }
}
