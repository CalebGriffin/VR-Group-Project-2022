using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DansLibrary;

public class Plane : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private MeshRenderer rend;

    private Vector3 targetObj;
    [SerializeField] private Transform startingPos;

    Vector3 forward;

    private float speed = 1f;
    private bool isMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rend = gameObject.GetComponent<MeshRenderer>();

        rend.gameObject.SetActive(false);
        
        isMoving = false;
        //Invoke("ReachedEnd", 1.5f);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(isMoving)
        {
            forward = transform.TransformDirection(Vector3.forward);
            rb.velocity = forward * speed;
            Accellerate();
        }
    }

    private void Accellerate()
    {
        float initialVelocity = rb.velocity.magnitude;
        float finalVelocity = rb.velocity.magnitude + 2f;
        float initialTime = Time.time;
        float finalTime = Time.time + 5f;

        speed += (finalVelocity - initialVelocity) / (finalTime - initialTime);


    }

    public void StartPlaneMovement(Vector3 target)
    {
        rend.gameObject.SetActive(true);
        targetObj = target;
        isMoving = true;

        GlobalTimer timer = new GlobalTimer(10f);
        StartCoroutine(timer.ScaledTimer());
        timer.timerCompleted += ReturnToStartingPosition;

        Invoke("ReachedEnd", 3f);
    }

    private void ReachedEnd()
    {
        StartCoroutine(TurnToTarget());
    }
    private IEnumerator TurnToTarget()
    {
        Vector3 directionToTarget = targetObj - rb.transform.position;
        float angle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);

        StartCoroutine(IncreaseHeight());

        while (CalculateDotProduct(directionToTarget) == false)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, angleAxis, 10f * Time.deltaTime);
            
            yield return new WaitForEndOfFrame();
        }


    }

    private bool CalculateDotProduct(Vector3 directionToTarget)
    {
        directionToTarget.y = 0;
        Vector3 forwardDir = transform.forward;
        forwardDir.y = 0;

        forwardDir.Normalize();
        directionToTarget.Normalize();

        float dotProduct = Vector3.Dot(forwardDir, directionToTarget);

        if (dotProduct >= 0.99)
            return true;
        return false;

    }

    private IEnumerator IncreaseHeight()
    {

        float yValue = 0.02f;
        for(int i = 0; i < 1000; i++)
        {
            if (i >= 30)
                yValue += 0.01f;

            transform.position = transform.position + new Vector3(0, yValue, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void ReturnToStartingPosition()
    {
        rend.gameObject.SetActive(false);
        gameObject.transform.position = startingPos.position;
        gameObject.transform.localEulerAngles = new Vector3(0, -101, 0);
        speed = 1f;
        isMoving = false;
        GameFeedbackEvents.instance.SwitchToBirdsEye();
        gVar.playerTurnOver = true;
    }
}
