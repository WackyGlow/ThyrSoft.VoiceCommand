namespace ThyreSoft.VoiceCommand.Domain.IServices;

public interface ITextSimilarityService
{
    double ComputeSimilarity(string text1, string text2);
    int ComputeLevenshteinDistance(string s, string t);
}