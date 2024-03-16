using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using ThyreSoft.VoiceCommand.Domain.IServices;

namespace ThyrSoft.VoiceCommand.Infrastructure.ExternalServices;

public class SpeechRecognitionServices : ISpeechRecognitionService
{
    private readonly string[] definedSentences;
    private readonly double similarityThreshold;
    private SpeechRecognizer speechRecognizer;

    public SpeechRecognitionServices(string[] definedSentences, double similarityThreshold)    
    {
        this.definedSentences = definedSentences;
        this.similarityThreshold = similarityThreshold;
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
        // Handle recognizing event
    }

    private void SpeechRecognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
    {
        // Handle recognized event
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