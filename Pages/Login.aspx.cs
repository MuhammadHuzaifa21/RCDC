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

    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string id = txtID.Text;
        string password = txtPassword.Text;

        // Example logic
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