using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.MsTickets.UploadFileError
{
    class Program
    {
        static void Main(string[] args)
        {
            string spFilePath = "<site url>/Shared%20Documents/Devjani/ConfidentialDoc1.docx";
            string localFilePath = "D:\\ConfidentialDoc1.docx";
            SPODataAccess dataAccess = new SPODataAccess();
            ClientContext ctx = dataAccess.TryResolveClientContext(new Uri(spFilePath) );
            dataAccess.UploadFileByPath(ctx, spFilePath, localFilePath);
        }
    }
}
