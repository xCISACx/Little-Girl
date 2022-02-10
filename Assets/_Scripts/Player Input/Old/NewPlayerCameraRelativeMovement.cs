using UnityEngine;

public class NewPlayerCameraRelativeMovement : MonoBehaviour
{
    private Vector3 _movementDirection;

    private PlayerGlobalVariables _playerGlobalVariables;

    public Rigidbody rb;
    public Vector3 playerDirection;
    public float speed;
    
    public float horizontalAxis;
    public float verticalAxis;
    
    public Vector3
        NormalizedForward,
        NormalizedRight,
        TargetNormalizedForward,
        TargetNormalizedRight;

    // Start is called before the first frame update
    void Start()
    {
        _playerGlobalVariables = GetComponent<PlayerGlobalVariables>();
        rb = GetComponent<Rigidbody>();
        
        NormalizedForward = _playerGlobalVariables.CurrentCamera.transform.forward;
        NormalizedRight = _playerGlobalVariables.CurrentCamera.transform.right;
    }

    void Update()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        
        NormalizedForward.y = 0f;
        NormalizedRight.y = 0f;
        NormalizedForward.Normalize();
        NormalizedForward.Normalize();
        
        if (_playerGlobalVariables.CanMove)
        {
            playerDirection = GetKeyboardInput();

            //TODO: DETECT IF USING CONTROLLER AND GET INPUT
            //playerDirection = GetInput();

            var camera = _playerGlobalVariables.CurrentCamera.transform.GetChild(0);

            Vector3 dir = camera.TransformDirection(playerDirection);
            dir.Set(dir.x, 0, dir.z);

            playerDirection = dir.normalized * playerDirection.magnitude;

            rb.velocity = new Vector3(playerDirection.x * speed, rb.velocity.y, playerDirection.z * speed);

            transform.LookAt(transform.position + new Vector3(playerDirection.x, 0, playerDirection.z));

            if(playerDirection.y == 0)
            {
                if(playerDirection.x == 0)
                {
                    NormalizedForward = TargetNormalizedForward;
                    NormalizedRight = TargetNormalizedRight;
                }
            }
        }
    }
    
    private void Move()
    {
        rb.velocity = new Vector3(playerDirection.x * speed,
            rb.velocity.y,
            playerDirection.z * speed);
    }

    private Vector3 GetInput()
    {
        Vector3 dir = Vector3.zero;

        dir.x = Input.GetAxis("LHorizontal");
        dir.y = 0;
        dir.z = Input.GetAxis("LVertical");

        if (dir.magnitude > 1)
        {
            dir.Normalize();
        }

        return dir;
    }
	
    private Vector3 GetKeyboardInput()
    {
        Vector3 dir = Vector3.zero;

        dir.x = Input.GetAxis("Horizontal");
        dir.y = 0;
        dir.z = Input.GetAxis("Vertical");

        if (dir.magnitude > 1)
        {
            dir.Normalize();
        }

        return dir;
    }
    
    private Vector3 RotateWithView()
    {
        Vector3 dir = _playerGlobalVariables.CurrentCamera.transform.GetChild(0).TransformDirection(playerDirection);
        dir.Set(dir.x, 0, dir.z);
        return dir.normalized * playerDirection.magnitude;
    }
}
