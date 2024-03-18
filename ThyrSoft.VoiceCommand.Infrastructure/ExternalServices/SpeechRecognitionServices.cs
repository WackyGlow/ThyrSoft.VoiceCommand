using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using ThyreSoft.VoiceCommand.Domain.IServices;
using ThyrSoft.VoiceCommand.Application.Services;

namespace ThyrSoft.VoiceCommand.Infrastructure.ExternalServices
{
    public class SpeechRecognitionServices : ISpeechRecognitionService
    {
        private readonly string[] definedSentences;
        private readonly double similarityThreshold;
        private SpeechRecognizer speechRecognizer;
        private System.Timers.Timer silenceTimer;
        private readonly SentenceHypothesizer sentenceHypothesizer;

        private string previousText = ""; // To store recognized text

        public SpeechRecognitionServices(string[] definedSentences, double similarityThreshold)
        {
            this.definedSentences = definedSentences;
            this.similarityThreshold = similarityThreshold;

            // Initialize sentenceHypothesizer


            // Initialize silenceTimer
            silenceTimer = new System.Timers.Timer(1000); // 1 second silence threshold
            silenceTimer.Elapsed += SilenceTimer_Elapsed;
        }

        /// <summary>
        /// Event handler triggered when the silence timer elapses.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments containing information about the elapsed event.</param>
        private void SilenceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            silenceTimer.Stop();
            if (!string.IsNullOrWhiteSpace(previousText))
            {
                Console.WriteLine($"Silent: {previousText}");
                previousText = "";
            }
        }

        /// <summary>
        /// Starts continuous speech recognition asynchronously.
        /// </summary>
        /// This method initializes the speech recognizer using the provided speech key and region from environment variables.
        /// It configures the recognizer for Danish (da-DK) speech recognition language.
        /// It sets up event handlers for Recognizing, Recognized, and Canceled events.
        /// Finally, it starts continuous speech recognition asynchronously using the initialized speech recognizer.
        public async Task StartRecognitionAsync()
        {
            // Retrieve the speech key and region from environment variables
            var speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
            var speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

            // Initialize the speech configuration using the subscription key and region
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);

            // Set the speech recognition language to Danish (da-DK)
            speechConfig.SpeechRecognitionLanguage = "da-DK";

            // Create the audio configuration using default microphone input
            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

            // Initialize the speech recognizer with the configured speech and audio configurations
            speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            // Register event handlers for recognizing, recognized, and canceled events
            speechRecognizer.Recognizing += SpeechRecognizer_Recognizing;
            speechRecognizer.Recognized += SpeechRecognizer_Recognized;
            speechRecognizer.Canceled += SpeechRecognizer_Canceled;

            // Start continuous speech recognition asynchronously
            await speechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
        }

        private void SpeechRecognizer_Recognizing(object sender, SpeechRecognitionEventArgs e)
        {
            silenceTimer.Stop();
            silenceTimer.Start();
        }

        private void SpeechRecognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
        {
            string newText = e.Result.Text.Trim().ToLower(); // Convert recognized text to lowercase
            if (e.Result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrWhiteSpace(newText) && !sentenceHypothesizer.IsSimilarText(newText, previousText, definedSentences, similarityThreshold))
            {
                previousText = newText;
                // Hypothesize the sentence if it's within an acceptable margin of similarity
                string hypothesizedSentence = sentenceHypothesizer.HypothesizeSentence(newText, definedSentences, similarityThreshold);
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
        }

        private void SpeechRecognizer_Canceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            Console.WriteLine($"CANCELED: Reason={e.Reason}");

            if (e.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
            }
        }

        public async Task StopRecognitionAsync()
        {
            if (speechRecognizer != null)
            {
                await speechRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                speechRecognizer.Recognizing -= SpeechRecognizer_Recognizing;
                speechRecognizer.Recognized -= SpeechRecognizer_Recognized;
                speechRecognizer.Canceled -= SpeechRecognizer_Canceled;
                speechRecognizer.Dispose();
                speechRecognizer = null;
            }
        }
    }
}
