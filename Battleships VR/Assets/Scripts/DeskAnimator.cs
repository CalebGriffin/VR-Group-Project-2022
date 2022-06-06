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
    [SerializeField] private GameObject resetButton;
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

    public void TransitionToEnd(bool playerWon)
    {
        if (!playerWon)
            BringDownTheBlinds();
        TurnOnTheMenu(playerWon);
        MoveTurnClockDown();
        FlipMovingPartBack();
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

    private void FlipMovingPartBack()
    {
        LeanTween.rotateZ(movingPart, 360, animationTime).setOnStart(() =>
        {
            resetButton.SetActive(true);
            pinParent.SetActive(false);
        });
        LeanTween.moveLocalY(movingPart, 0.45f, animationTime).setOnComplete(() =>
        {
            enemyBoard.SetActive(false);
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
        LeanTween.moveLocalY(turnClock, 0.6f, animationTime).setOnStart(() =>
        {
            turnClock.SetActive(true);
        });
    }

    private void MoveTurnClockDown()
    {
        LeanTween.moveLocalY(turnClock, 0.2f, animationTime).setOnComplete(() =>
        {
            turnClock.SetActive(false);
        });
    }

    private void BringUpTheBlinds()
    {
        LeanTween.moveLocalY(blinds, 2.34f, animationTime);
    }

    private void BringDownTheBlinds()
    {
        LeanTween.moveLocalY(blinds, 0.0902563f, animationTime);
    }

    private void TurnOffTheMenu()
    {
        menuParent.SetActive(false);
        camDisplayParent.SetActive(true);
    }

    private void TurnOnTheMenu(bool playerWon)
    {
        camDisplayParent.SetActive(false);
        UIManager.instance.ResetMenuText();
        menuParent.SetActive(true);
        UIManager.instance.DisplayEndText(playerWon);
    }
}
