using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using File = Microsoft.SharePoint.Client.File;

namespace NW.MsTickets.UploadFileError
{
    public class SPODataAccess
    {
        public ClientContext TryResolveClientContext(Uri requestUri)
        {
            ClientContext context = null;
            var baseUrl = requestUri.GetLeftPart(UriPartial.Authority);
            for (int i = requestUri.Segments.Length; i >= 0; i--)
            {
                var path = string.Join(string.Empty, requestUri.Segments.Take(i));
                string url = string.Format("{0}{1}", baseUrl, path);
                try
                {
                    context = SPOAuth.GetAppOnlyClientContext(url, false);
                    context.Load(context.Site, t => t.SensitivityLabelInfo);
                    context.Load(context.Web, t => t.AllProperties);
                    context.ExecuteQuery();

                    return context;
                }
                catch (Exception ex)
                {
                    // Do nothing since it may not be valid web url
                }
            }
            throw new Exception("Could not form a client context with url: ");
        }

        public Int32 UploadFileByPath(ClientContext ctx, string destinationFileUrl
                                                , string filePath)
        {
            Web web = ctx.Web;
            Microsoft.SharePoint.Client.TimeZone webTimeZone = ctx.Web.RegionalSettings.TimeZone;
            ctx.Load(webTimeZone);
            ctx.ExecuteQuery();
            string filerelativeUrl = StringHelper.GetFileRelativeUrl(ctx.Url, destinationFileUrl);
            //file = ctx.Web.GetFileByServerRelativeUrl(filerelativeUrl);
            File file = MIPHelper.GetFileByResourcePath(ctx, filerelativeUrl, true);

            ctx.Load(file, f => f.ListItemAllFields, f => f.ListItemAllFields.ParentList);// ListId Does not work in on prem
            ctx.ExecuteQuery();
            List targetLib = file.ListItemAllFields.ParentList;
            ctx.Load(targetLib, l => l.RootFolder);
            // Get the information about the folder that will hold the file
            ctx.Load(targetLib.RootFolder, f => f.ServerRelativeUrl);
            ctx.ExecuteQuery();

            Guid uploadId = Guid.NewGuid();

            int blockSize = AppConfigInfo.FileChunkSizeInMB * 1024 * 1024;
            long fileSize = new FileInfo(filePath).Length;
            // File object.

            if (fileSize <= 2 * 1024 * 1024 - 100) // To be less than 2 MB
            {
                if (ctx.HasPendingRequest)
                {
                    ctx.ExecuteQuery();
                }
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    FileCreationInformation fileInfo = new FileCreationInformation();
                    fileInfo.ContentStream = fs;
                    fileInfo.Url = destinationFileUrl;
                    fileInfo.Overwrite = true;
                    var uploadedFile = targetLib.RootFolder.Files.Add(fileInfo);
                    ctx.ExecuteQuery();
                }
            }
            else
            {
                // Use large file upload approach
                ClientResult<long> bytesUploaded = null;

                FileStream fs = null;
                Microsoft.SharePoint.Client.File uploadFile = null;
                try
                {
                    fs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] buffer = new byte[blockSize];
                        long fileoffset = 0;
                        long totalBytesRead = 0;
                        int bytesRead;
                        bool first = true;

                        // Read data from filesystem in blocks 
                        while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytesRead = totalBytesRead + bytesRead;

                            if (first)
                            {
                                using (MemoryStream contentStream = new MemoryStream())
                                {
                                    // Add an empty file.
                                    FileCreationInformation fileInfo = new FileCreationInformation();
                                    fileInfo.ContentStream = contentStream;
                                    fileInfo.Url = destinationFileUrl;
                                    fileInfo.Overwrite = true;
                                    uploadFile = targetLib.RootFolder.Files.Add(fileInfo);

                                    // Start upload by uploading the first slice. 
                                    using (MemoryStream s = new MemoryStream(buffer))
                                    {
                                        // Call the start upload method on the first slice
                                        bytesUploaded = uploadFile.StartUpload(uploadId, s);
                                        ctx.ExecuteQuery();
                                        // fileoffset is the pointer where the next slice will be added
                                        fileoffset = bytesUploaded.Value;
                                    }

                                    // we can only start the upload once
                                    first = false;
                                }
                            }
                            else
                            {
                                // Get a reference to our file
                                //uploadFile = ctx.Web.GetFileByServerRelativeUrl(filerelativeUrl);
                                uploadFile = MIPHelper.GetFileByResourcePath(ctx, filerelativeUrl, true);

                                using (MemoryStream s = new MemoryStream(buffer))
                                {
                                    // Continue sliced upload
                                    bytesUploaded = uploadFile.ContinueUpload(uploadId, fileoffset, s);
                                    ctx.ExecuteQuery();
                                    // update fileoffset for the next slice
                                    fileoffset = bytesUploaded.Value;
                                }
                            }
                            if (totalBytesRead == fileSize)
                            {
                                // Get a reference to our file
                                uploadFile = MIPHelper.GetFileByResourcePath(ctx, filerelativeUrl, true);
                                // We've reached the end of the file
                                using (MemoryStream s = new MemoryStream(buffer, 0, bytesRead))
                                {
                                    // End sliced upload by calling FinishUpload
                                    uploadFile = uploadFile.FinishUpload(uploadId, fileoffset, s);
                                    ctx.ExecuteQuery();
                                }
                            }

                        }
                    }
                }
                catch (Exception uploadEx)
                {
                    ExceptionDispatchInfo.Capture(uploadEx).Throw();
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                }
            }

            // Get the uploaded file's latest version
            //file = ctx.Web.GetFileByServerRelativeUrl(filerelativeUrl);
            file = MIPHelper.GetFileByResourcePath(ctx, filerelativeUrl, true);
            ctx.Load(file, f => f.UIVersion);
            ctx.ExecuteQuery();
            return file.UIVersion;

        }

    }
}
