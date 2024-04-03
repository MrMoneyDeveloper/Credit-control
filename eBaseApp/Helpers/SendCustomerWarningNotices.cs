using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;

namespace eBaseApp.Helpers
{
    public class SendCustomerWarningNotices
    {
        public static async Task SendWhatsAppNotification(string recipientNumber, string message)
        {
            var whatsAppObject = new WhatsAppObjectBuilder(recipientNumber, message);
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://89ykr3.api.infobip.com/whatsapp/1/message/text");
            request.Headers.Add("Authorization", "App 731e7a356e13d572b378f95cbf7c35af-eea3adfd-b92d-43b2-8940-3440945c4b77");
            var content = new StringContent(JsonSerializer.Serialize(whatsAppObject), null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return;
        }

        public static async Task SendCustomerSMS(string recipientNumber, string message)
        {
            var grapeVine = new GrapeVine(recipientNumber, message);
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://129.232.208.13/GrapeVine/api/sms/send");
            var content = new StringContent(JsonSerializer.Serialize(grapeVine), null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return;
        }
    }
}


public class WhatsAppObjectBuilder
{
    public string from { get; set; }
    public string to { get; set; }
    public string messageId { get; set; }
    public Content content { get; set; }

    public WhatsAppObjectBuilder(string recipientNumber, string message)
    {
        this.to = recipientNumber;
        this.from = "27829908994";
        this.messageId = "a28dd97c-1ffb-4fcf-99f1-0b557ed381d";
        this.content = new Content("PRE-TERMINATION NOTICE is hereby given in terms of the Council's relevant By-laws that your account is in arrears at the statement due date.Please note that the electricity and /or water services to your property may be disconnected / restricted without further notice, unless payment of the outstanding amount is made within fourteen (14) days from the date of this notice.\n\nClick on the link below to make payment:\nPayment link:\nhttps://tinyurl.com/4d9vkedb");
    }
}

public class Content
{
    public Content(string text)
    {
        this.text = text;
    }
    public string text { get; set; }
}

public class GrapeVine
{
    public GrapeVine(string recipientNumber, string message)
    {
        this.recipientNumber = recipientNumber;
        this.message = "PRE-TERMINATION NOTICE is hereby given in terms of the Council's relevant By-laws that your account is in arrears at the statement due date.Please note that the electricity and /or water services to your property may be disconnected / restricted without further notice, unless payment of the outstanding amount is made within fourteen (14) days from the date of this notice.\n\nClick on the link below to make payment:\nPayment link:\nhttps://tinyurl.com/4d9vkedb";
    }
    public string recipientNumber { get; set; }
    public string message { get; set; }
}