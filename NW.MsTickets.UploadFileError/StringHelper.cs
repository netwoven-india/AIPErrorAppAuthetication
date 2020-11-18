
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NW.MsTickets.UploadFileError
{
    public class StringHelper
    {
        #region Public methods
        public static string GetEncodedString(string sourceString)
        {
            if (!String.IsNullOrEmpty(sourceString))
            {
                return HttpUtility.UrlEncode(sourceString).Replace(".", "%2e").Replace("_", "%5f");
            }
            else
            {
                return sourceString;
            }
        }
        public static string GetDecodedString(string sourceString)
        {
            if (!String.IsNullOrEmpty(sourceString))
            {
                //return HttpUtility.UrlDecode(sourceString);
                return Uri.UnescapeDataString(sourceString);
            }
            else
            {
                return sourceString;
            }
        }
        public static string WrapContent(string content)
        {
            return ("\"" + content.Replace("\"", "\"\"") + "\"");
        }
        public static string GetFileRelativeUrl(string siteUrl, string fileUrl)
        {
            siteUrl = siteUrl.TrimEnd('/');
            Uri uriSiteAddress = new Uri(siteUrl);
            string hostPart = uriSiteAddress.GetLeftPart(UriPartial.Authority);
            string fileRelativeUrl = fileUrl.Replace(hostPart, "");
            return fileRelativeUrl;
        }

        public static bool CheckEquality(string str, string otherStr)
        {
            return str.Equals(otherStr, StringComparison.OrdinalIgnoreCase);
        }

        public static List<char> ContainsIllegalCharacters(string input, char[] notAllowedCharacters)
        {
            List<char> illChars = new List<char>();
            foreach (var character in notAllowedCharacters)
            {
                if (input.IndexOf(character) != -1)
                {
                    illChars.Add(character);
                }
            }
            return illChars;
        }
        #endregion
    }
}
