using System;
using System.Collections.Generic;
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

        string[] definedSentences = { "hej", "hvordan går det", "jeg er sulten" }; // Defined sentences
        double similarityThreshold = 0.8; // Adjust this threshold as needed

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
            silenceTimer.Stop();
            silenceTimer.Start();
        };

        speechRecognizer.Recognized += (s, e) =>
        {
            string newText = e.Result.Text.Trim().ToLower(); // Convert recognized text to lowercase
            if (e.Result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrWhiteSpace(newText) && !IsSimilarText(newText, previousText, definedSentences, similarityThreshold))
            {
                previousText = newText;
                // Hypothesize the sentence if it's within an acceptable margin of similarity
                string hypothesizedSentence = HypothesizeSentence(newText, definedSentences, similarityThreshold);
                if (hypothesizedSentence != null)
                {
                    Console.WriteLine($"Hypothesized: {hypothesizedSentence}");
                    // Execute code specific to the hypothesized sentence
                }
                else
                {
                    Console.WriteLine($"Unrecognized phrase: {newText}");
                }
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

    static bool IsSimilarText(string newText, string previousText, string[] definedSentences, double similarityThreshold)
    {
        if (string.IsNullOrWhiteSpace(previousText))
            return false;

        foreach (string sentence in definedSentences)
        {
            if (TextSimilarity(newText, sentence) >= similarityThreshold)
                return true;
        }

        return false;
    }

    static string HypothesizeSentence(string newText, string[] definedSentences, double similarityThreshold)
    {
        foreach (string sentence in definedSentences)
        {
            if (TextSimilarity(newText, sentence) >= similarityThreshold)
                return sentence;
        }

        return null;
    }

    static double TextSimilarity(string text1, string text2)
    {
        // Implement text similarity comparison algorithm (e.g., Levenshtein Distance, Cosine Similarity, etc.)
        // For simplicity, let's use Levenshtein Distance here
        int maxLength = Math.Max(text1.Length, text2.Length);
        int distance = ComputeLevenshteinDistance(text1, text2);
        return 1 - (double)distance / maxLength;
    }

    static int ComputeLevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        // Step 1
        if (n == 0)
            return m;

        if (m == 0)
            return n;

        // Step 2
        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 0; j <= m; d[0, j] = j++) ;

        // Step 3
        for (int i = 1; i <= n; i++)
        {
            //Step 4
            for (int j = 1; j <= m; j++)
            {
                // Step 5
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                // Step 6
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        // Step 7
        return d[n, m];
    }
}
