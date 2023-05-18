
using System.Text;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using FrameClass;

namespace CalculatorFunctions;
//hello

public class Calculator : ICalculator
{
    const int TOTAL_FRAMES = 10;
    const int MAX_SCORE = 10;

    // Local Stack SQS Queues
    private const string RollQueueUrl = "http://localhost:4566/000000000000/RollsQueue";
    private const string ScoreQueueUrl = "http://localhost:4566/000000000000/ScoreQueue";

    private readonly AmazonSQSClient sqsClient;
    private string ScoreString { get; set; } = string.Empty;

    public Calculator()
    {
        sqsClient = new AmazonSQSClient(new AmazonSQSConfig
        {
            // LocalStack SQS service URL
            ServiceURL = "http://localhost:4566"
        });
    }

    // Listening to 'RollsQueue' for New Messages
    public void StartListening()
    {
        /*
         While(true) is not the most efficient approach but it serves the purpose.
        Alternative way is to handle events in AWS Lambda, so due to lack of
        time I have used this approach         
         */

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

    // Deserializing JSON to Frames
    private Frames[] ProcessRoll(string roll)
    {
        var frames = JsonSerializer.Deserialize<Frames[]>(roll);
        return frames;
    }

    // Publish Result to 'Score Queue'
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

    // Check Strike Function
    private bool IsFrameStrike(Frames frame)
    {
        if (frame.roll1 == MAX_SCORE)
        {
            return true;
        }
        return false;
    }

    // Check Spare Function
    private bool IsFrameSpare(Frames frame)
    {
        if (frame.roll1 + frame.roll2 == MAX_SCORE)
        {
            return true;
        }
        return false;
    }

    // Calculate Score of Frames
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
                score += MAX_SCORE;

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
                            score += MAX_SCORE;
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
                score += MAX_SCORE;

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
                output.Append("[X" + "|" + (lastFrame.roll2 == MAX_SCORE ? "X" : lastFrame.roll2) + "|" + (lastFrame.roll3 == MAX_SCORE ? "X" : lastFrame.roll3) + "=>" + score + "]");
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

