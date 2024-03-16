namespace ThyreSoft.VoiceCommand.Domain.IServices;

public interface ISpeechRecognitionService
{
    Task StartRecognitionAsync();
    Task StopRecognitionAsync();
}