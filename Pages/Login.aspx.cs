using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["DC_records"] = null;
        Session["RC_records"] = null;
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string id = txtID.Text;
        string password = txtPassword.Text;

        //var users = new Dictionary<string, string>
        //{
        //    { "1", "1" },
        //    { "25128", "123" },
        //    { "test", "pass" }
        //};

        //if (users.ContainsKey(id) && users[id] == password)
        //{
        //    Session["User"] = id;
        //    Response.Redirect("~/Pages/DC.aspx");
        //}
        //else
        //{
        //    lblMessage.Text = "Account not found. Please register first.";
        //}

        if (id == "1" && password == "1") 
        { 
            Session["User"] = id;
            Response.Redirect("~/Pages/DC.aspx"); 
        } 
        else 
        { 
            lblMessage.Text = "Account not found. Please register first."; 
        }
    }
}