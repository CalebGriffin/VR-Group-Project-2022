using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DroneScript : MonoBehaviour
{
    [Header("Move camera to ship")]
    [SerializeField] private List<Transform> ships = new List<Transform>();
    [SerializeField] private float distanceFromShip = 100f;

    [Header("Birds eye position")]
    [SerializeField] private Transform birdsEyePos;

    [Header("Camera values")]
    private Camera droneCam;
    [SerializeField] private float birdsEyeHeight = 200f;
    [SerializeField] private float height = 7f;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private bool remainStatic = false;

    private bool assignPosition = true;
    private Transform target;



    // Start is called before the first frame update
    void Start()
    {
        droneCam = gameObject.GetComponent<Camera>();
        GameFeedbackEvents.instance.switchViewToShip += SwitchToShip;
        GameFeedbackEvents.instance.switchToBirdsEye += BirdsEyeView;
    }

    // Update is called once per frame
    void Update()
    {
        


        if (target == null)
            return;
        float x = Mathf.Cos(Time.time * rotationSpeed) * radius;
        float z = Mathf.Sin(Time.time * rotationSpeed) * radius;

        transform.position = new Vector3(target.position.x + x, height, target.position.z + z);
        transform.LookAt(target);
        if (!remainStatic)
        {

            
            return;
            //transform.position = new Vector3(target.position.x + 12f, height, target.position.z + 3f);
            //transform.position = new Vector3(target.position.x + 3f, height, target.position.z - 8f);
            
        }

    }


    public void SwitchToShip(Vector3 targetPos, int index)
    {
        target = null;
        droneCam.orthographic = false;
        //Called from the players side when the player takes a shot
        Transform ship = ships[index];
        Vector3 direction = (targetPos - ship.position).normalized;
        Vector3 cameraPoint = ship.position + (distanceFromShip * direction);
        transform.position = new Vector3(cameraPoint.x, height, cameraPoint.z);


        transform.LookAt(ship);
    }

    public void BirdsEyeView()
    {
        droneCam.orthographic = true;
        droneCam.orthographicSize = 260;

        transform.position = birdsEyePos.position;
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
    }


    public void SwitchTarget(Transform target)
    {
        //Called from the AI's side to focus on the attacked point
        this.target = target;
    }

}
