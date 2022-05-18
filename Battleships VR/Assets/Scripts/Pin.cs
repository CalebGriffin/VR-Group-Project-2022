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
    }

    public void OnLetGo()
    {
        if (!hoveringOverTheBoard)
        {
            return;
        }

        placed = true;
        hoveringOverTheBoard = false;

        // Call the method to shoot at the AI
        int shootPosition = player.Board.Matrix[(int)dynamicPosition.localPosition.x, (int)dynamicPosition.localPosition.z];

        player.Decision(shootPosition);
    }
}
