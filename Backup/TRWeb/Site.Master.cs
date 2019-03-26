using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace TRWeb
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private Auth a = new Auth();

        protected void Page_Load(object sender, EventArgs e)
        {
            ArrayList userInfo = new ArrayList();
            userInfo = (ArrayList)Session["User"];

            if (userInfo.Count > 0)
            {
                a = ((Auth)userInfo[0]);

                if (a.UserGroup == "IT")
                {

                }
                else
                {
                    MenuItemCollection menuItems = NavigationMenu.Items;
                    MenuItem adminItem = new MenuItem();
                    MenuItem propItem = new MenuItem();
                    MenuItem pTickItem = new MenuItem();
                    MenuItem housItem = new MenuItem();
                    MenuItem watItem = new MenuItem();
                    MenuItem pAdminItem = new MenuItem();
                    foreach (MenuItem menuItem in menuItems)
                    {
                        if (menuItem.Text == "Administration")
                        {
                            adminItem = menuItem;
                        }
                        if (menuItem.Text == "Parking Ticket")
                        {
                            pTickItem = menuItem;
                        }
                        if (menuItem.Text == "Property Tax")
                        {
                            propItem = menuItem;
                        }
                        if (menuItem.Text == "Public Housing")
                        {
                            housItem = menuItem;
                        }
                        if (menuItem.Text == "Water Utility")
                        {
                            watItem = menuItem;
                        }
                        if (menuItem.Text == "Portal Administration")
                        {
                            pAdminItem = menuItem;
                        }   
                    }
                    menuItems.Remove(adminItem);
                    menuItems.Remove(propItem);
                    menuItems.Remove(pTickItem);
                    menuItems.Remove(housItem);
                    menuItems.Remove(watItem);
                    menuItems.Remove(pAdminItem);
                }
            }
            else
            {
                MenuItemCollection menuItems = NavigationMenu.Items;
                MenuItem adminItem = new MenuItem();
                MenuItem propItem = new MenuItem();
                MenuItem pTickItem = new MenuItem();
                MenuItem housItem = new MenuItem();
                MenuItem watItem = new MenuItem();
                MenuItem reportItem = new MenuItem();
                MenuItem pAdminItem = new MenuItem();
                foreach (MenuItem menuItem in menuItems)
                {
                    if (menuItem.Text == "Administration")
                    {
                        adminItem = menuItem;
                    }
                    if (menuItem.Text == "Parking Ticket")
                    {
                        pTickItem = menuItem;
                    }
                    if (menuItem.Text == "Property Tax")
                    {
                        propItem = menuItem;
                    }
                    if (menuItem.Text == "Public Housing")
                    {
                        housItem = menuItem;
                    }
                    if (menuItem.Text == "Water Utility")
                    {
                        watItem = menuItem;
                    }
                    if (menuItem.Text == "Report")
                    {
                        reportItem = menuItem;
                    }
                    if (menuItem.Text == "Portal Administration")
                    {
                        pAdminItem = menuItem;
                    } 
                }
                menuItems.Remove(adminItem);
                menuItems.Remove(propItem);
                menuItems.Remove(pTickItem);
                menuItems.Remove(housItem);
                menuItems.Remove(watItem);
                menuItems.Remove(reportItem);
                menuItems.Remove(pAdminItem);
            }
        }
    }
}
