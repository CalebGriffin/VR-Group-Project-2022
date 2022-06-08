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

    [SerializeField] private GameObject[] uiElementsInTheSea;

    private bool assignPosition = true;
    private Transform target;

    private bool alwaysLook = false;

    // Start is called before the first frame update
    void Start()
    {
        alwaysLook = false;
        droneCam = gameObject.GetComponent<Camera>();
        GameFeedbackEvents.instance.switchViewToShip += SwitchToShip;
        GameFeedbackEvents.instance.switchToBirdsEye += BirdsEyeView;
        GameFeedbackEvents.instance.switchToWaterView += SwitchTarget;
        GameFeedbackEvents.instance.switchToPlaneView += LookAtPlane;
    }

    // Update is called once per frame
    void Update()
    {
        if (alwaysLook && target != null)
            transform.LookAt(target);


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
        ToggleUIElements(false);
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
        ToggleUIElements(true);
        droneCam.orthographic = true;
        droneCam.orthographicSize = 330;

        transform.position = birdsEyePos.position;
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
    }


    public void SwitchTarget(Transform target)
    {
        ToggleUIElements(false);
        droneCam.orthographic = false;
        //Called from the AI's side to focus on the attacked point
        this.target = target;
        StartCoroutine(DestroyTarget());
    }
    private IEnumerator DestroyTarget(float waitTimer = 2f)
    {
        yield return new WaitForSeconds(waitTimer);
        target = null;
        alwaysLook = false;
    }

    private void LookAtPlane(Transform target)
    {
        this.target = target;
        alwaysLook = true;
        StartCoroutine(DestroyTarget(10f));
    }

    private void ToggleUIElements(bool to)
    {
        foreach (GameObject x in uiElementsInTheSea)
        {
            x.SetActive(to);
        }
    }


}
