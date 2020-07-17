﻿using Microsoft.SharePoint.Client;
using SPCoder.Core.Plugins;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.SharePoint.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SPCoder.Utils.Nodes
{
    public class FileNode : BaseNode, LeafNode
    {
        public FileNode(File file)
        {
            file.Context.Load(file);
            base.Title = file.Name;
            base.SPObjectType = file.GetType().Name;
            base.Url = file.ServerRelativeUrl;
        }

        private File realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            object objParent = base.ParentNode.SPObject;
            if (objParent != null)
            {
                if (objParent is Web)
                {
                    File file = ((Web)objParent).GetFileByServerRelativeUrl(this.Url);
                    realObject = file;

                    return file;
                }

                if (objParent is Folder)
                {
                    File file = ((Folder)objParent).GetFile(this.Title);

                    realObject = file;
                    return file;
                }
            }

            return null;
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            var realObj = GetRealSPObject();
            File thisFile = ((File)realObj);
            thisFile.EnsureProperties(f => f.ServerRelativeUrl);

            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:

                    if (realObj != null)
                    {
                        Web objWeb = (Web)base.ParentNode.SPObject;

                        string url = objWeb.Url.Replace(objWeb.ServerRelativeUrl, thisFile.ServerRelativeUrl);
                        return url;
                    }
                    else
                        return null;
                case NodeActions.Copy:
                    if (realObj != null && actionItem.Name == "Copy link")
                    {
                        if (base.ParentNode.SPObject is Folder)
                        {
                            // Parent is a folder
                            Folder parentFolder = (Folder)base.ParentNode.SPObject;
                            Web objWeb = parentFolder.ListItemAllFields.ParentList.ParentWeb;

                            return WebUtils.MakeAbsoluteUrl(objWeb, thisFile.ServerRelativeUrl);
                        } 
                        else
                        {
                            // Parent is a web
                            Web objParent = (Web)base.ParentNode.SPObject;
                            return WebUtils.MakeAbsoluteUrl(objParent, thisFile.ServerRelativeUrl);
                        }
                    }
                    else
                        return null;
                //for plugins always return the real object
                case NodeActions.Plugin:
                    if (realObj != null)
                    {
                        return realObj;
                    }
                    else
                        return null;
                default:
                    return null;
            }
        }

        public override List<BaseActionItem> GetNodeActions()
        {
            List<BaseActionItem> actions = new List<BaseActionItem>();
            actions.Add(new BaseActionItem { Node = this, Name = "Open in browser", Action = Core.Utils.NodeActions.ExternalOpen });
            actions.Add(new BaseActionItem { Node = this, Name = "Copy link", Action = Core.Utils.NodeActions.Copy });
            //Check all plugins
            var baseActions = base.GetNodeActions();
            if (baseActions.Count > 0)
                actions.AddRange(baseActions);

            return actions;
        }
    }
}
