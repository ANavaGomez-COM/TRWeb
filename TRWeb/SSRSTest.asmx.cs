using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace TRWeb
{
    /// <summary>
    /// Summary description for SSRSTest
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SSRSTest : System.Web.Services.WebService
    {

        [WebMethod]
        public List<string> getListPlayers()
        {
            List<string> list = new List<string>();

            list.Add("ABC");
            list.Add("MNO");
            list.Add("PQR");
            list.Add("XYZ");

            return list;
        }

    }
}
