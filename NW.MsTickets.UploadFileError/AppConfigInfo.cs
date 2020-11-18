using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NW.MsTickets.UploadFileError
{
    public static class AppConfigInfo
    {

        public enum CurrentRequestType { SPOnline, SPOnpremise, Utility, Filer, DurableFunction, BulkLabelling }
        public static CurrentRequestType CurrentContainerRequestType { get; set; }
        public static string CurrentAssemlyPath { get; set; }
        public static string FunctionAppDirectory { get; set; }

        private static string GetAppSettingByConfig(Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!String.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            return string.Empty;
        }

        private static string _imageFileSizeValidationMessage;
        public static string ImageFileSizeValidationMessage
        {
            get
            {
                _imageFileSizeValidationMessage = GetConfigValue("OP_PostValidation_FileSizeExceededErrorMsg");
                return _imageFileSizeValidationMessage;
            }
        }

        private static string _msLoginApi;
        public static string MsLoginApi
        {
            get
            {
                _msLoginApi = GetConfigValue("msloginapi");
                return _msLoginApi;
            }
        }

        private static string _graphApi;
        public static string GraphApi
        {
            get
            {
                _graphApi = GetConfigValue("graphapi");
                return _graphApi;
            }
        }

        private static string _notificationType;
        public static string NotificationType
        {
            get
            {
                _notificationType = GetConfigValue("NotificationType");
                return _notificationType;
            }
        }

        private static string _mipAlertWebHook;
        public static string MIPAlertWebHook
        {
            get
            {
                _mipAlertWebHook = GetConfigValue("MIPAlertWebHook");
                return _mipAlertWebHook;
            }
        }

        private static string _mipAlertList;
        public static string MIPAlertList
        {
            get
            {
                _mipAlertList = GetConfigValue("MIPAlertList");
                return _mipAlertList;
            }
        }

        private static string _applicationId;
        public static string ApplicationId
        {
            get
            {
                _applicationId = GetConfigValue("ida:ClientId");
                return _applicationId;
            }
        }

        private static string _tenantId;
        public static string TenantId
        {
            get
            {
                _tenantId = GetConfigValue("ida:TenantId");
                return _tenantId;
            }
        }

        private static string _redirectUri;
        public static string RedirectUri
        {
            get
            {
                _redirectUri = GetConfigValue("ida:RedirectUri");
                return _redirectUri;
            }
        }

        private static string _applicationSecret;
        public static string ApplicationSecret
        {
            get
            {
                _applicationSecret = GetConfigValue("ida:ClientSecret");
                return _applicationSecret;
            }
        }

        private static string _certificateThumbprint;
        public static string CertificateThumbprint
        {
            get
            {
                _certificateThumbprint = GetConfigValue("ida:CertificateThumbprint");
                return _certificateThumbprint;
            }
        }

        public static Int16 FileChunkSizeInMB
        {
            get
            {
                return Convert.ToInt16(GetConfigValue("FileChunkSizeInMB"));
            }
        }

        private static string _azureTableStorage;
        public static string AzureTableStorage
        {
            get
            {
                if (_azureTableStorage == null)
                {
                    _azureTableStorage = GetConfigValue("AzureWebJobsStorage");

                }
                return _azureTableStorage;
            }
            set
            {
                _azureTableStorage = value;
            }
        }

        //private static string _consumerType;
        //public static string ConsumerType
        //{
        //    get
        //    {
        //        _consumerType = GetConfigValue("ConsumerType");
        //        return _consumerType;
        //    }
        //}

        private static string _requesterEmail;
        public static string RequesterEmail
        {
            get
            {
                _requesterEmail = GetConfigValue("RequesterEmail");
                return _requesterEmail;
            }
        }

        private static string _adminEmail;
        public static string AdminEmail
        {
            get
            {
                _adminEmail = GetConfigValue("AdminEmail");
                return _adminEmail;
            }
        }

        private static string _customMetaDataOwnerKey;
        public static string CustomMetaDataOwnerKey
        {
            get
            {
                _customMetaDataOwnerKey = GetConfigValue("CustomMetaDataOwnerKey");
                return _customMetaDataOwnerKey;
            }
        }

        private static string _allowedFileTypes;
        public static string AllowedFileTypes
        {
            get
            {
                _allowedFileTypes = GetConfigValue("AllowedFileTypes");
                return _allowedFileTypes;
            }
        }

        private static string _fileSizeLimitInKB;
        public static string FileSizeLimitInKB
        {
            get
            {
                _fileSizeLimitInKB = GetConfigValue("FileSizeLimitInKB");
                return _fileSizeLimitInKB;
            }
        }

        private static bool _notificationEnabled;
        public static bool NotificationEnabled
        {
            get
            {
                _notificationEnabled = GetConfigValue("NotificationEnabled") == "1";
                return _notificationEnabled;
            }
        }

        public static X509Certificate2 AppCertificateFromStore
        {
            get
            {
                X509Certificate2 appOnlyCertificate = null;

                X509Certificate2Collection certCollection = GetCertCollection(StoreName.My, StoreLocation.CurrentUser);

                if (certCollection.Count > 0)
                {
                    appOnlyCertificate = certCollection[0];
                }
                else
                {
                    certCollection = GetCertCollection(StoreName.My, StoreLocation.LocalMachine);
                    if (certCollection.Count > 0)
                    {
                        appOnlyCertificate = certCollection[0];
                    }
                }
                return (appOnlyCertificate);
            }
        }

        private static X509Certificate2Collection GetCertCollection(StoreName storeName, StoreLocation storeLocation)
        {
            X509Store certStore = null;
            certStore = new X509Store(storeName, storeLocation);
            certStore.Open(OpenFlags.ReadOnly);
            // Get the first cert with the thumbprint
            X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, CertificateThumbprint, false);
            certStore.Close();

            return certCollection;
        }

        public static string IssuerId
        {
            get
            {
                return GetConfigValue("IssuerId");
            }
        }

        public static string OnPremApplicationId
        {
            get
            {
                return GetConfigValue("ida:OnPremClientId");
            }
        }

        private static string _pExtensionRequired;
        public static string PExtensionRequired
        {
            get
            {
                _pExtensionRequired = GetConfigValue("PExtensionRequired");
                return _pExtensionRequired;
            }
        }

        private static string _restrictLabelWithoutProtection;
        public static string RestrictLabelWithoutProtection
        {
            get
            {
                _restrictLabelWithoutProtection = GetConfigValue("RestrictLabelWithoutProtection");
                return _restrictLabelWithoutProtection;
            }
        }

        private static string _restrictNoProtAllowRemoveProt;
        public static string RestrictNoProtAllowRemoveProt
        {
            get
            {
                _restrictNoProtAllowRemoveProt = GetConfigValue("RestrictNoProtAllowRemoveProt");
                return _restrictNoProtAllowRemoveProt;
            }
        }

        private static string _sensitivityLabelTableName;
        public static string SensitivityLabelTableName
        {
            get
            {
                _sensitivityLabelTableName = GetConfigValue("SensitivityLabelTableName");
                return _sensitivityLabelTableName;
            }
        }

        private static string _internalDomainName;
        public static string InternalDomainName
        {
            get
            {
                _internalDomainName = GetConfigValue("InternalDomainName");
                return _internalDomainName;
            }
        }

        private static string _nonOfficeFileTypes;
        public static string NonOfficeFileTypes
        {
            get
            {
                _nonOfficeFileTypes = GetConfigValue("NonOfficeFileTypes");
                return _nonOfficeFileTypes;
            }
        }

        private static string _pFileExtensionRequired;
        public static string PFileExtensionRequired
        {
            get
            {
                _pFileExtensionRequired = GetConfigValue("PFileExtensionRequired");
                return _pFileExtensionRequired;
            }
        }

        private static string _spoAccountConfigs;
        public static string SPOAccountConfigs
        {
            get
            {
                _spoAccountConfigs = GetConfigValue("SPOAccountConfigs");
                return _spoAccountConfigs;
            }
        }

        private static string _unknownPermanentExceptions;
        public static string UnknownPermanentExceptions
        {
            get
            {
                _unknownPermanentExceptions = GetConfigValue("UnknownPermanentExceptions");
                return _unknownPermanentExceptions;
            }
        }

        private static string _filerDomainName;
        public static string FilerDomainName
        {
            get
            {
                _filerDomainName = GetConfigValue("FilerDomainName");
                return _filerDomainName;
            }
        }

        private static string _filerImpersonatedUserName;
        public static string FilerImpersonatedUserName
        {
            get
            {
                _filerImpersonatedUserName = GetConfigValue("FilerImpersonatedUserName");
                return _filerImpersonatedUserName;
            }
        }

        private static string _filerImpersonatedUserPassword;
        public static string FilerImpersonatedUserPassword
        {
            get
            {
                _filerImpersonatedUserPassword = GetConfigValue("FilerImpersonatedUserPassword");
                return _filerImpersonatedUserPassword;
            }
        }

        private static string _filerIsImpersonationEnabled;
        public static string FilerIsImpersonationEnabled
        {
            get
            {
                _filerIsImpersonationEnabled = GetConfigValue("FilerIsImpersonationEnabled");
                return _filerIsImpersonationEnabled;
            }
        }


        private static string _functionURLSPFXRequestQueue;
        public static string FunctionURLSPFXRequestQueue
        {
            get
            {
                _functionURLSPFXRequestQueue = GetConfigValue("ida:LabelingReceiverURL");
                return _functionURLSPFXRequestQueue;
            }
        }

        private static string _sharingLabel;
        public static string SharingLabel
        {
            get
            {
                _sharingLabel = GetConfigValue("SharingLabel");
                return _sharingLabel;
            }
        }

        private static string _templateLabelNoneEncryption;
        public static string TemplateLabelNoneEncryption
        {
            get
            {
                _templateLabelNoneEncryption = GetConfigValue("TemplateLabelNoneEncryption");
                return _templateLabelNoneEncryption;
            }
        }

        private static string _customSensitivityColumnName;
        public static string CustomSensitivityColumnName
        {
            get
            {
                _customSensitivityColumnName = GetConfigValue("CustomSensitivityColumnName");
                return _customSensitivityColumnName;
            }
        }

        private static string _notificationEmailSubject;
        public static string NotificationEmailSubject
        {
            get
            {
                _notificationEmailSubject = GetConfigValue("NotificationEmailSubject");
                return _notificationEmailSubject;
            }
        }

        private static string _webPropertyDefaultSensitivityLabel;
        public static string WebPropertyDefaultSensitivityLabel
        {
            get
            {
                _webPropertyDefaultSensitivityLabel = GetConfigValue("WebPropertyDefaultSensitivityLabel");
                return _webPropertyDefaultSensitivityLabel;
            }
        }

        public static string GetConfigValue(string configKey)
        {
            // When Assembly path is set from consumer like filer, get it from dll configuration
            if (!String.IsNullOrEmpty(CurrentAssemlyPath))
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(CurrentAssemlyPath);
                return GetAppSettingByConfig(config, configKey);
            }
            else // Get it from app settings
            {
                string configValue = Convert.ToString(ConfigurationManager.AppSettings[configKey]);
                return configValue;
            }
        }
    }
}
