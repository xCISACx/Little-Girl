using UnityEngine;

public class PlayerCameraRelativeMovement : MonoBehaviour
{
    public Vector3
        //The vectors used for movement
        NormalizedForward,
        NormalizedRight,
        //The target vectors which will come into use as soon as the player releases all movement keys
        TargetNormalizedForward,
        TargetNormalizedRight;

    //The rotated direction
    private Vector3 _movementDirection;

    //The player's global variables script
    private PlayerGlobalVariables _playerGlobalVariables;

    //The player's model rotation script
    [SerializeField]
    private PlayerModelRotation _playerModelRotation;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //Instance creation
        _playerGlobalVariables = GetComponent<PlayerGlobalVariables>();
        _playerModelRotation = transform.GetChild(1).GetComponent<PlayerModelRotation>();
        NormalizedForward = _playerGlobalVariables.CurrentCamera.transform.forward;
        NormalizedRight = _playerGlobalVariables.CurrentCamera.transform.right;
        animator = _playerGlobalVariables.gameObject.GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        //Read vertical and horizontal movement inputs
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");
        
        //Reset the rotated vectors' yy coordinates
        NormalizedForward.y = 0f;
        NormalizedRight.y = 0f;

        //Normalize the rotated vectors
        NormalizedForward.Normalize();
        NormalizedForward.Normalize();
        
        //Set the direction according to the normalized vectors
        _movementDirection = NormalizedForward * verticalAxis + NormalizedRight * horizontalAxis;

        //Normalize the movement vector
        _movementDirection.Normalize();

        //If the player can move
        if(_playerGlobalVariables.CanMove)
        {
            //Move the player
            //transform.Translate(_movementDirection * _playerGlobalVariables.movementSpeed * Time.deltaTime);
            _playerGlobalVariables._rigidbody.velocity = new Vector3(_movementDirection.x * _playerGlobalVariables.movementSpeed, 
                _playerGlobalVariables._rigidbody.velocity.y, _movementDirection.z * _playerGlobalVariables.movementSpeed);
            //If the player is moving
            if (_movementDirection.magnitude > 0.001f)
            {
                //Rotate the player model to face the rotated movement
                _playerModelRotation.RotateModel(_movementDirection);
            }
        }
        
        //If the rotated vectors are different from the target rotated vectors
        if(NormalizedForward != TargetNormalizedForward)
        {
            if(NormalizedRight != TargetNormalizedRight)
            {
                //Check if the player is still pressing a key
                if(verticalAxis == 0)
                {
                    if(horizontalAxis == 0)
                    {
                        //Update the rotated vectors
                        NormalizedForward = TargetNormalizedForward;
                        NormalizedRight = TargetNormalizedRight;
                    }
                }
            }
        }
    }

    void Update()
    {
        animator.SetFloat("PlayerInput", _movementDirection.sqrMagnitude);
    }
}
