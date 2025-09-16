using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Fusion.Addons.Physics;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    [SerializeField]
    Rigidbody rigidbody3D;

    [SerializeField]
    NetworkRigidbody3D networkRigidbody3D;

    [SerializeField]
    ConfigurableJoint mainJoint;

    [SerializeField]
    Animator animator;

    //Input
    Vector2 moveInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;

    //Controller settings
    float maxSpeed = 3;

    //States
    bool isGrounded = false;

    //Raycasts
    RaycastHit[] raycastHits = new RaycastHit[10];

    //Syncing of physics objects
    SyncPhysicsObject[] syncPhysicsObjects;

    //Cinemachine
    CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineBrain cinemachineBrain;

    //Syncing clients ragdolls, quanternion may not be the most efficient way but it works for now.
    [Networked, Capacity(10)] public NetworkArray<Quaternion> networkPhysicsSyncedRotations { get; }

    void Awake()
    {
        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Move input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            isJumpButtonPressed = true;
    }

    public override void FixedUpdateNetwork()
    {
        Vector3 localVelocifyVsForward = Vector3.zero;
        float localForwardVelocity = 0;

        if (Object.HasStateAuthority)
        {
            //Assume that we are not grounded. 
            isGrounded = false;

            //Check if we are grounded
            int numberOfHits = Physics.SphereCastNonAlloc(rigidbody3D.position, 0.1f, transform.up * -1, raycastHits, 0.5f);

            //Check for valid results
            for (int i = 0; i < numberOfHits; i++)
            {
                //Ignore self hits
                if (raycastHits[i].transform.root == transform)
                    continue;

                isGrounded = true;

                break;
            }

            //Apply extra gravity to charcter to make it less floaty
            if (!isGrounded)
                rigidbody3D.AddForce(Vector3.down * 10);

            localVelocifyVsForward = transform.forward * Vector3.Dot(transform.forward, rigidbody3D.linearVelocity);
            localForwardVelocity = localVelocifyVsForward.magnitude;
        }
        
        if(GetInput(out NetworkInputData networkInputData))
        {
            float inputMagnitued = networkInputData.movementInput.magnitude;

            if (inputMagnitued != 0)
            {
                Quaternion desiredDirection = Quaternion.LookRotation(new Vector3(networkInputData.movementInput.x, 0, networkInputData.movementInput.y * -1), transform.up);

                //Rotate target towards direction
                mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, desiredDirection, Runner.DeltaTime * 300);

                if (localForwardVelocity < maxSpeed)
                {
                    //Move the character in the direction it is facing
                    rigidbody3D.AddForce(transform.forward * inputMagnitued * 30);
                }
            }

            if (isGrounded && networkInputData.isJumpPressed)
            {
                rigidbody3D.AddForce(Vector3.up * 20, ForceMode.Impulse);

                isJumpButtonPressed = false;
            }
        }

       
        if(Object.HasStateAuthority)
        {
            animator.SetFloat("movementSpeed", localForwardVelocity * 0.4f);

            //Update the joints rotation based on the animations
            for (int i = 0; i < syncPhysicsObjects.Length; i++)
            {
                syncPhysicsObjects[i].UpdateJointFromAnimation();
                networkPhysicsSyncedRotations.Set(i, syncPhysicsObjects[i].transform.localRotation);
            }

            if (transform.position.y < -10)
                networkRigidbody3D.Teleport(Vector3.zero, Quaternion.identity);

        }
    }

    public override void Render()
    {
        if (!Object.HasStateAuthority)
        {
            var interpolated = new NetworkBehaviourBufferInterpolator(this);

            //Get the networked physics objects from the host and update the clients
            for (int i = 0; i < syncPhysicsObjects.Length; i++)
            {
                syncPhysicsObjects[i].transform.localRotation = Quaternion.Slerp(syncPhysicsObjects[i].transform.localRotation, networkPhysicsSyncedRotations.Get(i), interpolated.Alpha);
            }
        }

        if (Object.HasInputAuthority)
        {
            cinemachineBrain.ManualUpdate();
            cinemachineVirtualCamera.UpdateCameraState(Vector3.up, Runner.LocalAlpha);
        }
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //Move data
        networkInputData.movementInput = moveInputVector;

        if (isJumpButtonPressed)
            networkInputData.isJumpPressed = true;

        //Reset jump button
        isJumpButtonPressed = false;

        return networkInputData;
    }

    public override void Spawned()
    {
        if(Object.HasInputAuthority)
        {
            Local = this;

            cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineBrain = FindObjectOfType<CinemachineBrain>();

            cinemachineVirtualCamera.m_Follow = transform;
            cinemachineVirtualCamera.m_LookAt = transform;

            Utils.DebugLog("Spawned player with input authority");
        }
        else Utils.DebugLog("Spawned player without input authority");

        //Make it easier to tell which player is which.
        transform.name = $"P_{Object.Id}";

        if (!Object.HasStateAuthority)
        {
            Destroy(mainJoint);

            rigidbody3D.isKinematic = true;
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.InputAuthority == player)
            Runner.Despawn(Object);
    }
}
