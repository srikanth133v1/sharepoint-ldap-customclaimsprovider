using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration.Claims;

namespace NS.Extranet.ClaimsProvider.Features.FarmFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("51473737-4023-4268-a831-efb554d97718")]
    public class FarmFeatureEventReceiver : SPClaimProviderFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.
        public override string ClaimProviderAssembly
        {
            get { return typeof(NSClaimsProvider).Assembly.FullName; }
        }

        public override string ClaimProviderDescription
        {
            get { return "NS LDAP FBA claim provider provider"; }
        }

        public override string ClaimProviderDisplayName
        {
            get { return NSClaimsProvider.ProviderDisplayName; }
        }

        public override string ClaimProviderType
        {
            get { return typeof(NSClaimsProvider).FullName; }
        }
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            RemoveClaimProvider(ClaimProviderDisplayName);
            base.FeatureDeactivating(properties);
        }
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

            ExecBaseFeatureActivated(properties);
            SPClaimProviderManager cpm = SPClaimProviderManager.Local;

            foreach (SPClaimProviderDefinition cp in cpm.ClaimProviders)
            {

                if (cp.ClaimProviderType == typeof(NSClaimsProvider))
                {

                    cp.IsUsedByDefault = false;

                    cpm.Update();

                    break;

                }

            }

        }
        private void ExecBaseFeatureActivated(Microsoft.SharePoint.SPFeatureReceiverProperties properties)
        {
            // Wrapper function for base FeatureActivated. Used because base
            // keyword can lead to unverifiable code inside lambda expression.
            base.FeatureActivated(properties);
        }
    }
}
