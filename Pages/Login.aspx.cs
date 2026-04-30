using System;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class Pages_Login : System.Web.UI.Page
{
    string connStrMNT = WebConfigurationManager
                        .ConnectionStrings["MyDbConnectionMNT"]
                        .ConnectionString;

    string connStrELEC = WebConfigurationManager
                        .ConnectionStrings["MyDbConnection"]
                        .ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        Session["DC_records"] = null;
        Session["RC_records"] = null;
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        int userId;

        if (!int.TryParse(txtID.Text.Trim(), out userId))
        {
            lblMessage.Text = "Invalid ID format!";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            return;
        }

        string password = txtPassword.Text.Trim();

        // ✅ FIRST: CHECK METERREADERS
        if (CheckLogin(connStrMNT, "METERREADERS", userId, password))
            return;

        // ✅ SECOND: CHECK LOGIN_INFO
        if (CheckLogin(connStrELEC, "LOGIN_INFO", userId, password))
            return;

        // ❌ IF NOT FOUND IN BOTH
        lblMessage.Text = "Invalid ID or Password!";
        lblMessage.ForeColor = System.Drawing.Color.Red;
    }

    private bool CheckLogin(string connStr, string tableName, int userId, string password)
    {
        // ✅ whitelist tables (prevents misuse later)
        if (tableName != "METERREADERS" && tableName != "LOGIN_INFO")
            throw new Exception("Invalid table name");

        using (OracleConnection con = new OracleConnection(connStr))
        {
            con.Open();

            string query = @"
                SELECT ID, USER_NAME, ROLE_ID 
                FROM " + tableName + @" 
                WHERE ID = :id 
                  AND TRIM(PASSWORD) = TRIM(:password) 
                  AND IS_ACTIVE = 'Y'";

            using (OracleCommand cmd = new OracleCommand(query, con))
            {
                cmd.BindByName = true;

                cmd.Parameters.Add(":id", OracleDbType.Int32).Value = userId;
                cmd.Parameters.Add(":password", OracleDbType.Varchar2).Value = password;

                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        Session["User"] = dr["ID"].ToString();
                        Session["UserName"] = dr["USER_NAME"].ToString();
                        Session["Role"] = dr["ROLE_ID"].ToString();

                        Response.Redirect("~/Pages/DC.aspx");
                        return true;
                    }
                }
            }
        }

        return false;
    }


}