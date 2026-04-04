using System;
using System.Data;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

public partial class Pages_RC : System.Web.UI.Page
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
            // Remove Session.
            Session.Remove("RC_records");

            LoadPrecincts();
            LoadBlocks("");
        }

        if (Session["RC_records"] != null)
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

            string query = @"SELECT 
                                DISTINCT PRCNT_NM FROM DCRC 
                            WHERE IS_DC = 1 AND IS_RC = 0 AND RC_TMP = 1 
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

            string query = @"SELECT 
                                DISTINCT BLOCK_NM FROM DCRC
                            WHERE IS_DC = 1 AND IS_RC = 0 AND RC_TMP = 1";

            if (!string.IsNullOrEmpty(precinct))
                query += " AND PRCNT_NM=:precinct";

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

        lblStatus.Text = "";
        lblNoRecord.Text = "";
    }

    /* SHOW RECORD */
    void pcdShowRecord()
    {
        DataTable dt = (DataTable)Session["RC_records"];

        if (dt == null || dt.Rows.Count == 0)
        {
            pnlRecords.Visible = false;
            lblNoRecord.Text = "No records to display. <br /> Please select again.";
            lblNoRecord.ForeColor = System.Drawing.Color.Green;
            return;
        }

        int index = (int)Session["RC_index"];

        // Safety: ensure index is within bounds
        if (index < 0) index = 0;
        if (index >= dt.Rows.Count) index = dt.Rows.Count - 1;

        DataRow row = dt.Rows[index];

        lblBarcode.Text = row["BARCODE"].ToString();
        lblName.Text = row["RESNAME"].ToString();
        lblAddress.Text = row["ADDRESS"].ToString();

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

        lblTotalBill.Text = row["TBIL_AMT"].ToString();
        lblTotalRec.Text = row["TBIL_AMT_REC"].ToString();
        lblUnpaid.Text = row["TBIL_AMT_DIF"].ToString();

        if (Convert.ToDecimal(lblUnpaid.Text) < 0)
            lblUnpaid.ForeColor = System.Drawing.Color.Red;

        lblCounter.Text = (index + 1) + " / " + dt.Rows.Count;
    }

    /* BUTTONS */    
    // Next 
    protected void btnNext_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";

        DataTable dt = (DataTable)Session["RC_records"];

        int index = (int)Session["RC_index"];

        if (index < dt.Rows.Count - 1)
            index++;

        Session["RC_index"] = index;

        pcdShowRecord();
    }
    
    // Prev
    protected void btnPrev_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";

        int index = (int)Session["RC_index"];

        if (index > 0)
            index--;

        Session["RC_index"] = index;

        pcdShowRecord();
    }

    // Swipe
    void HandleSwipe()
    {
        string target = Request["__EVENTTARGET"];

        if (target == "SwipeNext")
            btnNext_Click(null, null);

        if (target == "SwipePrev")
            btnPrev_Click(null, null);
    }
    
    // Proceed
    protected void btnProceed_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";

        string precinct = ddlPrecinct.SelectedValue;
        string block = ddlBlock.SelectedValue;
        //int rcCount = 0;

        DataTable dt = new DataTable();

        using (OracleConnection con = new OracleConnection(connStr))
        {

            string query = @"
                            SELECT 
                                BARCODE,RESNAME,ADDRESS,
                                MBIL_AMT,EBIL_AMT,WBIL_AMT,GBIL_AMT,RBIL_AMT,BBIL_AMT,
                                MBIL_AMT_REC,EBIL_AMT_REC,WBIL_AMT_REC,GBIL_AMT_REC,RBIL_AMT_REC,BBIL_AMT_REC,
                                TBIL_AMT,TBIL_AMT_REC,TBIL_AMT_DIF
                            FROM DCRC
                            WHERE TBIL_AMT_DIF < 0 AND IS_DC = 1 AND IS_RC = 0 AND RC_TMP = 1";

            string query_RC_Count = @"
                                SELECT COUNT(*) 
                                FROM DCRC
                                WHERE TBIL_AMT_DIF < 0 AND IS_DC = 1 AND RC_TMP = 1";

            if (!string.IsNullOrEmpty(precinct))
                query += " AND PRCNT_NM=:precinct";
                query_RC_Count += " AND PRCNT_NM = :precinct";

            if (!string.IsNullOrEmpty(block))
                query += " AND BLOCK_NM=:block";
                query_RC_Count += " AND BLOCK_NM = :block";

            query += " ORDER BY TBIL_AMT_DIF";

            // LOAD RECORDS
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

        /* GET COUNT OF RECONNECTIONS APPROVED */
//        using (OracleConnection con = new OracleConnection(connStr))
//        {
//            con.Open();

//            string query_RC_Count = @"
//                                SELECT COUNT(*) 
//                                FROM DCRC
//                                WHERE TBIL_AMT_DIF < 0 AND IS_DC = 1 AND RC_TMP = 1";

//            if (!string.IsNullOrEmpty(precinct))
//                query_RC_Count += " AND PRCNT_NM = :precinct";

//            if (!string.IsNullOrEmpty(block))
//                query_RC_Count += " AND BLOCK_NM = :block";

//            query_RC_Count += " ORDER BY TBIL_AMT_DIF";

//            // GET COUNT
//            using (OracleCommand cmdCount = new OracleCommand(query_RC_Count, con))
//            {
//                if (!string.IsNullOrEmpty(precinct))
//                    cmdCount.Parameters.Add(":precinct", OracleDbType.Varchar2).Value = precinct;

//                if (!string.IsNullOrEmpty(block))
//                    cmdCount.Parameters.Add(":block", OracleDbType.Varchar2).Value = block;

//                rcCount = Convert.ToInt32(cmdCount.ExecuteScalar());
//            }
//        }

        if (dt.Rows.Count == 0)
        {
            lblNoRecord.Text = "No records found for selected filters.";
            lblNoRecord.ForeColor = System.Drawing.Color.Red;
            pnlRecords.Visible = false; // optional: hide records panel
            return;
        }

        // Show total disconnections
        //lblBelowProceed.Text = rcCount + " Reconnection Approval Found.";
        //lblBelowProceed.ForeColor = System.Drawing.Color.Green;

        Session["RC_records"] = dt;
        Session["RC_index"] = 0;

        pnlRecords.Visible = true;

        pcdShowRecord();
    }

    // Reconnect
    protected void btnReconnect_Click(object sender, EventArgs e)
    {
        PerformReconnect();  // your current DB update + session removal code

        // Redirect to avoid double submit
        // Show the message
        lblStatus.Text = "Reconnected Successfully! (Moving to next record...)";
        lblStatus.ForeColor = System.Drawing.Color.Green;

        // Register a client-side script to redirect after 3 seconds (3000 ms)
        string redirectScript = @"
            setTimeout(function() {
                window.location.href = window.location.href;
            }, 3000);";

        ScriptManager.RegisterStartupScript(this, this.GetType(), "RedirectAfterDelay", redirectScript, true);
    }

    /* TO AVOID DOUBLE SUBMISSION */
    private void PerformReconnect()
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
                            IS_RC = :isrc,
                            RC_DT = :rcdt,
                            RC_BY = :rcby
                        WHERE BARCODE = :barcode";

                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Transaction = trans;
                    cmd.BindByName = true;

                    cmd.Parameters.Add(":isrc", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add(":rcdt", OracleDbType.Date).Value = DateTime.Now;
                    cmd.Parameters.Add(":rcby", OracleDbType.Int32).Value = user;
                    cmd.Parameters.Add(":barcode", OracleDbType.Varchar2).Value = barcode;

                    cmd.ExecuteNonQuery();
                }


                // ✅ 2. GET AMOUNTS FROM DCRC
                int risAmt = 0, secAmt = 0, enfAmt = 0;

                string fetchQuery = @"
                        SELECT 
                            NVL(RIS_AMT,0), NVL(SECURITY_AMT,0), NVL(ENFORCEMENT_AMT,0)
                        FROM DCRC
                        WHERE BARCODE = :barcode";

                using (OracleCommand cmdFetch = new OracleCommand(fetchQuery, con))
                {
                    cmdFetch.Transaction = trans;
                    cmdFetch.BindByName = true;

                    cmdFetch.Parameters.Add("barcode", barcode);

                    using (OracleDataReader dr = cmdFetch.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            risAmt = Convert.ToInt32(dr[0]);
                            secAmt = Convert.ToInt32(dr[1]);
                            enfAmt = Convert.ToInt32(dr[2]);
                        }
                    }
                }

                // ✅ 3. CONDITIONAL UPDATES

                if (risAmt > 0)
                {
                    UpdateChildTable(con, trans, "DCRC_RIS", barcode);
                }

                if (secAmt > 0)
                {
                    UpdateChildTable(con, trans, "DCRC_SECURITY", barcode);
                }

                if (enfAmt > 0)
                {
                    UpdateChildTable(con, trans, "DCRC_ENFORCEMENT", barcode);
                }


                trans.Commit();

                /* --- NEW CHANGES --- */
                /* REMOVE CURRENT RECORD FROM SESSION */

                DataTable dt = Session["RC_records"] as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    pnlRecords.Visible = false;
                    lblNoRecord.Text = "No records found.";
                    return;
                }

                int index = Convert.ToInt32(Session["RC_index"]);

                dt.Rows.RemoveAt(index);

                /* HANDLE END OF LIST - RESET SESSION */
                if (dt.Rows.Count == 0)
                {
                    Session.Remove("RC_records");
                    Session.Remove("RC_index");

                    pnlRecords.Visible = false;
                    lblNoRecord.Text = "All Reconnections Done.";
                    lblNoRecord.ForeColor = System.Drawing.Color.Green;
                    return;
                }

                if (index >= dt.Rows.Count)
                    index = dt.Rows.Count - 1;

                Session["RC_records"] = dt;
                Session["RC_index"] = index;

                pcdShowRecord();
                /* --- --- */

                lblNoRecord.Text = "Reconnected Successfully!";
                lblNoRecord.ForeColor = System.Drawing.Color.Green;
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
        string query = "UPDATE " + tableName + " SET IS_RC = 1 WHERE REG_NO = :barcode";

        using (OracleCommand cmd = new OracleCommand(query, con))
        {
            cmd.Transaction = trans;
            cmd.BindByName = true;

            cmd.Parameters.Add("barcode", barcode);

            cmd.ExecuteNonQuery();
        }
    }

}