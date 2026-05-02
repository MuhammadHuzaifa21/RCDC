using System;
using System.Web;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;


public partial class Pages_Record : System.Web.UI.Page
{
    string connStrMNT = WebConfigurationManager
                        .ConnectionStrings["MyDbConnectionMNT"]
                        .ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["User"] == null)
        {
            Response.Redirect("~/Pages/Login.aspx");
        }

        if (!IsPostBack)
        {
            bool allowed = HasFormAccess();

            if (allowed)
            {
                lblNoAccess.Visible = false;

                LoadGrid(1);
                gvRecords.Visible = true;
            }
            else
            {
                formPanel.Visible = false;
                gvRecords.Visible = false;

                txtSearchBarcode.Visible = false;
                txtSearchPrecinct.Visible = false;
                btnClear.Visible = false;
            }
        }
    }

    private bool HasFormAccess()
    {
        int userId;

        if (Session["User"] == null)
        {
            lblNoAccess.Text = "Session expired. Please log in again.";
            lblNoAccess.ForeColor = System.Drawing.Color.Red;
            lblNoAccess.Visible = true;
            return false;
        }

        if (!int.TryParse(Session["User"].ToString(), out userId))
        {
            lblNoAccess.Text = "Invalid session user.";
            lblNoAccess.ForeColor = System.Drawing.Color.Red;
            lblNoAccess.Visible = true;
            return false;
        }

        // ✅ Allowed users list
        int[] allowedUsers = { 1, 17101 };

        if (!allowedUsers.Contains(userId))
        {
            lblNoAccess.Text = "You are not authorized to access this page.";
            lblNoAccess.ForeColor = System.Drawing.Color.Red;
            lblNoAccess.Visible = true;
            return false;
        }

        lblNoAccess.Visible = false;
        return true;
    }

    protected void SearchChanged(object sender, EventArgs e)
    {
        gvRecords.PageIndex = 0;
        LoadGrid(1);
    }

    /* GRIDS - LOAD DATA IN GRIDS */
    private void LoadGrid(int pageIndex = 1)
    {
        try
        {
            int pageSize = 15;

            int startRow = ((pageIndex - 1) * pageSize) + 1;
            int endRow = pageIndex * pageSize;

            int totalRows;

            string barcodeFilter = txtSearchBarcode.Text.Trim();
            string precinctFilter = txtSearchPrecinct.Text.Trim();

            /* For Date Filter */
            string dcFrom = txtDCFrom.Text.Trim();
            string dcTo = txtDCTo.Text.Trim();

            //string rcFrom = txtRCFrom.Text.Trim();
            //string rcTo = txtRCTo.Text.Trim();

            using (OracleConnection con = new OracleConnection(connStrMNT)) 
            {
                con.Open();

                // COUNT: DC / RC
                OracleCommand countStats = new OracleCommand(@"
                        SELECT 
                            SUM(CASE WHEN IS_DC = 1 OR IS_DC = 2 OR IS_DC = 3 THEN 1 ELSE 0 END) AS DC_COUNT,
                            SUM(CASE WHEN IS_RC = 1 THEN 1 ELSE 0 END) AS RC_COUNT
                        FROM DCRC", con);

                using (OracleDataReader dr = countStats.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lblDCCount.Text = dr["DC_COUNT"].ToString();
                        lblRCCount.Text = dr["RC_COUNT"].ToString();
                    }
                }


                //AND (:rcFrom IS NULL OR RC_DT >= TO_DATE(:rcFrom, 'YYYY-MM-DD'))
                //AND (:rcTo IS NULL OR RC_DT <= TO_DATE(:rcTo, 'YYYY-MM-DD'))

                // 1️⃣ Get total count
                using (OracleCommand countCmd = new OracleCommand(@"
                    SELECT 
                        COUNT(*) FROM DCRC 
                    WHERE NVL(TBIL_AMT_DIF, 0) <= NVL(TBIL_AMT, 0)
                        AND NVL(IS_RC , 0) = 0
                        AND TBIL_AMT_DIF != 0
                        AND (:barcode IS NULL OR BARCODE LIKE '%' || :barcode || '%')
                        AND (:precinct IS NULL OR PRCNT_NM LIKE '%' || :precinct || '%')
                        AND (:dcFrom IS NULL OR DC_DT >= TO_DATE(:dcFrom, 'YYYY-MM-DD'))
                        AND (:dcTo IS NULL OR DC_DT < TO_DATE(:dcTo, 'YYYY-MM-DD') + 1)", con))
                {
                    countCmd.BindByName = true;

                    countCmd.Parameters.Add("barcode", string.IsNullOrEmpty(barcodeFilter) ? (object)DBNull.Value : barcodeFilter);
                    countCmd.Parameters.Add("precinct", string.IsNullOrEmpty(precinctFilter) ? (object)DBNull.Value : precinctFilter);
                    countCmd.Parameters.Add("dcFrom", string.IsNullOrEmpty(dcFrom) ? (object)DBNull.Value : dcFrom);
                    countCmd.Parameters.Add("dcTo", string.IsNullOrEmpty(dcTo) ? (object)DBNull.Value : dcTo);

                    //countCmd.Parameters.Add("rcFrom", string.IsNullOrEmpty(rcFrom) ? (object)DBNull.Value : rcFrom);
                    //countCmd.Parameters.Add("rcTo", string.IsNullOrEmpty(rcTo) ? (object)DBNull.Value : rcTo);

                    totalRows = Convert.ToInt32(countCmd.ExecuteScalar());

                    //lblTotalRecords.Text = totalRows.ToString();
                }
                
                //AND (:rcFrom IS NULL OR RC_DT >= TO_DATE(:rcFrom, 'YYYY-MM-DD'))
                //AND (:rcTo IS NULL OR RC_DT <= TO_DATE(:rcTo, 'YYYY-MM-DD'))

                using (OracleCommand cmd = new OracleCommand(@" 
                    SELECT 
                        RN,
                        BARCODE, RESNAME, ADDRESS, PRCNT_NM, BLOCK_NM, TBIL_AMT, TBIL_AMT_REC, TBIL_AMT_DIF,
                        MBIL_AMT, EBIL_AMT, WBIL_AMT, GBIL_AMT, RBIL_AMT, BBIL_AMT,
                        MBIL_AMT_REC, EBIL_AMT_REC, WBIL_AMT_REC, GBIL_AMT_REC,
                        RBIL_AMT_REC, BBIL_AMT_REC,
                        IS_DC, DC_DT, IS_RC, RC_TMP
                    FROM (
                        SELECT 
                            BARCODE, RESNAME, ADDRESS, PRCNT_NM, BLOCK_NM, TBIL_AMT, TBIL_AMT_REC, TBIL_AMT_DIF,
                            MBIL_AMT, EBIL_AMT, WBIL_AMT, GBIL_AMT, RBIL_AMT, BBIL_AMT,
                            MBIL_AMT_REC, EBIL_AMT_REC, WBIL_AMT_REC, GBIL_AMT_REC,
                            RBIL_AMT_REC, BBIL_AMT_REC,
                            IS_DC, DC_DT, IS_RC, RC_TMP,
        
                            ROW_NUMBER() OVER (ORDER BY TBIL_AMT DESC) AS RN

                        FROM DCRC
                        WHERE NVL(TBIL_AMT_DIF, 0) <= NVL(TBIL_AMT, 0)
                            AND NVL(IS_RC, 0) = 0
                            AND TBIL_AMT_DIF != 0
                            AND (:barcode IS NULL OR BARCODE LIKE '%' || :barcode || '%')
                            AND (:precinct IS NULL OR PRCNT_NM LIKE '%' || :precinct || '%')
                            AND (:dcFrom IS NULL OR DC_DT >= TO_DATE(:dcFrom, 'YYYY-MM-DD'))
                            AND (:dcTo IS NULL OR DC_DT < TO_DATE(:dcTo, 'YYYY-MM-DD') + 1)
                    )

                    WHERE RN BETWEEN :StartRow AND :EndRow
                    ORDER BY RN", con))
                {
                    cmd.BindByName = true;

                    cmd.Parameters.Add("barcode", string.IsNullOrEmpty(barcodeFilter) ? (object)DBNull.Value : barcodeFilter);
                    cmd.Parameters.Add("precinct", string.IsNullOrEmpty(precinctFilter) ? (object)DBNull.Value : precinctFilter);
                    cmd.Parameters.Add("dcFrom", string.IsNullOrEmpty(dcFrom) ? (object)DBNull.Value : dcFrom);
                    cmd.Parameters.Add("dcTo", string.IsNullOrEmpty(dcTo) ? (object)DBNull.Value : dcTo);

                    //cmd.Parameters.Add("rcFrom", string.IsNullOrEmpty(rcFrom) ? (object)DBNull.Value : rcFrom);
                    //cmd.Parameters.Add("rcTo", string.IsNullOrEmpty(rcTo) ? (object)DBNull.Value : rcTo);
                    cmd.Parameters.Add("StartRow", startRow);
                    cmd.Parameters.Add("EndRow", endRow);
                    

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvRecords.VirtualItemCount = totalRows; // 🔥 REQUIRED
                        gvRecords.DataSource = dt;
                        gvRecords.DataBind();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error loading data: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void gvRecords_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvRecords.PageIndex = e.NewPageIndex;

        int pageIndex = e.NewPageIndex + 1; // Oracle is 1-based
        LoadGrid(pageIndex);               // ✅ FIXED
    }

    /* GRIDS - CUSTOM HEADERS */
    protected void gvRecords_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            GridView gv = (GridView)sender;

            // FIRST ROW (Main Headers)
            GridViewRow row1 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);

            /* ROW 1 - CLIENT DETAILS (The Main Frozen Header) */
            TableHeaderCell clientHeader = new TableHeaderCell();
            clientHeader.Text = "CLIENT DETAILS";
            clientHeader.ColumnSpan = 10;
            clientHeader.HorizontalAlign = HorizontalAlign.Center;
            row1.Cells.Add(clientHeader);

            gv.Controls[0].Controls.AddAt(0, row1);

            /* ROW 1 - C2 */
            TableHeaderCell bill_amnt = new TableHeaderCell();
            bill_amnt.Text = "BILL AMOUNT";
            bill_amnt.ColumnSpan = 6;
            bill_amnt.HorizontalAlign = HorizontalAlign.Center;
            row1.Cells.Add(bill_amnt);

            gv.Controls[0].Controls.AddAt(0, row1);

            /* ROW 1 - C3 */
            TableHeaderCell bill_amnt_rec = new TableHeaderCell();
            bill_amnt_rec.Text = "BILL AMOUNT RECEIVED";
            bill_amnt_rec.ColumnSpan = 6;
            bill_amnt_rec.HorizontalAlign = HorizontalAlign.Center;
            row1.Cells.Add(bill_amnt_rec);

            gv.Controls[0].Controls.AddAt(0, row1);
        }
    }

    /* GRIDS - HIGHLIGHT FIELDS */
    protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // Get IS_DC Value
            int isDisconnected = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "IS_DC"));

            // Get RC_TMP Value
            int reconnectTemp = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "RC_TMP"));

            if (isDisconnected == 1)
            {
                e.Row.CssClass = "disconnected-row";                                
            }

            if (isDisconnected == 2)
            {
                e.Row.CssClass = "updated-row"; 
            }

            if (isDisconnected == 3)
            {
                e.Row.CssClass = "adc-row";
            }

            if (reconnectTemp == 1)
            {
                e.Row.CssClass = "approved-reconnect-row";
            }
        }
    }    

    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtSearchBarcode.Text = "";
        txtSearchPrecinct.Text = "";
        txtDCFrom.Text = "";
        txtDCTo.Text = "";
        //txtRCFrom.Text = "";
        //txtRCTo.Text = "";

        gvRecords.PageIndex = 0;
        LoadGrid(1);
    }

    protected void btnExportDC_Click(object sender, EventArgs e)
    {
        using (OracleConnection con = new OracleConnection(connStrMNT))
        {
            con.Open();

            string sql = @"
            SELECT * FROM DCRC
            WHERE IS_DC = 1 OR IS_DC = 2 OR IS_DC = 3";

            ExportCsv(sql, "DC.csv", con);
        }
    }

    protected void btnExportRC_Click(object sender, EventArgs e)
    {
        using (OracleConnection con = new OracleConnection(connStrMNT))
        {
            con.Open();

            string sql = @"
            SELECT * FROM DCRC
            WHERE IS_RC = 1";

            ExportCsv(sql, "RC.csv", con);
        }
    }

    /* EXPORT CSV */
    private void ExportCsv(string sql, string fileName, OracleConnection con)
    {
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.BindByName = true;

            using (OracleDataReader dr = cmd.ExecuteReader())
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                // HEADER
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    sb.Append("\"" + dr.GetName(i) + "\"");
                    if (i < dr.FieldCount - 1)
                        sb.Append(",");
                }
                sb.AppendLine();

                // DATA
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string value = dr[i] == DBNull.Value ? "" : dr[i].ToString();
                        value = value.Replace("\"", "\"\"");
                        sb.Append("\"" + value + "\"");

                        if (i < dr.FieldCount - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();
                }

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "text/csv";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);

                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
        }
    }

}