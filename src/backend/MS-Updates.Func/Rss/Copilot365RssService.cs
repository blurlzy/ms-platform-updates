using System.Globalization;
using System.Xml.Linq;

namespace MS_Updates.Func.Rss
{
     public sealed class Copilot365RssService
     {

          private const string RssFeedUrl = "https://techcommunity.microsoft.com/t5/s/gxcuf89792/rss/board?board.id=Microsoft365CopilotBlog";
          private static readonly XNamespace DC = "http://purl.org/dc/elements/1.1/";

          private readonly HttpClient _httpClient;

          public Copilot365RssService(HttpClient httpClient)
          {
               _httpClient = httpClient;
          }

          public async Task<IReadOnlyList<Copilot365Update>> GetUpdatesAsync(CancellationToken cancellationToken = default)
          {
               using var response = await _httpClient.GetAsync(RssFeedUrl, cancellationToken);
               response.EnsureSuccessStatusCode();

               await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
               var document = await XDocument.LoadAsync(
                   stream,
                   LoadOptions.PreserveWhitespace,
                   cancellationToken);

               var items = document.Root?
                  .Element("channel")?
                   .Elements("item")
                   .Select(MapItem)
                   .ToArray();

               return items ?? Array.Empty<Copilot365Update>();
          }

          private static Copilot365Update MapItem(XElement item)
          {
               var descriptionElement = item.Element("description");

               return new Copilot365Update(
                   Id: item.Element("guid")?.Value
                       ?? throw new InvalidOperationException("RSS item has no GUID."),
                   Title: item.Element("title")?.Value ?? string.Empty,
                   Link: item.Element("link")?.Value ?? string.Empty,
                   Description: descriptionElement?.Value.Trim() ?? string.Empty,
                   Creator: item.Element(DC + "creator")?.Value,
                   PublishedAt: ParseDate(item.Element("pubDate")?.Value),
                   UpdatedAt: ParseDate(item.Element(DC + "date")?.Value));
          }

          private static DateTimeOffset? ParseDate(string? value) =>
              DateTimeOffset.TryParse(
                  value,
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.AssumeUniversal,
                  out var date)
                  ? date
                  : null;
     }
}
