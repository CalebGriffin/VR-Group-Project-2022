using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    private Rigidbody rb;

    private Transform targetObj;

    Vector3 forward;

    private float speed = 1f;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        //Invoke("ReachedEnd", 1.5f);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        forward = transform.TransformDirection(Vector3.forward);
        rb.velocity = forward * speed;
        Accellerate();
    }

    private void Accellerate()
    {
        float initialVelocity = rb.velocity.magnitude;
        float finalVelocity = rb.velocity.magnitude + 2f;
        float initialTime = Time.time;
        float finalTime = Time.time + 30f;

        speed += (finalVelocity - initialVelocity) / (finalTime - initialTime);


    }

    public void StartPlaneMovement()
    {
        Invoke("ReachedEnd", 1.5f);
    }

    private void ReachedEnd(Vector3 target)
    {
        targetObj.position = target;
        StartCoroutine(TurnToTarget());
    }
    private IEnumerator TurnToTarget()
    {
        Vector3 directionToTarget = targetObj.position - rb.transform.position;
        float angle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);

        StartCoroutine(IncreaseHeight());

        while (CalculateDotProduct(directionToTarget) == false)
        {
            Debug.Log("Coroutine ran");
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
            Debug.Log("Increasing height");
            if (i >= 30)
                yValue += 0.01f;

            transform.position = transform.position + new Vector3(0, yValue, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
