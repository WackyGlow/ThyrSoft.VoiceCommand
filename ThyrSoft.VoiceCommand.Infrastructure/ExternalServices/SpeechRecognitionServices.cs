using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using ThyreSoft.VoiceCommand.Domain.IServices;
using System;

namespace ThyrSoft.VoiceCommand.Infrastructure.ExternalServices
{
    public class SpeechRecognitionServices : ISpeechRecognitionService
    {
        private readonly string[] definedSentences;
        private readonly double similarityThreshold;
        private SpeechRecognizer speechRecognizer;
        private System.Timers.Timer silenceTimer;

        private string previousText = ""; // To store recognized text

        public SpeechRecognitionServices(string[] definedSentences, double similarityThreshold)
        {
            this.definedSentences = definedSentences;
            this.similarityThreshold = similarityThreshold;

            // Initialize silenceTimer
            silenceTimer = new System.Timers.Timer(1000); // 1 second silence threshold
            silenceTimer.Elapsed += SilenceTimer_Elapsed;
        }

        private void SilenceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            silenceTimer.Stop();
            if (!string.IsNullOrWhiteSpace(previousText))
            {
                Console.WriteLine($"Silent: {previousText}");
                previousText = "";
            }
        }

        public async Task StartRecognitionAsync()
        {
            var speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
            var speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechRecognitionLanguage = "da-DK";

            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            speechRecognizer.Recognizing += SpeechRecognizer_Recognizing;
            speechRecognizer.Recognized += SpeechRecognizer_Recognized;
            speechRecognizer.Canceled += SpeechRecognizer_Canceled;

            await speechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
        }

        private void SpeechRecognizer_Recognizing(object sender, SpeechRecognitionEventArgs e)
        {
            silenceTimer.Stop();
            silenceTimer.Start();
        }

        private void SpeechRecognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
        {
            // Handle recognized event
            previousText = e.Result.Text;
        }

        private void SpeechRecognizer_Canceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            // Handle canceled event
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
