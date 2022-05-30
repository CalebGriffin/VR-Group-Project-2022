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
        if (placed)
        {
            AddIgnoreHovering();
        }
        else if (!gVar.playerTurn)
        {
            AddIgnoreHovering();
        }
        else if (this.gameObject.GetComponent<IgnoreHovering>() != null)
        {
            RemoveIgnoreHovering();
        }

        if (!placed)
        {
            FireRaycast();
        }
    }

    private void AddIgnoreHovering()
    {
        if (this.gameObject.GetComponent<IgnoreHovering>() != null)
            return;

        this.gameObject.AddComponent<IgnoreHovering>();
        foreach (Transform child in transform)
        {
            child.gameObject.AddComponent<IgnoreHovering>();
        }
    }

    private void RemoveIgnoreHovering()
    {
        if (this.gameObject.GetComponent<IgnoreHovering>() == null)
            return;
        
        Destroy(this.gameObject.GetComponent<IgnoreHovering>());

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject.GetComponent<IgnoreHovering>());
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

    public void Reset()
    {
        ResetLockPoint();
        dynamicPosition.localPosition = new Vector3(0,0,0);
        hoveringOverTheBoard = false;
        placed = false;
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
        if (!GetComponent<LockToPoint>().snapTo == dynamicPosition || !hoveringOverTheBoard)
        {
            return;
        }

        placed = true;
        hoveringOverTheBoard = false;



        StartCoroutine(Disappear());
    }

    public IEnumerator Disappear()
    {
        // Call the method to shoot at the AI
        int shootPosition = player.Board.Matrix[Mathf.RoundToInt(dynamicPosition.localPosition.x), Mathf.RoundToInt(dynamicPosition.localPosition.z)];
        Debug.Log(dynamicPosition.localPosition.ToString());
        Debug.Log(Mathf.RoundToInt(dynamicPosition.localPosition.x) + ", " + Mathf.RoundToInt(dynamicPosition.localPosition.z));
        Debug.Log("Which means that the fired position is: " + player.Board.Matrix[Mathf.RoundToInt(dynamicPosition.localPosition.x), Mathf.RoundToInt(dynamicPosition.localPosition.z)]);

        player.Decision(shootPosition);

        while (gVar.playerTurnOver == false)
        {
            yield return null;
        }

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        gVar.playerTurnOver = false;

        // Call a method to spawn the next pin
        pinHolder.SpawnPin();

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
