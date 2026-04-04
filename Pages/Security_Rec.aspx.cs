using System;
using System.Data;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Web;

public partial class Pages_Security_Rec : System.Web.UI.Page
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
            txtRegNo.Focus();
        }
    }

    // 🔍 MAIN METHOD
    void LoadRecord()
    {
        using (OracleConnection con = new OracleConnection(connStr))
        {
            con.Open();

            string regNo = txtRegNo.Text.Trim();

            // ✅ 1. BASE CHECK
            string checkQuery = "SELECT COUNT(*) FROM DCRC_SECURITY WHERE REG_NO = :REG_NO";

            using (OracleCommand cmdCheck = new OracleCommand(checkQuery, con))
            {
                cmdCheck.Parameters.Add("REG_NO", regNo);

                int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                if (count == 0)
                {
                    lblStatus.Text = "No Security Charges Found!";
                    lblStatus.ForeColor = System.Drawing.Color.Orange;
                    return;
                }
            }

            string query = @"
                    SELECT
                        R.OWNER_NAME, R.OWNER_ADDRESS, 
                        DECODE(R.CATEGORY_ID,1,'COMMERCIAL','RESIDENTIAL') CAT_NM, 
                        (SELECT P.PRECENT_NM FROM PRECENT_MST P WHERE P.PRECENT_ID=R.PRECINCT_ID) PRECENT_NM, 
                        (SELECT B.BLOCK_NM FROM BLOCK_MST B WHERE B.BLOCK_ID=R.BLOCK_ID) BLOCK_NM, 
                        R.STREET_ID
                    FROM PRJ_ARCH.ARCH_FILE_MST R WHERE R.REG_NO = :regNo";

            using (OracleCommand cmd = new OracleCommand(query, con))
            {
                cmd.Parameters.Add("regNo", txtRegNo.Text.Trim());

                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        // ✅ Fill GridView
                        gvData.DataSource = dt;
                        gvData.DataBind();

                        //lblStatus.Text = "Record loaded successfully!";
                        //lblStatus.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        ClearFields();

                        lblStatus.Text = "No record found!";
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }
    }

    // 🔄 AUTO LOAD ON BARCODE SCAN
    protected void txtRegNo_TextChanged(object sender, EventArgs e)
    {
        lblStatus.Text = "";

        LoadRecord();
    }    

    /* BUTTONS */
    protected void btnPost_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";

        try
        {
            if (gvData.Rows.Count == 0)
            {
                lblStatus.Text = "Load record first!";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string regNo = txtRegNo.Text.Trim();
            string remarks = txtRemarks.Text.Trim();
            int amount;

            if (!int.TryParse(txtAmount.Text.Trim(), out amount))
            {
                lblStatus.Text = "Enter valid amount!";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }            

            // 🔹 Get values from GridView (first row)
            GridViewRow row = gvData.Rows[0];

            string ownerName = HttpUtility.HtmlDecode(row.Cells[0].Text);
            string ownerAddress = HttpUtility.HtmlDecode(row.Cells[1].Text);
            string catNm = HttpUtility.HtmlDecode(row.Cells[2].Text);
            string precentNm = HttpUtility.HtmlDecode(row.Cells[3].Text);
            string blockNm = HttpUtility.HtmlDecode(row.Cells[4].Text);
            int streetId = Convert.ToInt32(HttpUtility.HtmlDecode((row.Cells[5].Text)));

            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                using (OracleTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // ✅ 1. DUPLICATE CHECK
                        string checkQuery = "SELECT COUNT(*) FROM DCRC_SECURITY WHERE REG_NO = :REG_NO";

                        using (OracleCommand cmdCheck = new OracleCommand(checkQuery, con))
                        {
                            cmdCheck.Transaction = trans;
                            cmdCheck.Parameters.Add("REG_NO", regNo);

                            int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                            if (count == 0)
                            {
                                lblStatus.Text = "Record Does not exists!";
                                lblStatus.ForeColor = System.Drawing.Color.Red;
                                return;
                            }
                        }

                        string updateRIS = @"
                                UPDATE DCRC_SECURITY
                                SET 
                                    AMT_REC = NVL(AMT_REC,0) + :AMT,
                                    REMARKS = :REMARKS
                                WHERE REG_NO = :REG_NO";

                        using (OracleCommand cmdUpdate = new OracleCommand(updateRIS, con))
                        {
                            cmdUpdate.Transaction = trans;
                            cmdUpdate.BindByName = true;

                            cmdUpdate.Parameters.Add("AMT", OracleDbType.Int32).Value = amount;
                            cmdUpdate.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = remarks;
                            cmdUpdate.Parameters.Add("REG_NO", OracleDbType.Varchar2).Value = regNo;

                            cmdUpdate.ExecuteNonQuery();
                        }

                        // ✅ UPDATE DCRC TABLE
                        string updateDCRC = @"
                                UPDATE DCRC 
                                SET 
                                    SECURITY_AMT_REC = NVL(SECURITY_AMT_REC,0) + :AMT,
                                    TBIL_AMT_REC = NVL(TBIL_AMT_REC, 0) + :AMT,
                                    TBIL_AMT_DIF = (NVL(TBIL_AMT_REC, 0) + :AMT) - NVL(TBIL_AMT, 0)
                                WHERE BARCODE = :REG_NO";

                        using (OracleCommand cmdUpdate = new OracleCommand(updateDCRC, con))
                        {
                            cmdUpdate.Transaction = trans;
                            cmdUpdate.BindByName = true;

                            cmdUpdate.Parameters.Add("AMT", OracleDbType.Int32).Value = amount;
                            cmdUpdate.Parameters.Add("REG_NO", OracleDbType.Varchar2).Value = regNo;

                            int rowsAffected = cmdUpdate.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new Exception("Update failed: Record not found in DCRC.");
                            }
                        }

                        // ✅ COMMIT
                        trans.Commit();

                        lblStatus.Text = "Record posted successfully!";
                        lblStatus.ForeColor = System.Drawing.Color.Green;

                        ClearFields();
                        txtRegNo.Focus();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        lblStatus.Text = "Transaction failed: " + ex.Message;
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }       

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearFields();

        txtRegNo.Focus();
    }

    protected void btnHidden_Click(object sender, EventArgs e)
    {
        LoadRecord();
    }

    // 🧹 CLEAR METHOD
    void ClearFields()
    {
        lblStatus.Text = "";

        txtRegNo.Text = "";
        txtAmount.Text = "";
        txtRemarks.Text = "";

        gvData.DataSource = null;
        gvData.DataBind();
    }

}