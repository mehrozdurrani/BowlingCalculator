
using System;
using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using FrameClass;

using static System.Formats.Asn1.AsnWriter;

namespace CalculatorFunctions;

public class Calculator : ICalculator
{
    const int TOTAL_FRAMES = 10;

    private const string RollQueueUrl = "http://localhost:4566/000000000000/RollsQueue";
    private const string ScoreQueueUrl = "http://localhost:4566/000000000000/ScoreQueue";

    private readonly AmazonSQSClient sqsClient;
    private string ScoreString { get; set; }

    public Calculator()
    {
        sqsClient = new AmazonSQSClient(new AmazonSQSConfig
        {
            ServiceURL = "http://localhost:4566" // Replace with your LocalStack SQS service URL
        });
    }

    public void StartListening()
    {
        while (true)
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = RollQueueUrl,
                MaxNumberOfMessages = 1
            };

            var receiveMessageResponse = sqsClient.ReceiveMessageAsync(receiveMessageRequest).GetAwaiter().GetResult();

            if (receiveMessageResponse.Messages.Any())
            {
                foreach (var message in receiveMessageResponse.Messages)
                {
                    var frames = ProcessRoll(message.Body);

                    // Score Calculation
                    CalculateScore(frames);
                    // Publishing Result to the Score Queue
                    PublishResult(ScoreString);

                    sqsClient.DeleteMessageAsync(new DeleteMessageRequest(RollQueueUrl, message.ReceiptHandle)).GetAwaiter().GetResult();
                }
            }
            else
            {
                Console.WriteLine("No messages available in the queue.");
            }
        }
    }

    private Frames[] ProcessRoll(string roll)
    {
        // TODO Process Rolls from Here, will call calculate score from here
        var frames = JsonSerializer.Deserialize<Frames[]>(roll);

        return frames;
    }
    private void PublishResult(string result)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = ScoreQueueUrl,
            MessageBody = result
        };

        var sendMessageResponse = sqsClient.SendMessageAsync(sendMessageRequest).GetAwaiter().GetResult();

        if (sendMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine("Result published successfully!");
        }
        else
        {
            Console.WriteLine("Failed to publish result.");
        }
    }


    private bool IsFrameStrike(Frames frame)
    {
        if (frame.roll1 == 10)
        {
            return true;
        }
        return false;
    }
    private bool IsFrameSpare(Frames frame)
    {
        if (frame.roll1 + frame.roll2 == 10)
        {
            return true;
        }
        return false;
    }

    public int CalculateScore(Frames[] frames)
    {

        StringBuilder output = new StringBuilder();
        //TODO Bowling Score Calculation Code

        int score = 0;
        for (int i = 0; i < frames.Length; i++)
        {
            // Check Strike
            if (IsFrameStrike(frames[i]))
            {
                score += 10;

                // Ensure there is a next frame
                if (i + 1 < frames.Length)
                {
                    score += frames[i + 1].roll1;

                    // Check if the next frame is also a strike
                    if (IsFrameStrike(frames[i + 1]))
                    {
                        // Ensure there is a frame after the next frame
                        if (i + 2 < frames.Length)
                        {
                            score += frames[i + 2].roll1;
                        }
                        else
                        {
                            // All frames are strikes, add 10 as bonus
                            score += 10;
                        }
                    }
                    else
                    {
                        score += frames[i + 1].roll2;
                    }
                }
                output.Append("[X=>" + (score) + "]");
            }
            // Check Spare
            else if (IsFrameSpare(frames[i]))
            {
                score += 10;

                // Ensure there is a next frame
                if (i + 1 < frames.Length)
                {
                    score += frames[i + 1].roll1;
                }

                output.Append("[" + frames[i].roll1 + "|" + frames[i].roll2 + "=>" + score + "]");
            }
            else
            {
                score += frames[i].roll1 + frames[i].roll2;
                output.Append("[" + frames[i].roll1 + "|" + frames[i].roll2 + "=>" + score + "]");
            }

        }

        // Handling last frame
        if (frames.Length >= TOTAL_FRAMES)
        {
            Frames lastFrame = frames[frames.Length - 1];

            // Check Strike
            if (IsFrameStrike(lastFrame))
            {
                score += lastFrame.roll2 + lastFrame.roll3;
                output.Append("[X" + "|" + (lastFrame.roll2 == 10 ? "X" : lastFrame.roll2) + "|" + (lastFrame.roll3 == 10 ? "X" : lastFrame.roll3) + "=>" + score + "]"); 
            }
            // Check Spare
            else if (IsFrameSpare(lastFrame))
            {
                score += lastFrame.roll3;
                output.Append("[" + lastFrame.roll1 + "|" + lastFrame.roll2 + "|" + lastFrame.roll3 + "=>" + score + "]");
            }
        }
        output.Append("=>" + score);

        ScoreString = output.ToString();
        return score;
    }

}

