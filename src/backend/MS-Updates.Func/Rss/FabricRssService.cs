using System.Globalization;
using System.Xml.Linq;

namespace MS_Updates.Func.Rss
{
     public sealed class FabricRssService
     {
          private const string RssFeedUrl = "https://community.fabric.microsoft.com/oxcrx34285/rss/board?board.id=fbc_fabricupdatesblogs";
          private static readonly XNamespace DC = "http://purl.org/dc/elements/1.1/";

          private readonly HttpClient _httpClient;

          // ctor
          public FabricRssService(HttpClient httpClient)
          {
               _httpClient = httpClient;
          }

          public async Task<IReadOnlyList<FabricUpdate>> GetUpdatesAsync(CancellationToken cancellationToken = default)
          {
               using var response = await _httpClient.GetAsync(RssFeedUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
               response.EnsureSuccessStatusCode();

               await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
               var document = await XDocument.LoadAsync(
                   stream,
                   LoadOptions.PreserveWhitespace,
                   cancellationToken);

               return document.Root?
                   .Element("channel")?
                   .Elements("item")
                   .Select(MapItem)
                   .ToArray()
                   ?? Array.Empty<FabricUpdate>(); ;
          }

          private static FabricUpdate MapItem(XElement item)
          {
               var descriptionElement = item.Element("description");

               return new FabricUpdate(
                   Id: item.Element("guid")?.Value
                       ?? throw new InvalidOperationException("RSS item has no GUID."),
                   Title: item.Element("title")?.Value ?? string.Empty,
                   Link: item.Element("link")?.Value ?? string.Empty,
                   DescriptionHtml: descriptionElement?.Value.Trim() ?? string.Empty,
                   Creator: item.Element(DC + "creator")?.Value,
                   PublishedAt: ParseDate(item.Element("pubDate")?.Value),
                   CreatedAt: ParseDate(item.Element(DC + "date")?.Value));
          }

          //private static string GetInnerXml(XElement? element) =>
          //    element is null
          //        ? string.Empty
          //        : string.Concat(element.Nodes().Select(node => node.ToString())).Trim();

          private static DateTimeOffset? ParseDate(string? value) =>
              DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date)
                  ? date
                  : null;

          //private static bool? ParseBoolean(string? value) =>
          //    bool.TryParse(value, out var result) ? result : null;
     }
}
