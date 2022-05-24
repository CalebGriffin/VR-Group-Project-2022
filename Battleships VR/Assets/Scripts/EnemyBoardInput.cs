using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoardInput : MonoBehaviour
{
    [SerializeField] private PreviewPin previewPin;
    [SerializeField] private Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPinHoverEnter(GameObject pin)
    {
        // This method is here just in case we need it
    }

    public void OnPinHoverStay(GameObject pin)
    {
        if (!pin.GetComponent<Pin>().placed && pin.GetComponent<Pin>().hoveringOverTheBoard)
        {
            Vector3 previewPosition = FindRoundedPosition(pin.transform.localPosition);

            if (previewPosition != new Vector3(100, 100, 100))
            {
                previewPin.Show();
                previewPin.ChangePosition(previewPosition);

                pin.GetComponent<Pin>().SetLockPoint(previewPin.transform);
            }
            else
            {
                previewPin.Hide();
                pin.GetComponent<Pin>().ResetLockPoint();
            }
        }
    }

    public void OnPinHoverExit(GameObject pin)
    {
        pin.GetComponent<Pin>().placed = false;
        pin.GetComponent<Pin>().ResetLockPoint();

        previewPin.Hide();
    }

    private Vector3 FindRoundedPosition(Vector3 position)
    {
        int newX = RoundFloat(position.x);
        int newZ = RoundFloat(position.z);

        int boardPosition = int.Parse(newX.ToString() + (newZ + 1).ToString());

        if (player.uncheckedPositions.Contains(boardPosition))
        {
            return new Vector3(newX, 1, newZ);
        }
        else
        {
            return new Vector3(100, 100, 100);
        }
    }

    private int RoundFloat(float number)
    {
        int diff = (int)number % 1;

        number -= diff;

        if (diff > 1 / 2)
            number += 1;
        
        return (int)number;
    }
}
