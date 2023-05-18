﻿using Amazon.SQS;
using Amazon.SQS.Model;

namespace PresenterFunctions;
public class Presenter : IPresenter
{
    private string ScoreString { get; set; }

    // Setting Local Stack Queue URL
    private const string ScoreQueueUrl = "http://localhost:4566/000000000000/ScoreQueue";
    private readonly AmazonSQSClient sqsClient;


    public Presenter()
    {
        // Setting up LocalStack SQS service
        sqsClient = new AmazonSQSClient(new AmazonSQSConfig
        {
            // LocalStack SQS Service address
            ServiceURL = "http://localhost:4566"
        });
    }

    public string GetScore()
    {
        return ScoreString;
    }

    public void StartPresenterService()
    {
        // Presenter Listner Service.

        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = ScoreQueueUrl,
            MaxNumberOfMessages = 1
        };

        var receiveMessageResponse = sqsClient.ReceiveMessageAsync(receiveMessageRequest).GetAwaiter().GetResult();

        if (receiveMessageResponse.Messages.Any())
        {
            var latestMessage = receiveMessageResponse.Messages.Last();
            var score = latestMessage.Body;
            Console.WriteLine($"Presenting score: {score}");
            ScoreString = score.ToString();

            sqsClient.DeleteMessageAsync(new DeleteMessageRequest(ScoreQueueUrl, latestMessage.ReceiptHandle)).GetAwaiter().GetResult();

           
        }
        else
        {
            ScoreString="No new score in the Queue.";
            
        }
    }
}
