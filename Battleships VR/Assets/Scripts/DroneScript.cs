using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneScript : MonoBehaviour
{
    [Header("Move camera to ship")]
    [SerializeField] private List<Transform> ships = new List<Transform>();
    [SerializeField] private float distanceFromShip = 100f;

    [Header("Camera values")]
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
        GameFeedbackEvents.instance.switchViewToShip += SwitchToShip;
        //SwitchToShip(ships[3]);
        //target = ships[3];
        //target = ships[0];
        //SwitchTarget(target);
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
        //Called from the players side when the player takes a shot
        Transform ship = ships[index];
        Vector3 direction = (targetPos - ship.position).normalized;
        Vector3 cameraPoint = ship.position + (distanceFromShip * direction);
        transform.position = new Vector3(cameraPoint.x, height, cameraPoint.z);


        transform.LookAt(ship);
    }

    public void BirdsEyeView()
    {
        transform.position = new Vector3(0, birdsEyeHeight, 0);
        transform.LookAt(Vector3.down);
    }


    public void SwitchTarget(Transform target)
    {
        //Called from the AI's side to focus on the attacked point
        this.target = target;
    }

}
