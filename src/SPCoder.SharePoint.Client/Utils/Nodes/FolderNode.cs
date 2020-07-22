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
    public class FolderNode : BaseNode
    {
        public FolderNode(Folder folder)
        {
            folder.Context.Load(folder);
            base.Title = folder.Name;
            base.SPObjectType = folder.GetType().Name;
            base.Url = folder.ServerRelativeUrl;
        }

        private Folder realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            object objParent = base.ParentNode.SPObject;
            if (objParent != null)
            {
                if (objParent is List)
                {
                    Folder folder = ((List)objParent).RootFolder.ResolveSubFolder(this.Title);

                    realObject = folder;
                    return folder;
                }

                if (objParent is Web)
                {
                    Folder folder = ((Web)objParent).GetFolderByServerRelativeUrl(this.Url);
                    realObject = folder;

                    return folder;
                }

                if (objParent is Folder)
                {
                    Folder folder = ((Folder)objParent).ResolveSubFolder(this.Title);

                    realObject = folder;
                    return folder;
                }
            }

            return null;
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            var realObj = GetRealSPObject();
            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:

                    if (realObj != null)
                    {
                        Web objWeb = (Web)base.ParentNode.SPObject;

                        return WebUtils.MakeAbsoluteUrl(objWeb, ((Folder)realObj).ServerRelativeUrl);
                    }
                    else
                        return null;
                case NodeActions.Copy:
                    if (realObj != null && actionItem.Name == "Copy link")
                    {
                        Folder thisfolder = (Folder)realObj;
                        thisfolder.EnsureProperties(f => f.ServerRelativeUrl);

                        if (base.ParentNode.SPObject is List)
                        {
                            // Parent is a List
                            List objList = (List)base.ParentNode.SPObject;
                            Web parentWeb = objList.ParentWeb;
                            return WebUtils.MakeAbsoluteUrl(parentWeb, thisfolder.ServerRelativeUrl);
                        }
                        else if (base.ParentNode.SPObject is Web)
                        {
                            // Parent is a web
                            Web objParent = (Web)base.ParentNode.SPObject;
                            return WebUtils.MakeAbsoluteUrl(objParent, thisfolder.ServerRelativeUrl);
                        }
                        else
                        {
                            // Parent is a folder
                            Folder parentFolder = (Folder)base.ParentNode.SPObject;
                            Web objWeb = parentFolder.ListItemAllFields.ParentList.ParentWeb;
                            return WebUtils.MakeAbsoluteUrl(objWeb, thisfolder.ServerRelativeUrl);
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

        public override bool CanAcceptDragSource(BaseNode draggedItem)
        {
            if (draggedItem is FileNode)
            {
                return true;
            }

            return false;
        }
    }
}
