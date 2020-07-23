using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Utils.Nodes
{
    public class HubSiteNode : ScopedWebNode
    {
        public HubSiteProperties HubSiteProperties;

        public HubSiteNode(ClientContext ctx, HubSiteProperties hubSiteProperties)
            : base(ctx)
        {
            this.HubSiteProperties = hubSiteProperties;
        }

        public void InitAssociatedHubSitesNode()
        {
            var associatedSitesNode = new AssociatedSitesNode(this.HubSiteProperties);
            associatedSitesNode.ParentNode = this;
            associatedSitesNode.RootNode = this.RootNode;

            this.Children.Insert(0, associatedSitesNode);
        }
    }
}
