using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class TestRunner : MonoBehaviour
{
    public float timeDelay;

    public AI ai;
    public AI2 ai2;

    public bool ai1Turn = true;

    public bool playing = true;

    public bool simulating = false;
    
    int simulationsRun = 0;

    int aiWins = 0;
    int ai2Wins = 0;

    int aiMoves = 0;
    int ai2Moves = 0;

    public Text buttonText;

    public TextMeshProUGUI statsText;

    public GameObject[] stuffToDestroy;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Run());
        UpdateStatsText("Running");
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(timeDelay);

        if (playing && ai1Turn)
        {
            ai.Decision();
            aiMoves++;
            ai1Turn = false;
        }

        yield return new WaitForSeconds(timeDelay);

        if (playing && !ai1Turn)
        {
            ai2.Decision();
            ai2Moves++;
            ai1Turn = true;
        }

        StartCoroutine(Run());
    }

    public void Button(string winner)
    {
        playing = !playing;
        buttonText.text = playing ? "Pause" : "Play";
        StopSimulation(winner);
    }

    private IEnumerator StartSimulation()
    {
        yield return new WaitForSeconds(1);
        Simulate();
    }

    private void Simulate()
    {
        while(simulating)
        {
            ai.Decision();
            if (!simulating)
            {
                StopSimulation("AI2");
            }
            ai2.Decision();
            if (!simulating)
            {
                StopSimulation("AI");
            }
        }
    }

    public void StopSimulation(string winner)
    {
        simulationsRun++;
        playing = false;
        if (winner == "AI")
        {
            // Add to text file that AI won
            aiWins++;
            //Debug.Log("AI won");
            StreamWriter writer = new StreamWriter("Assets/Testing Stuff/Stats2.txt", true);
            int winnersRemainingPositions = 0;
            foreach (Boat boat in ai.Boats)
            {
                winnersRemainingPositions += boat.RemainingPositions.Count;
            }
            float aiWinPercentage = ((float)aiWins / (float)simulationsRun) * 100;
            writer.WriteLine($"AI won, {aiMoves} moves, {winnersRemainingPositions} left, {simulationsRun} simulations, {aiWinPercentage}%");
            writer.Close();
        }
        else if (winner == "AI2")
        {
            // Add to text file that AI2 won
            ai2Wins++;
            //Debug.Log("AI2 won");
            StreamWriter writer = new StreamWriter("Assets/Testing Stuff/Stats2.txt", true);
            int winnersRemainingPositions = 0;
            foreach (Boat boat in ai2.Boats)
            {
                winnersRemainingPositions += boat.RemainingPositions.Count;
            }
            float aiWinPercentage = ((float)aiWins / (float)simulationsRun) * 100;
            writer.WriteLine($"AI2 won, {ai2Moves} moves, {winnersRemainingPositions} left, {simulationsRun} simulations, {aiWinPercentage}%");
            writer.Close();
        }
        Reset();
        if (simulationsRun % 10 == 0)
        {
            Debug.Break();
            UpdateStatsText("Paused");
            UpdateStatsText("Running");
            playing = true;
        }
        else
        {
            UpdateStatsText("Running");
            playing = true;
        }
    }

    private void Reset()
    {
        aiMoves = 0;
        ai2Moves = 0;
        ai.Start();
        ai2.Start();
    }

    private void UpdateStatsText(string isRunning)
    {
        float aiWinPercentage = (float)aiWins / (float)simulationsRun * 100;
        statsText.text = @$"Simulation is: {isRunning}
Game Number: {simulationsRun}
AI Wins: {aiWins}
AI2 Wins: {ai2Wins}
AI Wins % = {aiWinPercentage}%";
    }

}
