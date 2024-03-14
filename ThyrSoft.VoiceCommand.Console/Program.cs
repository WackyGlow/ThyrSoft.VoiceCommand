using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;


class Program
{
    async static Task Main(string[] args)
    {   
        
        var speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
        var speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");
        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
        speechConfig.SpeechRecognitionLanguage = "da-DK";

        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

        string previousText = "";
        System.Timers.Timer silenceTimer = new System.Timers.Timer(1000); // 1 second silence threshold

        silenceTimer.Elapsed += (sender, e) =>
        {
            silenceTimer.Stop();
            if (!string.IsNullOrWhiteSpace(previousText))
            {
                Console.WriteLine($"Silent: {previousText}");
                previousText = "";
            }
        };

        speechRecognizer.Recognizing += (s, e) =>
        {
            string newText = e.Result.Text.Trim();
            if (!string.IsNullOrWhiteSpace(newText) && !IsSimilarText(newText, previousText))
            {
                previousText = newText;
            }
            silenceTimer.Stop();
            silenceTimer.Start();
        };

        speechRecognizer.Recognized += (s, e) =>
        {
            string newText = e.Result.Text.Trim();
            if (e.Result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrWhiteSpace(newText) && !IsSimilarText(newText, previousText))
            {
                previousText = newText;
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            }
            silenceTimer.Stop();
            silenceTimer.Start();
        };

        speechRecognizer.Canceled += (s, e) =>
        {
            Console.WriteLine($"CANCELED: Reason={e.Reason}");

            if (e.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
            }
        };

        await speechRecognizer.StartContinuousRecognitionAsync();

        Console.WriteLine("Listening... Press any key to stop.");

        while (true)
        {
            if (Console.KeyAvailable)
            {
                break;
            }
            await Task.Delay(100);
        }

        await speechRecognizer.StopContinuousRecognitionAsync();
    }

    static bool IsSimilarText(string newText, string previousText)
    {
        if (string.IsNullOrWhiteSpace(previousText))
            return false;

        return string.Equals(newText, previousText, StringComparison.OrdinalIgnoreCase);
    }
}
