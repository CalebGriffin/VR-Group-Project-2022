using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using TMPro;
using System.Net.Mail;

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
    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new System.Net.NetworkCredential("calebegriffin@gmail.com", "owytnconrwkulynr"),
        EnableSsl = true
    };

    private float loopingTimePassed = 0;
    private bool sentFailEmail = false;

    // Start is called before the first frame update
    void Start()
    {
        ai.StartCoroutine(nameof(Reset));
        ai2.StartCoroutine(nameof(Reset));
        StartCoroutine(Run());
        UpdateStatsText(true);
    }

    void Update()
    {
        if (!sentFailEmail)
            loopingTimePassed += Time.deltaTime;
        if (loopingTimePassed > 30)
        {
            sentFailEmail = true;
            loopingTimePassed = 0;
            SendFailEmail();
        }
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
        StartCoroutine("StopSimulation", winner);
    }

    public void ShootButton()
    {
        if (ai1Turn)
        {
            ai.Decision();
            aiMoves++;
            ai1Turn = false;
        }
        else
        {
            ai2.Decision();
            ai2Moves++;
            ai1Turn = true;
        }
    }

    public IEnumerator StopSimulation(string winner)
    {
        simulationsRun++;
        //stuffToDestroy = GameObject.FindGameObjectsWithTag("StuffToDestroy");
        //foreach (GameObject stuff in stuffToDestroy)
        //{
            //Destroy(stuff);
        //}
        playing = false;
        if (winner == "AI")
        {
            // Add to text file that AI won
            aiWins++;
            //Debug.Log("AI won");
            //StreamWriter writer = new StreamWriter("Assets/Testing Stuff/Stats2.txt", true);
            //int winnersRemainingPositions = 0;
            //foreach (Boat boat in ai.Boats)
            //{
                //winnersRemainingPositions += boat.RemainingPositions.Count;
            //}
            //float aiWinPercentage = ((float)aiWins / (float)simulationsRun) * 100;
            //writer.WriteLine($"AI won, {aiMoves} moves, {winnersRemainingPositions} left, {simulationsRun} simulations, {aiWinPercentage}%");
            //writer.Close();
        }
        else if (winner == "AI2")
        {
            // Add to text file that AI2 won
            ai2Wins++;
            //Debug.Log("AI2 won");
            //StreamWriter writer = new StreamWriter("Assets/Testing Stuff/Stats2.txt", true);
            //int winnersRemainingPositions = 0;
            //foreach (Boat boat in ai2.Boats)
            //{
                //winnersRemainingPositions += boat.RemainingPositions.Count;
            //}
            //float aiWinPercentage = ((float)aiWins / (float)simulationsRun) * 100;
            //writer.WriteLine($"AI2 won, {ai2Moves} moves, {winnersRemainingPositions} left, {simulationsRun} simulations, {aiWinPercentage}%");
            //writer.Close();
        }
        Reset();
        yield return new WaitForSeconds(0.001f);
        if (simulationsRun % 5000 == 0)
        {
            UpdateStatsText(false);
            WriteToFile();
            SendSuccessEmail();
            Debug.Break();
            playing = true;
        }
        else if (simulationsRun % 100 == 0)
        {
            UpdateStatsText(true);
            WriteToFile();
            playing = true;
        }
        else
        {
            UpdateStatsText(true);
            playing = true;
        }
    }

    public void Reset()
    {
        ai.StartCoroutine(nameof(Reset));
        ai2.StartCoroutine(nameof(Reset));
        stuffToDestroy = GameObject.FindGameObjectsWithTag("StuffToDestroy");
        foreach (GameObject stuff in stuffToDestroy)
        {
            Destroy(stuff);
        }
        aiMoves = 0;
        ai2Moves = 0;
        loopingTimePassed = 0;
    }

    private void WriteToFile()
    {
        StreamWriter writer = new StreamWriter($"Assets/Testing Stuff/Output{ai.StartingDeviation}.txt", true);
        float aiWinPercentage = ((float)aiWins / (float)simulationsRun) * 100;
        writer.WriteLine($"{simulationsRun}, {aiWinPercentage.ToString("F5")}");
        writer.Close();
    }

    private void SendSuccessEmail()
    {
        smtpClient.Send("calebegriffin@gmail.com", "calebegriffin@gmail.com", "ATTENTION", "This email is just to let you know that the simulation has finished!");
    }

    private void SendFailEmail()
    {
        smtpClient.Send("calebegriffin@gmail.com", "calebegriffin@gmail.com", "OH NO!", $"This email is just to let you know that the simulation has failed after {simulationsRun} games!");
    }

    private void UpdateStatsText(bool isRunning)
    {
        string isRunningString = isRunning ? "Running" : "Paused";
        if (!isRunning)
            Debug.Break();
        float aiWinPercentage = (float)aiWins / (float)simulationsRun * 100;
        statsText.text = @$"Simulation is: {isRunningString}
Game Number: {simulationsRun}
AI Wins: {aiWins}
AI2 Wins: {ai2Wins}
AI Win % = {aiWinPercentage.ToString("F4")}%";
    }

}
