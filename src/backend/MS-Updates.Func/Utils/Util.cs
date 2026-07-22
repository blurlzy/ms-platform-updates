
using System.Text.RegularExpressions;

namespace MS_Updates.Func.Utils
{
     public static class Util
     {
          public static string ExtractFirstParagraph(string? html)
          {
               if (string.IsNullOrEmpty(html))
                    return string.Empty;

                var match = Regex.Match(html, @"<(p|div)[^>]*>.*?</\1>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
               return match.Success ? match.Value : string.Empty;
          }

          // extracts the first sentence from a given text
          public static string ExtractFirstSentence(string text)
          {
               if (string.IsNullOrWhiteSpace(text))
                    return string.Empty;
               // Use a regex to find the first sentence
               var match = Regex.Match(text, @"(.*?[\.\!\?])\s", RegexOptions.Singleline);
               return match.Success ? match.Groups[1].Value : text; // Return the whole text if no sentence-ending punctuation is found
          }
     }
}
