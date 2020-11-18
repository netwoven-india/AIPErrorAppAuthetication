using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.MsTickets.UploadFileError
{
    public class SPOAuth
    {
        #region Public Methods
        public static ClientContext GetAppOnlyClientContext(String siteUrl, bool isFullPermissionApp = true)
        {
            AuthenticationManager authManager = new AuthenticationManager();
            ClientContext context = null;

            if (isFullPermissionApp)
            {
                context = authManager.GetAzureADAppOnlyAuthenticatedContext(
                    siteUrl,
                    AppConfigInfo.ApplicationId,
                    AppConfigInfo.TenantId,
                    AppConfigInfo.AppCertificateFromStore);
            }
            else
            {
                context = authManager.GetAppOnlyAuthenticatedContext(
                    siteUrl,
                    AppConfigInfo.ApplicationId,
                    AppConfigInfo.ApplicationSecret);
            }

            return (context);
        }
        #endregion
    }
}
