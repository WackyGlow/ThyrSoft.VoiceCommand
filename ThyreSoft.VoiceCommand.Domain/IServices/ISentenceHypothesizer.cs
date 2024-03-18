namespace ThyreSoft.VoiceCommand.Domain.IServices;

public interface ISentenceHypothesizer
{
    bool IsSimilarText(string newText, string previousText, string[] definedSentences, double similarityThreshold);
    public string HypothesizeSentence(string newText, string[] definedSentences, double similarityThreshold);
}