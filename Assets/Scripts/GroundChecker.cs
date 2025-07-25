using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public static GroundChecker singleton;

    private bool isGrounded;
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }

    public Vector3 GroundNormal { get; private set; } = Vector3.up;


    [SerializeField] private float raycastLength = 2500f;
    [SerializeField] private LayerMask groundMask;
    [System.NonSerialized] public float distanceOfPlayerFromGroundHitPoint = 1000f;
    [System.NonSerialized] public bool isGroundDetected = false;
    private Queue<float> previousDistancesFromGround = new Queue<float>() ;
    private int maxSizeOfTheQueueOfPrevDistancesFromGround = 3;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        previousDistancesFromGround.Enqueue(Mathf.Infinity);
    }
    private void Update()
    {
       
    }

    private void FixedUpdate()
    {
        // Perform raycast from slightly above the player
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, raycastLength, groundMask))
        {
        
            GroundNormal = hit.normal;
            distanceOfPlayerFromGroundHitPoint = Vector3.Distance(transform.position, hit.point);
            isGroundDetected = true;
           
           
            
        }
        else
        {
     
            GroundNormal = Vector3.up; // fallback
            distanceOfPlayerFromGroundHitPoint =1000f;//it(1000) means no ground detected below 
            isGroundDetected = false;
        }


        if (previousDistancesFromGround.Count >= maxSizeOfTheQueueOfPrevDistancesFromGround)
        {
            previousDistancesFromGround.Dequeue();
        }
       
            previousDistancesFromGround.Enqueue(distanceOfPlayerFromGroundHitPoint);

        if (Mathf.Abs(previousDistancesFromGround.Peek() - distanceOfPlayerFromGroundHitPoint) >= 0.05)
        {
           
      //not in ground
            IsGrounded = false;
        }
        else
        {
            //is in ground
            IsGrounded = true;
        }
        

        

    

    }

    
}
