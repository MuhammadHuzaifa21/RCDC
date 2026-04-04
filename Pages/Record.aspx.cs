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
    string connStr = WebConfigurationManager
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
            LoadGrid(1);
        }
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

            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                // COUNT: DC / RC
                OracleCommand countStats = new OracleCommand(@"
                        SELECT 
                            SUM(CASE WHEN IS_DC = 1 THEN 1 ELSE 0 END) AS DC_COUNT,
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

                // 1️⃣ Get total count
                using (OracleCommand countCmd = new OracleCommand(@"
                    SELECT 
                        COUNT(*) FROM DCRC 
                    WHERE NVL(TBIL_AMT_DIF, 0) <= NVL(TBIL_AMT, 0)
                        AND NVL(IS_RC , 0) = 0
                        AND TBIL_AMT_DIF != 0
                        AND (:barcode IS NULL OR BARCODE LIKE '%' || :barcode || '%')
                        AND (:precinct IS NULL OR PRCNT_NM LIKE '%' || :precinct || '%')", con))
                {
                    countCmd.BindByName = true;

                    countCmd.Parameters.Add("barcode", string.IsNullOrEmpty(barcodeFilter) ? (object)DBNull.Value : barcodeFilter);
                    countCmd.Parameters.Add("precinct", string.IsNullOrEmpty(precinctFilter) ? (object)DBNull.Value : precinctFilter);

                    totalRows = Convert.ToInt32(countCmd.ExecuteScalar());

                    lblTotalRecords.Text = totalRows.ToString();
                }
                

                using (OracleCommand cmd = new OracleCommand(@" 
                    SELECT * FROM (
                        SELECT 
                            BARCODE, RESNAME, ADDRESS, RCAT_NM, PRCNT_NM, BLOCK_NM, TBIL_AMT, TBIL_AMT_REC, TBIL_AMT_DIF,
                            MBIL_AMT, EBIL_AMT, WBIL_AMT, GBIL_AMT, RBIL_AMT, BBIL_AMT,
                            RIS_AMT, SECURITY_AMT, ENFORCEMENT_AMT,
                            MBIL_AMT_REC, EBIL_AMT_REC, WBIL_AMT_REC, GBIL_AMT_REC,
                            RBIL_AMT_REC, BBIL_AMT_REC, RIS_AMT_REC, SECURITY_AMT_REC, ENFORCEMENT_AMT_REC,                            
                            IS_DC, IS_RC, RC_TMP,
        
                            ROW_NUMBER() OVER (ORDER BY TBIL_AMT DESC) AS RN

                        FROM DCRC
                        WHERE NVL(TBIL_AMT_DIF, 0) <= NVL(TBIL_AMT, 0)
                            AND NVL(IS_RC, 0) = 0
                            AND TBIL_AMT_DIF != 0
                            AND (:barcode IS NULL OR BARCODE LIKE '%' || :barcode || '%')
                            AND (:precinct IS NULL OR PRCNT_NM LIKE '%' || :precinct || '%')
                    )
                    WHERE RN BETWEEN :StartRow AND :EndRow
                    ORDER BY RN", con))
                {
                    cmd.BindByName = true;

                    cmd.Parameters.Add("barcode", string.IsNullOrEmpty(barcodeFilter) ? (object)DBNull.Value : barcodeFilter);
                    cmd.Parameters.Add("precinct", string.IsNullOrEmpty(precinctFilter) ? (object)DBNull.Value : precinctFilter);
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
            clientHeader.ColumnSpan = 9;
            clientHeader.HorizontalAlign = HorizontalAlign.Center;
            clientHeader.CssClass = "sticky-col col-barcode"; // Starts at 0px left
            row1.Cells.Add(clientHeader);

            gv.Controls[0].Controls.AddAt(0, row1);

            /* ROW 1 - C2 */
            TableHeaderCell bill_amnt = new TableHeaderCell();
            bill_amnt.Text = "BILL AMOUNT";
            bill_amnt.ColumnSpan = 9;
            bill_amnt.HorizontalAlign = HorizontalAlign.Center;
            row1.Cells.Add(bill_amnt);

            gv.Controls[0].Controls.AddAt(0, row1);

            /* ROW 1 - C3 */
            TableHeaderCell bill_amnt_rec = new TableHeaderCell();
            bill_amnt_rec.Text = "BILL AMOUNT RECEIVED";
            bill_amnt_rec.ColumnSpan = 9;
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
            // 1. APPLY STICKY CLASSES TO THE FIRST 6 CELLS
            string[] stickyClasses = { "col-barcode", "col-name", "col-address", "col-cat", "col-prcnt", "col-block" };

            for (int i = 0; i < 6; i++)
            {
                e.Row.Cells[i].CssClass += " sticky-col " + stickyClasses[i];
            }

            //[cite_start];// 2. KEEP YOUR EXISTING COLOR LOGIC [cite: 20, 21]

            // Get IS_DC Value
            int isDisconnected = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "IS_DC"));

            // Get RC_TMP Value
            int reconnectTemp = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "RC_TMP"));

            if (isDisconnected == 1)
            {
                e.Row.CssClass = "disconnected-row";                                
            }

            if (reconnectTemp == 1)
            {
                e.Row.CssClass = "approved-reconnect-row";
            }

        }

        // Also apply classes to the standard sub-headers (RowType == Header)
        if (e.Row.RowType == DataControlRowType.Header)
        {
            string[] stickyClasses = { "col-barcode", "col-name", "col-address", "col-cat", "col-prcnt", "col-block" };
            for (int i = 0; i < 6; i++)
            {
                e.Row.Cells[i].CssClass += " sticky-col " + stickyClasses[i];
            }
        }
    }
    

    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtSearchBarcode.Text = "";
        txtSearchPrecinct.Text = "";

        gvRecords.PageIndex = 0;
        LoadGrid(1);
    }

    protected void btnExportDC_Click(object sender, EventArgs e)
    {
        using (OracleConnection con = new OracleConnection(connStr))
        {
            con.Open();

            string sql = @"
            SELECT * FROM DCRC
            WHERE IS_DC = 1";

            ExportCsv(sql, "DC.csv", con);
        }
    }

    protected void btnExportRC_Click(object sender, EventArgs e)
    {
        using (OracleConnection con = new OracleConnection(connStr))
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