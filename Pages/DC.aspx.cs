using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

public partial class Pages_DC : System.Web.UI.Page
{

    string connStr = WebConfigurationManager
                    .ConnectionStrings["MyDbConnectionMNT"]
                    .ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["User"] == null)
            Response.Redirect("~/Pages/Login.aspx");        

        if (!IsPostBack)
        {
            LoadPrecincts();
            LoadBlocks("");
        }

        if (Session["DC_records"] != null)
        {
            pnlRecords.Visible = true;
            pcdShowRecord();
        }

        // Only handle swipe postbacks
        if (IsPostBack)
            HandleSwipe();
    }

    /* LOAD PRECINCT */
    void LoadPrecincts()
    {
        using (OracleConnection con = new OracleConnection(connStr))
        {
            lblStatus.Text = "";

            string query = @"SELECT DISTINCT PRCNT_NM 
                            FROM DCRC 
                            ORDER BY PRCNT_NM";

            OracleDataAdapter da = new OracleDataAdapter(query, con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            ddlPrecinct.DataSource = dt;
            ddlPrecinct.DataTextField = "PRCNT_NM";
            ddlPrecinct.DataValueField = "PRCNT_NM";
            ddlPrecinct.DataBind();

            ddlPrecinct.Items.Insert(0, new ListItem("-- All Precincts --", ""));
        }
    }

    /* LOAD BLOCK */
    void LoadBlocks(string precinct)
    {
        using (OracleConnection con = new OracleConnection(connStr))
        {
            lblStatus.Text = "";

            string query = "SELECT DISTINCT BLOCK_NM FROM DCRC";

            if (!string.IsNullOrEmpty(precinct))
                query += " WHERE PRCNT_NM=:precinct";

            query += " ORDER BY BLOCK_NM";

            OracleCommand cmd = new OracleCommand(query, con);

            if (!string.IsNullOrEmpty(precinct))
                cmd.Parameters.Add(":precinct", OracleDbType.Varchar2).Value = precinct;

            OracleDataAdapter da = new OracleDataAdapter(cmd);

            DataTable dt = new DataTable();

            da.Fill(dt);

            ddlBlock.DataSource = dt;
            ddlBlock.DataTextField = "BLOCK_NM";
            ddlBlock.DataValueField = "BLOCK_NM";
            ddlBlock.DataBind();

            ddlBlock.Items.Insert(0, new ListItem("-- All Blocks --", ""));
        }
    }

    /* DROPDOWN SELECTED VALUE */
    protected void ddlPrecinct_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblStatus.Text = "";
        lblNoRecord.Text = "";

        LoadBlocks(ddlPrecinct.SelectedValue);
    }

    /* SHOW RECORD */
    void pcdShowRecord()
    {
        //lblStatus.Text = "";

        DataTable dt = (DataTable)Session["DC_records"];

        if (dt == null || dt.Rows.Count == 0)
        {
            pnlRecords.Visible = false;
            lblNoRecord.Text = "No records to display. <br /> Please select again.";
            lblNoRecord.ForeColor = System.Drawing.Color.Orange;
            return;
        }

        int index = (int)Session["DC_index"];

        // Safety: ensure index is within bounds
        if (index < 0) index = 0;
        if (index >= dt.Rows.Count) index = dt.Rows.Count - 1;

        DataRow row = dt.Rows[index];

        lblName.Text = row["RESNAME"].ToString();
        lblAddress.Text = row["ADDRESS"].ToString(); //  + " " + row["PRCNT_NM"].ToString() + " " + row["BLOCK_NM"].ToString()  
        lblMeterNo.Text = row["METERNO"].ToString();
        lblArrears.Text = row["TBIL_AMT_DIF"].ToString();

        lblBarcode.Text = row["BARCODE"].ToString();        

        DataTable bill = new DataTable();

        bill.Columns.Add("BillType");
        bill.Columns.Add("BillAmount");
        bill.Columns.Add("ReceivedAmount");

        bill.Rows.Add("Maintenance", row["MBIL_AMT"], row["MBIL_AMT_REC"]);
        bill.Rows.Add("Electric", row["EBIL_AMT"], row["EBIL_AMT_REC"]);
        bill.Rows.Add("Water", row["WBIL_AMT"], row["WBIL_AMT_REC"]);
        bill.Rows.Add("Gas", row["GBIL_AMT"], row["GBIL_AMT_REC"]);
        bill.Rows.Add("Rent", row["RBIL_AMT"], row["RBIL_AMT_REC"]);
        bill.Rows.Add("BNB", row["BBIL_AMT"], row["BBIL_AMT_REC"]);

        gvBillBreakup.DataSource = bill;
        gvBillBreakup.DataBind();

        if (Convert.ToDecimal(lblArrears.Text) < 0)
            lblArrears.ForeColor = System.Drawing.Color.Red;

        lblCounter.Text = (index + 1) + " / " + dt.Rows.Count;
    }    

    /* SEARCH BARCODE */
    protected void SearchChanged(object sender, EventArgs e)
    {
        string barcode = txtSearchBarcode.Text;
    }

    /* BUTTONS */
    protected void btnProceed_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";

        string precinct = ddlPrecinct.SelectedValue;
        string block = ddlBlock.SelectedValue;
        string barcode = txtSearchBarcode.Text;

        DataTable dt = new DataTable();

        using (OracleConnection con = new OracleConnection(connStr))
        {
            /* SEARCH BY BARCODE */
            if (!string.IsNullOrEmpty(barcode))
            {
                string queryBarcode = @"
                    SELECT 
                        BARCODE,RESNAME,ADDRESS, PRCNT_NM, BLOCK_NM,
                        MBIL_AMT,EBIL_AMT,WBIL_AMT,GBIL_AMT,RBIL_AMT,BBIL_AMT,
                        MBIL_AMT_REC,EBIL_AMT_REC,WBIL_AMT_REC,GBIL_AMT_REC,RBIL_AMT_REC,BBIL_AMT_REC,
                        TBIL_AMT,TBIL_AMT_REC,TBIL_AMT_DIF, METERNO
                    FROM DCRC
                    WHERE SUBSTR(METERNO, 1, 5) NOT IN ('30920', '30930') 
                        AND TBIL_AMT_DIF < 0 
                        AND NVL(IS_DC , 0) = 0 
                        AND BARCODE = :barcode";

                queryBarcode += " ORDER BY TBIL_AMT_DIF";

                using (OracleCommand cmdBarcodeSearch = new OracleCommand(queryBarcode, con))
                {
                    cmdBarcodeSearch.BindByName = true;

                    if (!string.IsNullOrEmpty(barcode))
                        cmdBarcodeSearch.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;

                    using (OracleDataAdapter da = new OracleDataAdapter(cmdBarcodeSearch))
                    {
                        da.Fill(dt);
                    }
                }

                // ✅ VALIDATION HERE
                if (dt.Rows.Count == 0)
                {
                    lblMessage.Text = "Barcode does not exist.";
                    lblMessage.Visible = true;

                    gvBillBreakup.DataSource = null;
                    gvBillBreakup.DataBind();
                }
                else
                {
                    lblMessage.Visible = false;
                }
            }

            /* SEARCH BY DROP DOWNS */
            string query = @"
                    SELECT 
                        BARCODE,RESNAME,ADDRESS, PRCNT_NM, BLOCK_NM,
                        MBIL_AMT,EBIL_AMT,WBIL_AMT,GBIL_AMT,RBIL_AMT,BBIL_AMT,
                        MBIL_AMT_REC,EBIL_AMT_REC,WBIL_AMT_REC,GBIL_AMT_REC,RBIL_AMT_REC,BBIL_AMT_REC,
                        TBIL_AMT,TBIL_AMT_REC,TBIL_AMT_DIF, METERNO                        
                    FROM DCRC
                    WHERE SUBSTR(METERNO, 1, 5) NOT IN ('30920', '30930') AND TBIL_AMT_DIF < 0 AND NVL(IS_DC , 0) = 0";

            if (!string.IsNullOrEmpty(precinct))
                query += " AND PRCNT_NM=:precinct";

            if (!string.IsNullOrEmpty(block))
                query += " AND BLOCK_NM=:block";

            query += " ORDER BY TBIL_AMT_DIF";

            using (OracleCommand cmd = new OracleCommand(query, con))
            {
                if (!string.IsNullOrEmpty(precinct))
                    cmd.Parameters.Add(":precinct", OracleDbType.Varchar2).Value = precinct;

                if (!string.IsNullOrEmpty(block))
                    cmd.Parameters.Add(":block", OracleDbType.Varchar2).Value = block;                

                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }

        if (dt.Rows.Count == 0)
        {
            lblNoRecord.Text = "No records found for selected filters.";
            lblNoRecord.ForeColor = System.Drawing.Color.Red;
            pnlRecords.Visible = false; // optional: hide records panel
            return;
        }

        Session["DC_records"] = dt;
        Session["DC_index"] = 0;

        pnlRecords.Visible = true;

        pcdShowRecord();

        lblNoRecord.Text = "Records Loaded!";
        lblNoRecord.ForeColor = System.Drawing.Color.Green;
    }

    protected void btnDisconnect_Click(object sender, EventArgs e)
    {
        PerformDisconnect();  // your current DB update + session removal code

        // Redirect to avoid double submit
        // Show the message
        lblStatus.Text = "Disconnected Successfully! (Moving to next record...)";
        lblStatus.ForeColor = System.Drawing.Color.Green;

        // Register a client-side script to redirect after 10 seconds (10000 ms)
        string redirectScript = @"
            setTimeout(function() {
                window.location.href = window.location.href;
            }, 3000);";

        ScriptManager.RegisterStartupScript(this, this.GetType(), "RedirectAfterDelay", redirectScript, true);
    }

    //CHECK DISCONNECTION STATUS BY METER NO.
    protected void btnCheckStatus_Click(object sender, EventArgs e)
    {
        string meterNo = txtCheckMeter.Text.Trim();

        if (string.IsNullOrEmpty(meterNo))
        {
            lblStatusResult.Text = "Please enter a Meter Number.";
            lblStatusResult.ForeColor = System.Drawing.Color.Red;
            pnlStatusResult.Visible = true;
            return;
        }

        using (OracleConnection con = new OracleConnection(connStr))
        {
            string query = @"
            SELECT RESNAME, IS_DC, DC_BY, DC_DT, METERNO, PRCNT_NM, BLOCK_NM
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
                        string resname = dr["RESNAME"].ToString();
                        int isDc = Convert.ToInt32(dr["IS_DC"]);
                        string dcBy = dr["DC_BY"] == DBNull.Value ? "N/A" : dr["DC_BY"].ToString();
                        string dcDt = dr["DC_DT"] == DBNull.Value ? "N/A" : Convert.ToDateTime(dr["DC_DT"]).ToString("dd-MMM-yyyy HH:mm");
                        string precinct = dr["PRCNT_NM"].ToString();
                        string block = dr["BLOCK_NM"].ToString();

                        string status = isDc == 1 ? "DISCONNECTED" : "ACTIVE (Not Disconnected)";
                        System.Drawing.Color statusColor = isDc == 1 ? System.Drawing.Color.Red : System.Drawing.Color.Green;

                        // Build result HTML without string interpolation
                        string resultHtml = string.Format(
                            "<strong>Meter:</strong> {0}<br/><strong>Consumer:</strong> {1}<br/><strong>Precinct / Block:</strong> {2} / {3}<br/><strong>Status:</strong> <span style='color:{4};'>{5}</span><br/>",
                            meterNo, resname, precinct, block,
                            System.Drawing.ColorTranslator.ToHtml(statusColor), status);

                        if (isDc == 1)
                        {
                            resultHtml += string.Format(
                                "<strong>Disconnected By (User ID):</strong> {0}<br/><strong>Disconnected On:</strong> {1}<br/>",
                                dcBy, dcDt);
                        }
                        else
                        {
                            resultHtml += "<em>No disconnection record found for this meter.</em>";
                        }

                        lblStatusResult.Text = resultHtml;
                        lblStatusResult.ForeColor = System.Drawing.Color.Black;
                    }
                    else
                    {
                        lblStatusResult.Text = string.Format("Meter Number '{0}' not found in DCRC table.", meterNo);
                        lblStatusResult.ForeColor = System.Drawing.Color.Red;
                    }
                    pnlStatusResult.Visible = true;
                }
            }
        }
    }

    /* TO AVOID DOUBLE SUBMISSION */
    private void PerformDisconnect()
    {
        using (OracleConnection con = new OracleConnection(connStr))
        {
            con.Open();
            OracleTransaction trans = con.BeginTransaction();

            try
            {
                string barcode = lblBarcode.Text;
                string user = Session["User"].ToString();

                string query = @"
                        UPDATE DCRC
                        SET 
                            IS_DC = :isdc,
                            DC_DT = :dcdt,
                            DC_BY = :dcby
                        WHERE BARCODE = :barcode";

                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Transaction = trans;
                    cmd.BindByName = true;

                    cmd.Parameters.Add(":isdc", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add(":dcdt", OracleDbType.Date).Value = DateTime.Now;
                    cmd.Parameters.Add(":dcby", OracleDbType.Int32).Value = user;
                    cmd.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;

                    cmd.ExecuteNonQuery();
                }                                

                // ✅ COMMIT
                trans.Commit();

                /* --- NEW CHANGES --- */

                /* REMOVE CURRENT RECORD FROM SESSION */
                DataTable dt = (DataTable)Session["DC_records"];
                int index = (int)Session["DC_index"];

                dt.Rows.RemoveAt(index);

                /* HANDLE END OF LIST */
                if (dt.Rows.Count == 0)
                {
                    pnlRecords.Visible = false;
                    lblStatus.Text = "All records completed.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    return;
                }

                if (index >= dt.Rows.Count)
                    index = dt.Rows.Count - 1;

                Session["DC_records"] = dt;
                Session["DC_index"] = index;

                pcdShowRecord();
                /* --- --- */

                lblStatus.Text = "Disconnected Successfully!";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Error: " + ex.Message;
                throw;
            }

        }
    }

    private void UpdateChildTable(OracleConnection con, OracleTransaction trans, string tableName, string barcode)
    {
        string query = "UPDATE " + tableName + " SET IS_DC = 1 WHERE REG_NO = :barcode";

        using (OracleCommand cmd = new OracleCommand(query, con))
        {
            cmd.Transaction = trans;
            cmd.BindByName = true;

            cmd.Parameters.Add("barcode", barcode);

            cmd.ExecuteNonQuery();
        }
    }

    /* PAGINATION */
    // Next 
    protected void btnNext_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";

        DataTable dt = (DataTable)Session["DC_records"];

        int index = (int)Session["DC_index"];

        if (index < dt.Rows.Count - 1)
            index++;

        Session["DC_index"] = index;

        pcdShowRecord();
    }

    // Prev 
    protected void btnPrev_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";

        int index = (int)Session["DC_index"];

        if (index > 0)
            index--;

        Session["DC_index"] = index;

        pcdShowRecord();
    }

    // SWIPE 
    void HandleSwipe()
    {
        string target = Request["__EVENTTARGET"];

        if (target == "SwipeNext")
            btnNext_Click(null, null);

        if (target == "SwipePrev")
            btnPrev_Click(null, null);
    }

    /* Clear Fields and Status */
    protected void btnClear_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";
        lblMessage.Text = "";
        lblNoRecord.Text = "";

        txtSearchBarcode.Text = "";

        ddlPrecinct.Text = "";
        ddlBlock.Text = "";

        Session["DC_records"] = null;
        Session["DC_index"] = null;

        pcdShowRecord(); // will automatically show "No records"
    }


}