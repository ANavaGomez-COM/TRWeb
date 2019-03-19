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
                    MenuItem watAdmin = new MenuItem();///ceb
                    MenuItem pAdminItem = new MenuItem();
                    // 9/11/2015 - KG added two new TR Admin menus if they have permissions
                    MenuItem pAdminUserItem = new MenuItem();
                    MenuItem pAdminNotifyItem = new MenuItem();

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
                        ///ceb
                        if (menuItem.Text == "Water Administration")
                        {
                            watAdmin = menuItem;
                        }
                        ///end ceb
                        if (menuItem.Text == "Portal Administration")
                        {
                            pAdminItem = menuItem;
                        }
                        if (menuItem.Text == "User Admin")
                        {
                            pAdminUserItem = menuItem;
                        }
                        if (menuItem.Text == "Notify Admin")
                        {
                            pAdminNotifyItem = menuItem;
                        }
                    }
                    menuItems.Remove(adminItem);
                    menuItems.Remove(propItem);
                    menuItems.Remove(pTickItem);
                    menuItems.Remove(housItem);
                    menuItems.Remove(watItem);
                    menuItems.Remove(pAdminItem);
                    if (a.UserGroup != "Water Admin")
                    {
                        menuItems.Remove(watAdmin);///ceb
                    }

                    if (a.Administrator == false)    // If not an administrator,
                    {
                        menuItems.Remove(pAdminUserItem);
                        menuItems.Remove(pAdminNotifyItem);
                    }
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
                MenuItem watAdmin = new MenuItem();
                MenuItem reportItem = new MenuItem();
                MenuItem pAdminItem = new MenuItem();
                ///ceb
                MenuItem pAdminUserItem = new MenuItem();
                MenuItem pAdminNotifyItem = new MenuItem();
                //end ceb
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
                    if (menuItem.Text == "Water Administration")
                    {
                        watAdmin = menuItem;
                    }
                    if (menuItem.Text == "Report")
                    {
                        reportItem = menuItem;
                    }
                    if (menuItem.Text == "Portal Administration")
                    {
                        pAdminItem = menuItem;
                    }

                    /////ceb
                    if (menuItem.Text == "User Admin")
                    {
                        pAdminUserItem = menuItem;
                    }
                    if (menuItem.Text == "Notify Admin")
                    {
                        pAdminNotifyItem = menuItem;
                    }
                    ///end ceb
                }
                menuItems.Remove(adminItem);
                menuItems.Remove(propItem);
                menuItems.Remove(pTickItem);
                menuItems.Remove(housItem);
                menuItems.Remove(watItem);
                menuItems.Remove(watAdmin);
                menuItems.Remove(reportItem);
                menuItems.Remove(pAdminItem);
                //ceb
                menuItems.Remove(pAdminUserItem);
                menuItems.Remove(pAdminNotifyItem);
                //end ceb


            }
        }
    }
}
