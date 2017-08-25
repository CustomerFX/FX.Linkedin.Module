using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sage.Entity.Interfaces;
using Sage.Platform.Application;
using Sage.Platform.Application.UI;
using Sage.Platform.WebPortal.SmartParts;
using Sage.Platform.WebPortal.Workspaces;

namespace FX.Linkedin
{
    public class Module : IModule
    {
        public Module() { }

        [ServiceDependency(Type = typeof(WorkItem))]
        public UIWorkItem ParentWorkItem { get; set; }

        [ServiceDependency]
        public IEntityContextService EntityContext { get; set; }

        public void Load()
        {
            try
            {
                if (this.EntityContext.EntityType != typeof(IAccount) && this.EntityContext.EntityType != typeof(IContact))
                    return;
            
                var mainContentWorkspace = this.ParentWorkItem.Workspaces["MainContent"] as MainContentWorkspace;
                
                foreach (SmartPart smartPart in mainContentWorkspace.SmartParts)
                {
                    if (smartPart.ID == "AccountDetails" || smartPart.ID == "ContactDetails")
                    {
                        var btnLinkedin = new ImageButton
                        {
                            ImageUrl = "~/images/icon-linkedin.png",
                            ID = "btnLinkedInSearch",
                            AlternateText = "Search on LinkedIn...",
                            ToolTip = "Search on LinkedIn...",
                            OnClientClick = string.Format("javascript: var win = window.open('{0}');", GetLinkedinQuery(this.EntityContext.EntityType, this.EntityContext.Description))
                        };

                        foreach (Control control in smartPart.Controls)
                        {
                            var container = control as SmartPartToolsContainer;
                            if (container != null && container.ToolbarLocation == SmartPartToolsLocation.Right)
                            {
                                container.Controls.Add(btnLinkedin);
                            }
                        }
                        break;
                    }
                }
            }
            catch { }
        }

        private string GetLinkedinQuery(Type entityType, string entityName)
        {
            const string url = "https://www.linkedin.com/search/results/{0}/?keywords={1}";

            var queryType = (entityType == typeof(IAccount) ? "companies" : "people");
            var queryValue = entityName;

            return string.Format(url,queryType, HttpUtility.UrlEncode(queryValue));
        }
    }
}
