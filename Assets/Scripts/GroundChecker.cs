using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public static GroundChecker singleton;

    private bool isGrounded;
    public bool IsGrounded { get => isGrounded;  set => isGrounded = value; }

    public Vector3 GroundNormal { get; private set; } = Vector3.up;
   

    [SerializeField] private float raycastLength = 1.2f;
    [SerializeField] private LayerMask groundMask;
    [System.NonSerialized] public float distanceOfPlayerFromGroundHitPoint=1000f;
    [System.NonSerialized] public bool isGroundDetected = false;

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
       
    }

    private void FixedUpdate()
    {
        // Perform raycast from slightly above the player
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, raycastLength, groundMask))
        {
            //IsGrounded = true;
            GroundNormal = hit.normal;
            distanceOfPlayerFromGroundHitPoint = Vector3.Distance(transform.position, hit.point);
            isGroundDetected = true;
           
           
            
        }
        else
        {
            // IsGrounded = false;
            GroundNormal = Vector3.up; // fallback
            distanceOfPlayerFromGroundHitPoint =1000f;//it(1000) means no ground detected below 
            isGroundDetected = false;
        }

        if (distanceOfPlayerFromGroundHitPoint < 0.4)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }

    
}
