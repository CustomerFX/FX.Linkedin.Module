using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Sage.Entity.Interfaces;
using Sage.Platform.Application;
using Sage.Platform.Application.UI;
using Sage.Platform.WebPortal.SmartParts;
using Sage.Platform.WebPortal.Workspaces;
using Sage.Platform;

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
                if (mainContentWorkspace == null) return;
                
                foreach (SmartPart smartPart in mainContentWorkspace.SmartParts)
                {
                    if (smartPart.ID != "AccountDetails" && smartPart.ID != "ContactDetails") continue;

                    var btnLinkedin = new ImageButton
                    {
                        ImageUrl = "~/images/icon-linkedin.png",
                        ID = "btnLinkedInSearch",
                        AlternateText = "Search on LinkedIn...",
                        ToolTip = string.Format("Search for {0} on LinkedIn...", this.EntityContext.Description),
                        OnClientClick = string.Format("javascript: var win = window.open('{0}');", GetLinkedinQuery())
                    };

                    foreach (var container in smartPart.Controls.OfType<SmartPartToolsContainer>().Where(c => c.ToolbarLocation == SmartPartToolsLocation.Right))
                    {
                        container.Controls.Add(btnLinkedin);
                    }
                    break;
                }
            }
            catch { }
        }

        private string GetLinkedinQuery()
        {
            const string queryUrl = "https://www.linkedin.com/search/results/{0}/?keywords={1}";

            var queryType = (this.EntityContext.EntityType == typeof(IAccount) ? "companies" : "people");
            var queryValue = this.EntityContext.Description;

            if (this.EntityContext.EntityType == typeof(IAccount))
            {
                var account = EntityFactory.GetById<IAccount>(this.EntityContext.EntityID);
                queryValue = account.AccountName;
            }
            else
            {
                var contact = EntityFactory.GetById<IContact>(this.EntityContext.EntityID);
                queryValue = (contact.FirstName + " " + contact.LastName).Trim();
                queryValue += " " + contact.AccountName;
                
            }

            return string.Format(queryUrl, queryType, HttpUtility.UrlEncode(queryValue.Trim()));
        }
    }
}
