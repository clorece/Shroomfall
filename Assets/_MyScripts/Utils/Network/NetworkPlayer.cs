using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigidbody3D;

    [SerializeField]
    ConfigurableJoint mainJoint;

    //input
    Vector2 moveInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;
    float maxSpeed = 3;
    bool isGrounded = false;
    RaycastHit[] raycastHits = new RaycastHit[10];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpButtonPressed = true;
        }
    }

    void FixedUpdate()
    {
        //Assume that we are not grounded
        isGrounded = false;

        //Check if we are grounded
        int numberOfHits = Physics.SphereCastNonAlloc(rigidbody3D.position, 0.1f, Vector3.down, raycastHits, 0.5f);

        //Check for valid results
        for (int i = 0; i < numberOfHits; i++)
        {
            //Ignore self-collision
            if (raycastHits[i].transform.root == transform)
                continue;

            isGrounded = true;
            break;
        }

        if (!isGrounded)
            rigidbody3D.AddForce(Vector3.down * 10);

        float inputMagnitude = moveInputVector.magnitude;

        if (inputMagnitude != 0)
        {
            //Forces character to face the direction of movement
            Quaternion desiredDirection = Quaternion.LookRotation(new Vector3(moveInputVector.x, 0, moveInputVector.y * -1), transform.up);

            //Rotate target towards direction
            mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, desiredDirection, 300 * Time.fixedDeltaTime);

            Vector3 localVelocityVsForward = transform.forward * Vector3.Dot(transform.forward, rigidbody3D.linearVelocity);

            float localForwardVelocity = localVelocityVsForward.magnitude;

            if (localForwardVelocity < maxSpeed)
            {
                //Move the character in the direction it is facing
                rigidbody3D.AddForce(transform.forward * inputMagnitude * 30);
            }
        }
        
        if(isGrounded && isJumpButtonPressed)
        {
            rigidbody3D.AddForce(Vector3.up * 20, ForceMode.Impulse);
            isJumpButtonPressed = false;
        }
    }
    
}
