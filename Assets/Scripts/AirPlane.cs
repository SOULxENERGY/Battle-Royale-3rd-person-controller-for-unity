using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AirPlane : MonoBehaviour
{
    [SerializeField] private Transform startingPoint;
    [SerializeField] private Transform destinyPoint;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stoppingDist = 0.1f;

    private Vector3 moveDir;

    [SerializeField] private List<Transform> allBlades = new List<Transform>();
    [SerializeField] private float bladeSpeed = 10f;
    [SerializeField] private Transform playerSpawnPos;
    
    [SerializeField] private Transform player;

    void Start()
    {
        
        if (startingPoint != null && destinyPoint != null)
        {
            transform.position = startingPoint.position;
            transform.LookAt(destinyPoint.position);
            moveDir = (destinyPoint.position - startingPoint.position).normalized;
        }
        else
        {
            Debug.LogError("Starting point or destiny point is not set.");
            enabled = false;
        }
       
       
    }

    public void SkyDive(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            
            Transform player = Instantiate(this.player);

            // Disable entire player before teleport
            player.gameObject.SetActive(false);

            // Now safe to change position
            player.transform.position = playerSpawnPos.position;

            // Reactivate
            player.gameObject.SetActive(true);
        }
       

    }

    void FixedUpdate()
    {
       
            float distFromDestiny = Vector3.Distance(transform.position, destinyPoint.position);
       
        if (distFromDestiny > stoppingDist)
            {
            moveDir = (destinyPoint.position - transform.position).normalized;
           // Debug.Log(distFromDestiny);
                transform.position += moveDir * speed * Time.fixedDeltaTime;
            }
            else
            {
         
                transform.position = destinyPoint.position;
                moveDir = Vector3.zero; // Stop movement
            }

        foreach (Transform blade in allBlades)
        {
            blade.Rotate(new Vector3(bladeSpeed * Time.fixedDeltaTime, 0, 0));
        }
    }
    
}
