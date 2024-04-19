using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;

namespace REST_Testing
{
    public class ZipCode
    {
        public string Body { get; set; }

        public ZipCode(List<string> zipCodeList)
        {
            var withQuotes = zipCodeList.Select(z => z.AddDoubleQuote()).ToList();
            var asString = string.Join(", ", withQuotes);
            Body = "[" + asString + "]";
        }

        static public List<string> ZipCodesToList(string zipCodes)
        {
            var zipCodesList = zipCodes.Trim(['[', ']']).Replace("\"", "").Split(new char[] { ',' }).ToList();
            return zipCodesList;
        }
    }
}
