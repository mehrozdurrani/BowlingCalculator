using System;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using FrameClass;
using System.Text.Json;
// hello
namespace RollsFunctions;
public class Rolls : IRolls
{
    // LocalStack SQS queue URL
    private const string QueueUrl = "http://localhost:4566/000000000000/RollsQueue"; 
    private readonly AmazonSQSClient sqsClient;

    public Rolls()
    {
        sqsClient = new AmazonSQSClient(new AmazonSQSConfig
        {
            // LocalStack SQS service URL
            ServiceURL = "http://localhost:4566" 
        });
    }

    // Publishing Frame of Rolls to 'RollsQueue"
    public void SendRolls(Frames[] framesOfRolls)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = QueueUrl,

            // Serializing 'Frames of Rolls' of type 'Frames' to JSON
            MessageBody = JsonSerializer.Serialize(framesOfRolls)
        };

        var sendMessageResponse = sqsClient.SendMessageAsync(sendMessageRequest).GetAwaiter().GetResult();

        if (sendMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine("Roll sent successfully!");
        }
        else
        {
            Console.WriteLine("Failed to send roll.");
        }
    }

}

