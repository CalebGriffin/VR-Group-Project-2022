using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneScript : MonoBehaviour
{
    [SerializeField] private List<Transform> ships = new List<Transform>();

    [SerializeField] private float height = 7f;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private bool remainStatic = false;
    private bool assignPosition = true;
    private Transform target;

    [SerializeField] private Transform dronePos;
    private List<Transform> gunsToLook = new List<Transform>();





    // Start is called before the first frame update
    void Start()
    {

        SwitchToShip(ships[0]);
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

        

        


        

        transform.LookAt(target);




        if (!remainStatic)
        {

            transform.position = new Vector3(target.position.x + x, height, target.position.z + z);
            return;
            //transform.position = new Vector3(target.position.x + 12f, height, target.position.z + 3f);
            //transform.position = new Vector3(target.position.x + 3f, height, target.position.z - 8f);
            
        }

    }


    private void AdjustCameraRotation()
    {
        //Get the current rotation of the boat 
        float targetRotation = target.transform.localRotation.eulerAngles.y;
        //when rotation is 0
        //x + 3f, z - 8f
    }


    public void SwitchToShip(Transform ship)
    {
        Vector3 position = new Vector3(ship.position.x, 0, ship.position.z);
        transform.position = position;
        transform.LookAt(ship);
    }


    public void SwitchTarget(Transform target)
    {
        this.target = target;






    }
}
