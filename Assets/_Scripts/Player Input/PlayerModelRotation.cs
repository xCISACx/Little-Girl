using UnityEngine;

public class PlayerModelRotation : MonoBehaviour
{
    public void RotateModel(Vector3 movementDirection)
    {
        //Rotate the player model to face the movement direction over time
        transform.rotation = Quaternion.Lerp(
                                              transform.rotation,
                                              Quaternion.LookRotation(movementDirection, Vector3.up),
                                              10f * Time.deltaTime
                                            );
    }
}
