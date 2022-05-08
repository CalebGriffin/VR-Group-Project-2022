using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mail;

public class EmailTest : MonoBehaviour
{
    [ContextMenu(nameof(SendTestEmail))]
    private void SendTestEmail()
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential("calebegriffin@gmail.com", "owytnconrwkulynr"),
            EnableSsl = true
        };

        smtpClient.Send("calebegriffin@gmail.com", "calebegriffin@gmail.com", "Test Email", "This is a test email.");
    }
}
