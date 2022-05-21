using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    
    [SerializeField] private float compensationRotation = 0f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private List<GameObject> gunBarrels;
    [SerializeField] private GameObject explosionParticle;

    //TESTING
    private Transform barrel;

    private Vector3 directionToLook;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Barrel"))
                gunBarrels.Add(child.gameObject);
            //Debug.Log(child);
        }

    }

    public void Fire(Vector3 target)
    {
        //Only rotate the guns that are on the correct side by firing a raycast and checking if it collides with anything
        Transform barrel = gameObject.GetComponentInChildren<Transform>();
        Vector3 direction = target - barrel.position;

        //TESTING
        this.barrel = barrel;


        directionToLook = direction;

        if (Physics.Raycast(transform.position, direction, 200f, LayerMask.GetMask("Ships")))
        {
            Debug.Log("Hitting something");
            return;
        }

        StartCoroutine(RotateGuns(direction.normalized));
    }

    private void Update()
    {
        //TESTING
        if (barrel == null && directionToLook == Vector3.zero)
            return;
        Debug.DrawRay(barrel.position, directionToLook, Color.green);
    }


    private IEnumerator RotateGuns(Vector3 direction)
    {
        Debug.DrawRay(transform.position, direction);

        //Get the angle 
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        if (compensationRotation != 0)
            angle += compensationRotation;
        Debug.Log("Calculated angle " + angle);

        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);

        //TESTING
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        while (CalculateDotProduct(direction) == false)
        {
            Debug.Log("Rotating");
            //Slowly transition from the current rotation to the target angle
            transform.rotation = Quaternion.RotateTowards(transform.rotation, angleAxis, Time.deltaTime * rotationSpeed);

            yield return new WaitForEndOfFrame();
        }
        
        ShootGuns();

    }

    private bool CalculateDotProduct(Vector3 direction)
    {
        Vector3 forwardDirection = transform.forward;
        direction.y = 0;
        forwardDirection.y = 0;
        forwardDirection.Normalize();
        direction.Normalize();

        float dotProduct;
        if (compensationRotation != 0)
            dotProduct = Vector3.Dot(-transform.forward, direction);
        else
            dotProduct = Vector3.Dot(transform.forward, direction);

        //Check if the local forward vector of the guns is in the same direction as the target direction
        //Debug.Log(dotProduct);
        if(dotProduct >= 0.99)
        {
            Debug.Log("Value is true");
            return true;
        }
        return false;
    }


    private void ShootGuns()
    {
        if (gunBarrels.Count <= 0)
            return;
        foreach(GameObject barrel in gunBarrels)
        {
            Animator anim = barrel.GetComponent<Animator>();
            anim.SetBool("isFiring", true);
            //explosionParticle.SetActive(true);
            Debug.Log("Ran shoot animation");
            StartCoroutine(StopShooting(anim));
        }

    }


    private IEnumerator StopShooting(Animator anim)
    {
        yield return new WaitForSeconds(2f);
        anim.SetBool("isFiring", false);
        //explosionParticle.SetActive(false);
    }

    private void RotateBarrles()
    {

    }
}
