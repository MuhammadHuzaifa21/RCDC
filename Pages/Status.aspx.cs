﻿using System;
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
                            string dcDt = dr["DC_DT"] == DBNull.Value ? "N/A" : Convert.ToDateTime(dr["DC_DT"]).ToString("dd-MMM-yyyy");
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
        string meterNo = txtCheckMeter.Text.Trim();

        if (string.IsNullOrEmpty(meterNo))
        {
            ShowError("Please enter a Meter Number.");
            return;
        }

        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                // ALWAYS CHECK FRESH FROM DATABASE - don't use ViewState
                string checkQuery = "SELECT IS_DC FROM DCRC WHERE TRIM(METERNO) = TRIM(:meterNo)";
                using (OracleCommand checkCmd = new OracleCommand(checkQuery, con))
                {
                    checkCmd.Parameters.Add(":meterNo", OracleDbType.Varchar2).Value = meterNo;
                    object result = checkCmd.ExecuteScalar();

                    if (result == null)
                    {
                        ShowError(string.Format("Meter '{0}' not found.", meterNo));
                        return;
                    }

                    int currentIsDc = Convert.ToInt32(result);

                    if (currentIsDc != 1)
                    {
                        ShowError(string.Format("Meter '{0}' can not be updated.", meterNo, currentIsDc));
                        return;
                    }
                }

                // Proceed with update
                string updateQuery = @"
                UPDATE DCRC 
                SET IS_DC = 2, 
                    DC_BY = 1, 
                    DC_DT = SYSDATE 
                WHERE TRIM(METERNO) = TRIM(:meterNo) 
                AND IS_DC = 1";

                using (OracleCommand cmd = new OracleCommand(updateQuery, con))
                {
                    cmd.Parameters.Add(":meterNo", OracleDbType.Varchar2).Value = meterNo;
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Just show success - DON'T call btnCheckStatus_Click again
                        ShowSuccess(string.Format("✓ Meter {0} successfully updated to DISCONNECTED", meterNo));
                        btnUpdateDisconnection.Enabled = false;
                        btnUpdateDisconnection.Text = "Update Disconnection (Already Updated)";

                        // Update the displayed info manually instead of calling btnCheckStatus_Click
                        lblStatusResult.Text = string.Format(@"
                        <div class='success-message'>
                            <strong>Successfully Updated!</strong><br/>
                            Meter {0} has been marked as DISCONNECTED
                        </div>
                        <table class='info-table'>
                            <tr><th>Status</th><td><span class='status-badge status-disconnected'>DISCONNECTED (Updated)</span></td></tr>
                            <tr><th>Disconnected By</th><td>{1}</td></tr>
                            <tr><th>Disconnected On</th><td>{2}</td></tr>
                        </table>",
                            meterNo,
                            Session["User"] != null ? Session["User"].ToString() : "SYSTEM",
                            DateTime.Now.ToString("dd-MMM-yyyy"));

                        pnlStatusResult.Visible = true;
                    }
                    else
                    {
                        ShowError(string.Format("Update failed for meter '{0}'.", meterNo));
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