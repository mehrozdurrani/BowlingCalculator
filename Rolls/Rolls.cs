using System;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using FrameClass;
using System.Text.Json;

namespace RollsFunctions;
public class Rolls : IRolls
{
    private const string QueueUrl = "http://localhost:4566/000000000000/RollsQueue"; // Replace with your LocalStack SQS queue URL
    private readonly AmazonSQSClient sqsClient;

    public Rolls()
    {
        sqsClient = new AmazonSQSClient(new AmazonSQSConfig
        {
            ServiceURL = "http://localhost:4566" // Replace with your LocalStack SQS service URL
        });
    }

    public void SendRolls(Frames[] framesOfRolls)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = QueueUrl,
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

