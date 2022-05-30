using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class DeskAnimator : MonoBehaviour
{
    private float animationTime = 1f;
    [SerializeField] private GameObject movingPart;
    [SerializeField] private GameObject blinds;
    [SerializeField] private GameObject buttonParent;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject turnClock;
    [SerializeField] private GameObject playerBoard;
    [SerializeField] private GameObject enemyBoard;
    [SerializeField] private GameObject pinParent;
    [SerializeField] private GameObject menuParent;
    [SerializeField] private GameObject camDisplayParent;

    [ContextMenu(nameof(TransitionToGame))]
    public void TransitionToGame()
    {
        // This is where all the methods will be called
        FlipMovingPart();
        MoveButtonsDown();
        BringUpTheBlinds();
        MoveTurnClockUp();
    }

    private void FlipMovingPart()
    {
        LeanTween.rotateZ(movingPart, 180, animationTime).setOnStart(() =>
        {
            enemyBoard.SetActive(true);
        });
        LeanTween.moveLocalY(movingPart, 0.446f, animationTime).setOnComplete(() =>
        {
            playerBoard.SetActive(false);
            pinParent.SetActive(true);
            TurnOffTheMenu();
        });
    }
    
    private void MoveButtonsDown()
    {
        LeanTween.moveY(buttonParent, 0.8f, animationTime).setOnStart(() =>
        {
            foreach (GameObject button in buttons)
            {
                button.AddComponent<IgnoreHovering>();
            }
        }).setOnComplete(() =>
        {
            buttonParent.SetActive(false);
        });
    }

    private void MoveTurnClockUp()
    {
        LeanTween.moveLocalY(turnClock, 0.6f, animationTime);
    }

    private void BringUpTheBlinds()
    {
        LeanTween.moveLocalY(blinds, 2.34f, animationTime);
    }

    private void TurnOffTheMenu()
    {
        menuParent.SetActive(false);
        camDisplayParent.SetActive(true);
    }
}
