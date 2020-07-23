using Microsoft.Online.SharePoint.TenantAdministration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Utils.Nodes
{
    public class AssociatedSitesNode : BaseNode
    {
        public AssociatedSitesNode(HubSiteProperties hubSiteProperties)
        {
            base.Title = "Associated Sites";
            this.SPObjectType = hubSiteProperties.GetType().Name;
            this.SPObject = hubSiteProperties;
        }
    }
}
