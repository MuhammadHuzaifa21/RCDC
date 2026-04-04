using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;


public partial class Components_Header : System.Web.UI.UserControl
{
    string connStr = WebConfigurationManager
                    .ConnectionStrings["MyDbConnectionMNT"]
                    .ConnectionString;


    protected void Page_Load(object sender, EventArgs e)
    {
        string page = System.IO.Path.GetFileName(Request.Path);

        if (page == "Login.aspx")
        {
            lnkLogin.Visible = false;
            btnUpdate.Visible = false;

            divCharges.Visible = false;
            divReceiving.Visible = false;
            divConnection.Visible = false;

            //lnkRis.Visible = false;
            //lnkSecurity.Visible = false;
            //lnkEnforcement.Visible = false;

            //lnkDC.Visible = false;
            //lnkRC.Visible = false;
            lnkRecords.Visible = false;
            btnLogout.Visible = false;            
        }

        if (page == "DC.aspx")
        {
            lnkLogin.Visible = false;
            //lnkDC.Visible = false;
            lnkRC.Visible = true;
            lnkRecords.Visible = true;
            btnLogout.Visible = true; 
        }

        if (page == "RC.aspx")
        {
            lnkLogin.Visible = false;
            lnkDC.Visible = true;
            //lnkRC.Visible = false;
            lnkRecords.Visible = true;
            btnLogout.Visible = true;            
        }
        
        if (page == "Record.aspx")
        {
            lnkLogin.Visible = false;
            lnkDC.Visible = true;
            lnkRC.Visible = true;
            lnkRecords.Visible = false;
            btnLogout.Visible = true;
        }

        if (page == "RIS.aspx" || page == "Security.aspx" || page == "Enforcement.aspx")
        {
            lnkLogin.Visible = false;
            //lnkRIS.Visible = false;
            lnkDC.Visible = true;
            lnkRC.Visible = true;
            lnkRecords.Visible = true;
            btnLogout.Visible = true;
        }
    }

    /* GET UPDATED */
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        string enteredPassword = hdnPassword.Value;

        // 🔐 Change this to your actual password logic
        //if (enteredPassword != "admin")
        //{
        //    lblMessage.Text = "You are not authorized.";
        //    lblMessage.ForeColor = System.Drawing.Color.Red;
        //    return;
        //}

        using (OracleConnection con = new OracleConnection(connStr))
        {
            using (OracleCommand cmd = new OracleCommand("SP_DCRC", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Records Updated.";
                    lblMessage.ForeColor = System.Drawing.Color.White;

                    // Register JavaScript to show and hide the label
                    string script = @"
                        var lbl = document.getElementById('" + lblMessage.ClientID + @"');
                        lbl.style.display = 'inline-block';
                        setTimeout(function(){ lbl.style.display = 'none'; }, 3000);"; // 3 seconds
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showMessage", script, true);
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                }
            }
        }
    }
    
    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect("~/Pages/Login.aspx");
    }
   
}