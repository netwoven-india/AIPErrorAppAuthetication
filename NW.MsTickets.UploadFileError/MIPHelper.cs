using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.MsTickets.UploadFileError
{
    public class MIPHelper
    {
        public static Microsoft.SharePoint.Client.File GetFileByResourcePath(ClientContext ctx, string filePath, bool isRelativeUrl)
        {
            // Get the relative url
            string fileRelativeUrl = string.Empty;
            if (isRelativeUrl)
            {
                fileRelativeUrl = filePath;
            }
            else
            {
                fileRelativeUrl = StringHelper.GetFileRelativeUrl(ctx.Url.TrimEnd('/'), filePath);
            }
            // Decode the url
            fileRelativeUrl = StringHelper.GetDecodedString(fileRelativeUrl);
            // Get the resource path
            ResourcePath filePathDecoded = ResourcePath.FromDecodedUrl(fileRelativeUrl);
            return (ctx.Web.GetFileByServerRelativePath(filePathDecoded));
        }

        public static Microsoft.SharePoint.Client.File GetFileByResourcePath(ClientRuntimeContext ctx, Web web, string filePath, bool isRelativeUrl)
        {
            // Get the relative url
            string fileRelativeUrl = string.Empty;
            if (isRelativeUrl)
            {
                fileRelativeUrl = filePath;
            }
            else
            {
                fileRelativeUrl = StringHelper.GetFileRelativeUrl(ctx.Url.TrimEnd('/'), filePath);
            }
            // Decode the url
            fileRelativeUrl = StringHelper.GetDecodedString(fileRelativeUrl);
            // Get the resource path
            ResourcePath filePathDecoded = ResourcePath.FromDecodedUrl(fileRelativeUrl);
            return (web.GetFileByServerRelativePath(filePathDecoded));
        }
    }
}
