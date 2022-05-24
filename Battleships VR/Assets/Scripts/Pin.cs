using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR.InteractionSystem.Sample;

public class Pin : MonoBehaviour
{
    public bool hoveringOverTheBoard = false;

    public bool placed = false;

    private float raycastDistance = 1f;

    [SerializeField] private GameObject enemyBoardParent;

    private RaycastHit hit;

    public Transform originalPosition;
    public Transform dynamicPosition;

    [SerializeField] private Player player;

    public PinHolder pinHolder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (gVar.playerTurn)
            GetComponent<Rigidbody>().WakeUp();
        else
            GetComponent<Rigidbody>().Sleep();

        GetComponent<Interactable>().enabled = gVar.playerTurn ? true : false;
        GetComponent<Throwable>().enabled = GetComponent<Interactable>().enabled;

        if (!placed)
        {
            FireRaycast();
        }
    }

    private void FireRaycast()
    {
        Debug.DrawRay(transform.position, Vector3.down * raycastDistance, Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance) && hit.collider.tag == "Enemy Board")
        {
            if (!hoveringOverTheBoard)
            {
                hoveringOverTheBoard = true;
                enemyBoardParent.GetComponent<EnemyBoardInput>().OnPinHoverEnter(this.gameObject);
            }
            else
            {
                enemyBoardParent.GetComponent<EnemyBoardInput>().OnPinHoverStay(this.gameObject);
            }
        }
        else if (hoveringOverTheBoard)
        {
            hoveringOverTheBoard = false;
            enemyBoardParent.GetComponent<EnemyBoardInput>().OnPinHoverExit(this.gameObject);
        }
    }

    public void SetLockPoint(Transform previewPinTransform)
    {
        dynamicPosition.position = previewPinTransform.position;
        this.GetComponent<LockToPoint>().snapTo = dynamicPosition;
    }

    public void ResetLockPoint()
    {
        this.GetComponent<LockToPoint>().snapTo = originalPosition;
    }

    public void OnPickUp()
    {
        placed = false;
        ResetLockPoint();
    }

    public void OnLetGo()
    {
        Debug.Log("Getting Here 0");
        if (!GetComponent<LockToPoint>().snapTo == dynamicPosition)
        {
            return;
        }

        placed = true;
        hoveringOverTheBoard = false;


        Debug.Log("Getting Here 1");

        StartCoroutine(Disappear());
    }

    public IEnumerator Disappear()
    {
        Debug.Log("Getting Here 2");
        yield return new WaitForSeconds(1);

        Debug.Log("Getting Here 3");
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        Debug.Log("Getting Here 4");
        yield return new WaitForSeconds(0.5f);

        // Call the method to shoot at the AI
        int shootPosition = player.Board.Matrix[(int)dynamicPosition.localPosition.x, (int)dynamicPosition.localPosition.z];

        player.Decision(shootPosition);

        Debug.Log("Getting Here 5");
        // Call a method to spawn the next pin
        pinHolder.SpawnPin();
        Debug.Log("Getting Here 6");

        // Destroy this GameObject
        Destroy(this.gameObject);
        Debug.Log("Getting Here 7");
    }
}
