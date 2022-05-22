using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    
    [SerializeField] private float compensationRotation = 0f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private List<GameObject> gunBarrels;
    [SerializeField] private GameObject explosionParticle;



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

    public void Fire(Vector3 target, int amount)
    {
        //Barrel used here instead of the gun transform because the origin is too far back on some guns
        Transform barrel = gameObject.GetComponentInChildren<Transform>();
        Vector3 direction = target - barrel.position;

        //Ensure that the guns don't rotate and fire on themselves 
        if (Physics.Raycast(transform.position, direction, 200f, LayerMask.GetMask("Ships")))
        {
            Debug.Log("Hitting something");
            return;
        }

        StartCoroutine(RotateGuns(direction.normalized, amount));
    }


    private IEnumerator RotateGuns(Vector3 direction, int amount)
    {

        //Get the angle needed to turn to face the direction
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        //Some of the guns are defaulted to face the opposite direction of the local z transfrom 
        //the compensation angle makes up for that 180 degree rotation
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
        
        StartCoroutine(ShootGuns(amount));

    }

    private bool CalculateDotProduct(Vector3 direction)
    {
        Vector3 forwardDirection = transform.forward;
        //Set both direction vector's y value to 0 to calculate the dot product across a flat plane
        direction.y = 0;
        forwardDirection.y = 0;
        //Normlize both vectors after the y value has been changed to ensure they are properly normalized
        forwardDirection.Normalize();
        direction.Normalize();

        //Find the dot prodcut but acount for the guns facing the wrong direction
        float dotProduct;
        if (compensationRotation != 0)
            dotProduct = Vector3.Dot(-transform.forward, direction);
        else
            dotProduct = Vector3.Dot(transform.forward, direction);

        //Check if the local forward vector of the guns is in the same direction as the target direction
        if(dotProduct >= 0.99)
        {
            Debug.Log("Value is true");
            return true;
        }
        return false;
    }


    private IEnumerator ShootGuns(int amount)
    {
        //Return if there aren't any gun barrels to avoid a null reference 
        if (gunBarrels.Count <= 0)
            yield return null;

        for(int i = 0; i < amount; i++)
        {
            foreach (GameObject barrel in gunBarrels)
            {
                Animator anim = barrel.GetComponent<Animator>();
                anim.SetBool("isFiring", true);
                explosionParticle.SetActive(true);
                Debug.Log("Ran shoot animation");
                StartCoroutine(StopShooting(anim));
            }
            yield return new WaitForSeconds(2.5f);
        }

        //THIS IS WHERE THE PLAYERS TURN SHOULD BE NOTIFIED TO END

    }


    private IEnumerator StopShooting(Animator anim)
    {
        yield return new WaitForSeconds(2f);
        anim.SetBool("isFiring", false);
        explosionParticle.SetActive(false);
    }
}
