using ThyreSoft.VoiceCommand.Domain.IServices;

namespace ThyrSoft.VoiceCommand.Application.Services;

public class SentenceHypothesizer : ISentenceHypothesizer
{
    private readonly TextSimilarityService _textSimilarityService = new();


    public bool IsSimilarText(string newText, string previousText, string[] definedSentences, double similarityThreshold)
    {
        if (string.IsNullOrWhiteSpace(previousText))
            return false;

        foreach (string sentence in definedSentences)
        {
            if (_textSimilarityService.ComputeSimilarity(newText, sentence) >= similarityThreshold)
                return true;
        }

        return false;
    }

    public string HypothesizeSentence(string newText, string[] definedSentences, double similarityThreshold)
    {
        foreach (string sentence in definedSentences)
        {   
            if (_textSimilarityService.ComputeSimilarity(newText, sentence) >= similarityThreshold)
                return sentence;
        }

        return null;
    }
}