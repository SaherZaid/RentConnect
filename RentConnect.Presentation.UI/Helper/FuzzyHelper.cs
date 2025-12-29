namespace RentConnect.Presentation.UI.Helper;

public static class FuzzyHelper
{
    public static int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s)) return t.Length;
        if (string.IsNullOrEmpty(t)) return s.Length;

        var dp = new int[s.Length + 1, t.Length + 1];

        for (int i = 0; i <= s.Length; i++)
            dp[i, 0] = i;
        for (int j = 0; j <= t.Length; j++)
            dp[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                dp[i, j] = new[] {
                    dp[i - 1, j] + 1,    // deletion
                    dp[i, j - 1] + 1,    // insertion
                    dp[i - 1, j - 1] + cost // substitution
                }.Min();
            }
        }

        return dp[s.Length, t.Length];
    }

    public static bool IsFuzzyMatch(string source, string query, int maxDistance = 2)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(query))
            return false;

        return LevenshteinDistance(source.ToLowerInvariant(), query.ToLowerInvariant()) <= maxDistance;
    }
}