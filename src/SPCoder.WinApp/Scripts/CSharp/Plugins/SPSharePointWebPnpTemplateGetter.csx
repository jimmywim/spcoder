﻿using SPCoder.Core.Plugins;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Net;
using System.Security;
using System.Threading;
/// <summary>
/// SPCoder plugin that uses Pnp library to get the xml template from Web object.
/// After getting the xml, the plugin opens a new tab with xml template.
/// </summary>
public class SPSharePointWebPnpTemplateGetter : BasePlugin
{
    public SPSharePointWebPnpTemplateGetter()
    {
        this.TargetType = typeof(Microsoft.SharePoint.Client.Web);
        this.Name       = "Get PnP provisioning template xml";
    }

    public override void Execute(Object target)
    {
        String xmlSource = this.GetTemplate((Microsoft.SharePoint.Client.Web)target);
        Result    = xmlSource;
        ExecuteCallback(xmlSource);
    }
    
    public String GetTemplate(Microsoft.SharePoint.Client.Web web)
    {
        ProvisioningTemplateCreationInformation ptci = new ProvisioningTemplateCreationInformation(web);

        ptci.ProgressDelegate = delegate(String message, Int32 progress, Int32 total)
        {
            // Only to output progress for console UI
            Console.WriteLine("{0:00}/{1:00} - {2}", progress, total, message);
        };

        // Execute actual extraction of the template
        ProvisioningTemplate template = web.GetProvisioningTemplate(ptci);
        
        return template.ToXML(null);
    }
}

//registration code
SPSharePointWebPnpTemplateGetter webPnPTemplateGetter = new SPSharePointWebPnpTemplateGetter();
webPnPTemplateGetter.Callback += GenerateNewSourceTab;
PluginContainer.Register(webPnPTemplateGetter);

logger.LogInfo("Registered plugin SPSharePointWebPnpTemplateGetter");

//webPnPTemplateGetter.Execute(web);
