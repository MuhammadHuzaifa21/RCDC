using System;
using System.Data;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

public partial class Pages_Status : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
                   .ConnectionStrings["MyDbConnectionMNT"]
                   .ConnectionString;

    // Use properties with ViewState to persist values between postbacks
    private string CurrentMeterNo
    {
        get { return ViewState["CurrentMeterNo"] as string ?? ""; }
        set { ViewState["CurrentMeterNo"] = value; }
    }

    private string CurrentBarcode
    {
        get { return ViewState["CurrentBarcode"] as string ?? ""; }
        set { ViewState["CurrentBarcode"] = value; }
    }

    private int CurrentIsDc
    {
        get { return ViewState["CurrentIsDc"] != null ? (int)ViewState["CurrentIsDc"] : 0; }
        set { ViewState["CurrentIsDc"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["User"] == null)
            Response.Redirect("~/Pages/Login.aspx");
    }

//    protected void btnCheckStatus_Click(object sender, EventArgs e)
//    {
//        string meterNo = txtCheckMeter.Text.Trim();

//        if (string.IsNullOrEmpty(meterNo))
//        {
//            ShowError("Please enter a Meter Number.");
//            btnUpdateDisconnection.Enabled = false;
//            pnlStatusResult.Visible = true;
//            return;
//        }

//        try
//        {
//            using (OracleConnection con = new OracleConnection(connStr))
//            {
//                string query = @"
//                    SELECT RESNAME, IS_DC, DC_BY, DC_DT, METERNO, PRCNT_NM, BLOCK_NM, BARCODE
//                    FROM DCRC
//                    WHERE METERNO = :meterNo
//                    AND ROWNUM = 1";

//                using (OracleCommand cmd = new OracleCommand(query, con))
//                {
//                    cmd.Parameters.Add(":meterNo", OracleDbType.Varchar2).Value = meterNo;
//                    con.Open();

//                    using (OracleDataReader dr = cmd.ExecuteReader())
//                    {
//                        if (dr.Read())
//                        {
//                            // STORE values in ViewState for later use
//                            CurrentMeterNo = meterNo;
//                            CurrentBarcode = dr["BARCODE"] != DBNull.Value ? dr["BARCODE"].ToString() : GenerateBarcode(meterNo);
//                            CurrentIsDc = Convert.ToInt32(dr["IS_DC"]);

//                            string resname = dr["RESNAME"].ToString();
//                            string dcBy = dr["DC_BY"] == DBNull.Value ? "N/A" : dr["DC_BY"].ToString();
//                            string dcDt = dr["DC_DT"] == DBNull.Value ? "N/A" : Convert.ToDateTime(dr["DC_DT"]).ToString("dd-MMM-yyyy HH:mm");
//                            string precinct = dr["PRCNT_NM"].ToString();
//                            string block = dr["BLOCK_NM"].ToString();

//                            // Check if IS_DC = 1 (disconnected) or = 0 (active)
//                            if (CurrentIsDc == 1)
//                            {
//                                // Show disconnected info with barcode
//                                string statusBadge = "<span class='status-badge status-disconnected'>DISCONNECTED</span>";

//                                string resultHtml = @"
//                                    <table class='info-table'>
//                                        <tr><th style='width:40%;'>Meter Number</th><td>" + meterNo + @"</td>
//                                        </tr>
//                                        <tr><th>Consumer Name</th><td>" + resname + @"</td>
//                                        </tr>
//                                        <tr><th>Precinct</th><td>" + precinct + @"</td>
//                                        </tr>
//                                        <tr><th>Block</th><td>" + block + @"</td>
//                                        </tr>
//                                        <tr><th>Barcode</th><td><strong>" + CurrentBarcode + @"</strong></td>
//                                        </tr>
//                                        <tr><th>Status</th><td>" + statusBadge + @"</td>
//                                        </tr>
//                                        <tr><th>Disconnected By</th><td>" + dcBy + @"</td>
//                                        </tr>
//                                        <tr><th>Disconnected On</th><td>" + dcDt + @"</td>
//                                        </tr>
//                                    </table>";

//                                lblStatusResult.Text = resultHtml;
//                                pnlStatusResult.Visible = true;

//                                // Enable update button for IS_DC = 1
//                                btnUpdateDisconnection.Enabled = true;
//                                btnUpdateDisconnection.Text = "Update Disconnection ";
//                            }
//                            else if (CurrentIsDc == 0)
//                            {
//                                // Store the meter but don't show full record
//                                string resultHtml = @"
//                                    <div class='warning-message'>
//                                        <strong>No Disconnection </strong><br/>
//                                    </div>";

//                                lblStatusResult.Text = resultHtml;
//                                pnlStatusResult.Visible = true;

//                                // Disable update button for IS_DC = 0
//                                btnUpdateDisconnection.Enabled = false;
//                                btnUpdateDisconnection.Text = "Update Disconnection ";
//                            }
//                            else if (CurrentIsDc == 2)
//                            {
//                                // Already updated to 2
//                                string statusBadge = "<span class='status-badge status-disconnected'>DISCONNECTED (Updated)</span>";

//                                string resultHtml = @"
//                                    <div class='warning-message'>
//                                        <strong>Already Updated</strong><br/>
//                                        This meter has already been marked as DISCONNECTED.
//                                    </div>
//                                    <table class='info-table'>
//                                       <tr><th style='width:40%;'>Meter Number</th><td>" + meterNo + @"</td>
//                                        </tr>
//                                        <tr><th>Consumer Name</th><td>" + resname + @"</td>
//                                        </tr>
//                                        <tr><th>Precinct</th><td>" + precinct + @"</td>
//                                        </tr>
//                                        <tr><th>Block</th><td>" + block + @"</td>
//                                        </tr>
//                                        <tr><th>Barcode</th><td><strong>" + CurrentBarcode + @"</strong></td>
//                                        </tr>
//                                        <tr><th>Status</th><td>" + statusBadge + @"</td>
//                                        </tr>
//                                        <tr><th>Disconnected By</th><td>" + dcBy + @"</td>
//                                        </tr>
//                                        <tr><th>Disconnected On</th><td>" + dcDt + @"</td>
//                                        </tr>
//                                    </table>";

//                                lblStatusResult.Text = resultHtml;
//                                pnlStatusResult.Visible = true;

//                                // Disable update button for IS_DC = 2
//                                btnUpdateDisconnection.Enabled = false;
//                                btnUpdateDisconnection.Text = "Update Disconnection (Already Updated)";
//                            }
//                        }
//                        else
//                        {
//                            ShowError(string.Format("Meter Number '{0}' not found in DCRC table.", meterNo));
//                            btnUpdateDisconnection.Enabled = false;
//                            // Clear stored values when meter not found
//                            CurrentMeterNo = "";
//                            CurrentBarcode = "";
//                            CurrentIsDc = 0;
//                        }
//                    }
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            ShowError("Database error: " + ex.Message);
//            btnUpdateDisconnection.Enabled = false;
//        }
//    }

    protected void btnCheckStatus_Click(object sender, EventArgs e)
    {
        string meterNo = txtCheckMeter.Text.Trim();

        if (string.IsNullOrEmpty(meterNo))
        {
            ShowError("Please enter a Meter Number.");
            btnUpdateDisconnection.Enabled = false;
            pnlStatusResult.Visible = true;
            return;
        }

        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                string query = @"
                    SELECT RESNAME, IS_DC, DC_BY, DC_DT, METERNO, PRCNT_NM, BLOCK_NM, BARCODE
                    FROM DCRC
                    WHERE METERNO = :meterNo
                    AND ROWNUM = 1";

                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Parameters.Add(":meterNo", OracleDbType.Varchar2).Value = meterNo;
                    con.Open();

                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // STORE values in ViewState for later use
                            CurrentMeterNo = meterNo;
                            CurrentBarcode = dr["BARCODE"] != DBNull.Value ? dr["BARCODE"].ToString() : GenerateBarcode(meterNo);
                            CurrentIsDc = Convert.ToInt32(dr["IS_DC"]);

                            string resname = dr["RESNAME"].ToString();
                            string dcBy = dr["DC_BY"] == DBNull.Value ? "N/A" : dr["DC_BY"].ToString();
                            string dcDt = dr["DC_DT"] == DBNull.Value ? "N/A" : Convert.ToDateTime(dr["DC_DT"]).ToString("dd-MMM-yyyy HH:mm");
                            string precinct = dr["PRCNT_NM"].ToString();
                            string block = dr["BLOCK_NM"].ToString();

                            // Check if IS_DC = 1 (disconnected) or = 0 (active)
                            if (CurrentIsDc == 1)
                            {
                                // Show disconnected info with barcode
                                string statusBadge = "<span class='status-badge status-disconnected'>DISCONNECTED</span>";

                                string resultHtml = @"
                                    <table class='info-table'>
                                        
                                        <tr><th>Consumer Name</th><td>" + resname + @"</td>
                                        </tr>
                                        <tr><th>Barcode</th><td><strong>" + CurrentBarcode + @"</strong></td>
                                        </tr>
                                        <tr><th>Status</th><td>" + statusBadge + @"</td>
                                        </tr>
                                        <tr><th>Disconnected By</th><td>" + dcBy + @"</td>
                                        </tr>
                                        <tr><th>Disconnected On</th><td>" + dcDt + @"</td>
                                        </tr>
                                    </table>";

                                lblStatusResult.Text = resultHtml;
                                pnlStatusResult.Visible = true;

                                // Enable update button for IS_DC = 1
                                btnUpdateDisconnection.Enabled = true;
                                btnUpdateDisconnection.Text = "Update Disconnection ";
                            }
                            else if (CurrentIsDc == 0)
                            {
                                // Store the meter but don't show full record
                                string resultHtml = @"
                                    <div class='warning-message'>
                                        <strong>No Disconnection. </strong><br/>
                                    </div>";

                                lblStatusResult.Text = resultHtml;
                                pnlStatusResult.Visible = true;

                                // Disable update button for IS_DC = 0
                                btnUpdateDisconnection.Enabled = false;
                                btnUpdateDisconnection.Text = "Update Disconnection";
                            }
                            else if (CurrentIsDc == 2)
                            {
                                // Already updated to 2
                                string statusBadge = "<span class='status-badge status-disconnected'>DISCONNECTED (Updated)</span>";

                                string resultHtml = @"
                                    <div class='warning-message'>
                                        <strong>Already Updated</strong><br/>
                                        This meter has already been marked as DISCONNECTED.
                                    </div>
                                    <table class='info-table'>
                                        <tr><th>Consumer Name</th><td>" + resname + @"</td>
                                        </tr>
                                        <tr><th>Barcode</th><td><strong>" + CurrentBarcode + @"</strong></td>
                                        </tr>
                                        <tr><th>Status</th><td>" + statusBadge + @"</td>
                                        </tr>
                                        <tr><th>Disconnected By</th><td>" + dcBy + @"</td>
                                        </tr>
                                        <tr><th>Disconnected On</th><td>" + dcDt + @"</td>
                                        </tr>
                                    </table>";

                                lblStatusResult.Text = resultHtml;
                                pnlStatusResult.Visible = true;

                                // Disable update button for IS_DC = 2
                                btnUpdateDisconnection.Enabled = false;
                                btnUpdateDisconnection.Text = "Update Disconnection (Already Updated)";
                            }
                        }
                        else
                        {
                            ShowError(string.Format("Meter Number '{0}' not found in DCRC table.", meterNo));
                            btnUpdateDisconnection.Enabled = false;
                            // Clear stored values when meter not found
                            CurrentMeterNo = "";
                            CurrentBarcode = "";
                            CurrentIsDc = 0;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("Database error: " + ex.Message);
            btnUpdateDisconnection.Enabled = false;
        }
    }


    protected void btnUpdateDisconnection_Click(object sender, EventArgs e)
    {
        // Get the meter number directly from the input field
        string meterNo = txtCheckMeter.Text.Trim();

        if (string.IsNullOrEmpty(meterNo))
        {
            ShowError("Please enter a Meter Number in the field above.");
            return;
        }

        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                // FIRST: Check what's actually in the database for this meter
                string checkQuery = "SELECT METERNO, IS_DC, BARCODE FROM DCRC WHERE METERNO = :meterNo";
                using (OracleCommand checkCmd = new OracleCommand(checkQuery, con))
                {
                    // Try multiple matching methods
                    checkCmd.Parameters.Add(":meterNo", OracleDbType.Varchar2).Value = meterNo;

                    using (OracleDataReader dr = checkCmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string dbMeterNo = dr["METERNO"].ToString();
                            int currentIsDcValue = Convert.ToInt32(dr["IS_DC"]);
                            string barcode = dr["BARCODE"] != DBNull.Value ? dr["BARCODE"].ToString() : GenerateBarcode(meterNo);

                            if (currentIsDcValue != 1)
                            {
                                ShowError(string.Format("Meter '{0}' has IS_DC = {1}. Only meters with IS_DC = 1 can be updated to IS_DC = 2.", meterNo, currentIsDcValue));
                                return;
                            }
                        }
                        else
                        {
                            // Try with TRIM and UPPER
                            checkCmd.CommandText = "SELECT METERNO, IS_DC, BARCODE FROM DCRC WHERE TRIM(UPPER(METERNO)) = TRIM(UPPER(:meterNo))";
                            using (OracleDataReader dr2 = checkCmd.ExecuteReader())
                            {
                                if (dr2.Read())
                                {
                                    string dbMeterNo = dr2["METERNO"].ToString();
                                    int currentIsDcValue = Convert.ToInt32(dr2["IS_DC"]);

                                    if (currentIsDcValue == 1)
                                    {
                                        // Found with TRIM/UPPER, use this meter number for update
                                        meterNo = dbMeterNo;
                                    }
                                    else
                                    {
                                        ShowError(string.Format("Meter '{0}' has IS_DC = {1}. Only meters with IS_DC = 1 can be updated.", meterNo, currentIsDcValue));
                                        return;
                                    }
                                }
                                else
                                {
                                    ShowError(string.Format("Meter Number '{0}' not found in database. Please check the meter number.", meterNo));
                                    return;
                                }
                            }
                        }
                    }
                }

                // Perform the update with TRIM to handle spaces
                string updateQuery = @"
                UPDATE DCRC 
                SET IS_DC = 2, 
                    DC_BY = :dcBy, 
                    DC_DT = SYSDATE 
                WHERE METERNO =:meterNo 
                AND IS_DC = 1";

                using (OracleCommand cmd = new OracleCommand(updateQuery, con))
                {
                    cmd.Parameters.Add(":meterNo", OracleDbType.Varchar2).Value = meterNo;
                    cmd.Parameters.Add(":dcBy", OracleDbType.Varchar2).Value = Session["User"] != null ? Session["User"].ToString() : "SYSTEM";

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowSuccess(string.Format(
                            "Successfully updated! Meter {0} has been set to DISCONNECTED (IS_DC = 2).<br/>" +
                            "Disconnected By: {1}<br/>" +
                            "Disconnected On: {2}",
                            meterNo,
                            Session["User"] != null ? Session["User"].ToString() : "SYSTEM",
                            DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss")));

                        // Disable update button after successful update
                        btnUpdateDisconnection.Enabled = false;
                        btnUpdateDisconnection.Text = "Update Disconnection (Already Updated)";

                        // Refresh the status display
                        btnCheckStatus_Click(sender, e);
                    }
                    else
                    {
                        throw new Exception(string.Format("No records updated for meter '{0}'. IS_DC is not 1 or meter number doesn't match exactly in database.", meterNo));
                        // Show detailed error for debugging
                        //ShowError(string.Format("No records updated for meter '{0}'.<br/><br/>Possible reasons:<br/>1. IS_DC is not 1 in database<br/>2. Meter number doesn't match exactly<br/><br/>Please verify the meter number in database.", meterNo));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("Error updating record: " + ex.Message);
        }
    }
    private string GenerateBarcode(string meterNo)
    {
        if (!string.IsNullOrEmpty(meterNo) && meterNo.Length >= 4)
        {
            string prefix = meterNo.Substring(0, 4).ToUpper();
            string suffix = meterNo.Length > 4 ? meterNo.Substring(4) : meterNo;
            return string.Format("{0}-{1}", prefix, suffix);
        }
        return meterNo;
    }

    private void ShowError(string message)
    {
        lblStatusResult.Text = string.Format("<div class='error-message'><strong>Error</strong><br/>{0}</div>", message);
        pnlStatusResult.Visible = true;
    }

    private void ShowSuccess(string message)
    {
        lblStatusResult.Text = string.Format("<div class='success-message'><strong>Success</strong><br/>{0}</div>", message);
        pnlStatusResult.Visible = true;
    }
}