using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public PlayerMovement pm;

    [Header("AirMovement")]
    public Transform orientation;
    public Rigidbody rb;
    public float horziontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    [Header("Swinging")]
    private float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;



    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;
    private Vector3 currentGrapplePosition;

    void Update()
    {
        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

        if (joint != null) AirMovement();
    }

    private void StartSwing()
    {

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {

            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }

    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StopSwing()
    {
        
        lr.positionCount = 0;
        Destroy(joint);

    }

    void DrawRope()
    {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }

    private void AirMovement()
    {
        //right
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horziontalThrustForce * Time.deltaTime);
        //left
        if(Input.GetKeyDown(KeyCode.A)) rb.AddForce(-orientation.right * horziontalThrustForce * Time.deltaTime);
        //forward
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);

        //shorten cable
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);
           
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
           

        }

        //extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendDistanceFromPoint * 0.8f;
            joint.minDistance = extendDistanceFromPoint * 0.25f;

        }





    }


}
