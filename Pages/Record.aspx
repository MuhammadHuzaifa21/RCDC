<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Record.aspx.cs" Inherits="Pages_Record" %>

<%@ Register Src="../Components/Header.ascx" TagName="Header" TagPrefix="uc" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>RCDC - Records</title>
    <link href="../CSS/style.css" rel="stylesheet" />

    <style>
        /* CONTAINER */
        .container { display: flex; justify-content: center; padding: 20px; }

        /* FORM PANEL */
        .form-panel {
            width: 100%;
            max-width: 1800px;
            margin: auto;
            overflow: hidden;
            background: white;
            padding: 12px;
            border-radius: 8px;
            flex: 2;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        /* ── TOP BAR ───────────────────────────────────────────── */
        /* TOP BAR CLEAN */
        .top-bar {
            display: flex;
            flex-direction: column;
            gap: 14px;
            padding: 10px;
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.06);
            margin-bottom: 5px;
        }

        /* HEADER */
        .top-header {
            display: flex;
            justify-content: center;
            /*align-items: center;*/
            text-align: center;
        }

        .top-header h2 {
            margin: 0;
            font-size: 22px;
            font-weight: 600;
            color: #2f4050;
        }

        /* FILTER ROWS */
        .filter-row {
            display: flex;
            flex-wrap: wrap;
            justify-content: space-between;
            align-items: flex-start;
            gap: 20px;
            background-color: #f7f9fb;
        }

        /* LEFT SIDE */
        .filters-left {
            display: flex;
            gap: 20px;
            flex-wrap: wrap;
        }

        /* RIGHT SIDE */
        .filters-right {
            margin-left: auto;
            display: flex;
            align-items: flex-start;
            align-self: center;
        }

        /* INPUT */
        .input {
            padding: 8px 12px;
            border: 1px solid #dcdcdc;
            border-radius: 6px;
            min-width: 180px;
            font-size: 13px;
        }

        .input:focus {
            border-color: #2c7be5;
            box-shadow: 0 0 0 2px rgba(44,123,229,0.15);
        }

        /* DATE GROUP */
        .date-group {
            display: flex;
            align-items: center;
            gap: 6px;
            background: #f7f9fb;
            padding: 6px 10px;
            border-radius: 6px;
        }

        .date-group label {
            font-size: 12px;
            font-weight: 600;
            color: #666;
        }

        /* ── LEFT: SEARCH ──────────────────────────────────────── */
        .search-section { display: flex; gap: 10px; align-items: center; }

        .search-box {
            padding: 8px 12px;
            border: 1px solid #ccc;
            border-radius: 5px;
            min-width: 180px;
            font-size: 14px;
            color: #333;
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
            outline: none;
        }

        .search-box:focus { border-color: #2c7be5; box-shadow: 0 0 0 3px rgba(44,123,229,0.15); }

        .btn {
            font-size: 13px;
            font-weight: 400;
            padding: 6px 12px;
            background: #2c7be5;
            border: none;
            color: white;
            border-radius: 5px;
            cursor: pointer;
            transition: background 0.2s ease, transform 0.1s ease;
        }

        .btn:hover { background: #1a6ad4; }
        .btn:active { transform: scale(0.97); } 
        .btnCancel { background: #e8e8e8; color: #555; font-weight: bold; border: 1px solid #333; margin-top: 17px; } 
        .btnCancel:hover { background: #d5d5d5; }

        /* ── CENTER: TITLE ─────────────────────────────────────── */
        .title-section { text-align: center; flex: 1; } 
        .title-section h2 { margin: 0; font-size: 28px; font-weight: 600; color: #2f4050; letter-spacing: -0.3px; }

        /* ── RIGHT: COUNTS ─────────────────────────────────────── */
        .count-section { display: flex; gap: 10px; margin-top: 17px; }

        .count-box {
            background: #2f4050;
            color: white;
            padding: 7px 21px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 14px;
            text-decoration: none;
            display: inline-block;
        }

        .total-count {
            background: #2f4050;
            color: white;
            padding: 7px 14px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 14px;
            display: none;
        }

        /* ── EXPORT BUTTON - SLIDE LEFT TO RIGHT ───────────────── */
        .export-btn {
            position: relative;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            white-space: nowrap;
            cursor: pointer;
            transition: background 0.25s ease, box-shadow 0.25s ease;
        }

        .export-btn:hover { background: #1ab394; box-shadow: 0 3px 10px rgba(26,179,148,0.35); }

        /* DEFAULT TEXT — slides out to the LEFT */
        .export-btn .default-text {
            display: flex;
            align-items: center;
            gap: 4px;
            transition: transform 0.32s cubic-bezier(0.4, 0, 0.2, 1), opacity 0.25s ease;
            transform: translateX(0);
            opacity: 1;
        }

        .export-btn:hover .default-text { transform: translateX(-110%); opacity: 0; }

        /* HOVER TEXT — slides IN from the RIGHT */
        .export-btn .hover-text {
            position: absolute;
            left: 0;
            right: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            font-size: 12px;
            font-weight: 700;
            letter-spacing: 0.08em;
            transform: translateX(110%);
            opacity: 0;
            transition: transform 0.32s cubic-bezier(0.4, 0, 0.2, 1), opacity 0.25s ease;
            pointer-events: none;
        }

        .export-btn:hover .hover-text { transform: translateX(0); opacity: 1; } 
        .export-btn .hover-text svg { width: 13px; height: 13px; flex-shrink: 0; }

        /* ─────────────────── */

        /* GRID */
        .grid-wrapper { width: 100%; max-height: 80vh; overflow-x: auto; overflow-y: auto; }
        .grid-wrapper::-webkit-scrollbar { height: 8px; }
        .grid-wrapper::-webkit-scrollbar-thumb { background: #888; border-radius: 4px; }

        .table-responsive { width: 100%; overflow-x: auto; }

        /* Prevent column squeezing */
        .grid-style { border-collapse: collapse; min-width: 1600px; width: max-content; font-family: Segoe UI, Arial; font-size: 13px; table-layout: auto; } 
        .grid-style th { background-color: #2f4050; color: white; text-align: center; padding: 6px; border: 1px solid #d0d0d0; white-space: nowrap; position: sticky; top: 0; z-index: 2; }
        .grid-style td { padding: 8px; border: 1px solid #d0d0d0; white-space: nowrap; } 
        .grid-style tr:hover { background: #eef5ff; }
        .grid-style td:nth-child(8) { background-color: #fff7cc; /* light yellow */ }         

        .grid-pager { text-align: center; padding: 10px; background: #f3f4f6; position: sticky; bottom: 0; }
        .grid-pager a,
        .grid-pager span { margin: 0 5px; padding: 6px 10px; border-radius: 5px; text-decoration: none; border: 1px solid #ccc; color: #333; }
        .grid-pager span { background: #2f4050; color: white; font-weight: bold; }
        .grid-pager td:nth-child(8) { background: #f3f4f6; color: white; font-weight: 600; }
        .grid-pager tr:hover { background: #f3f4f6; }

        .disconnected-row { background-color: #ffe6e6 !important; }         /* Red - for IS_DC = 1 */        
        .updated-row { background-color: #ffeaa7 !important; }              /* Yellow - for IS_DC = 2 */        
        .adc-row { background-color: #efc46a !important; }                  /* Orange - for IS_DC = 3 */        
        .approved-reconnect-row { background-color: #cffbb8 !important; }   /* Green - for RC_TMP = 1 */

        /* LEGEND BAR */
        .legend-bar { display: flex; flex-wrap: wrap; gap: 15px; padding: 10px 15px; margin-bottom: 5px; background: #f8f9fa; border-radius: 6px; border: 1px solid #e0e0e0; }
        .legend-item { display: flex; align-items: center; font-size: 13px; color: #333; gap: 6px; }
        .legend-color { width: 16px; height: 16px; border-radius: 3px; border: 1px solid #ccc; }

        /* MATCH YOUR ROW COLORS */
        .legend-color.disconnected { background-color: #ffe6e6; }
        .legend-color.updated { background-color: #ffeaa7; }
        .legend-color.adc { background-color: #efc46a; }
        .legend-color.approved { background-color: #cffbb8; }

        .auth-message { display: flex; justify-content: center; align-items: center; height: 2vh; margin-top: 10px; margin-bottom: 0; }
        .auth-message span,
        .auth-message label { font-size: 18px; font-weight: 500; color: #e74c3c; background: #fff5f5; padding: 6px 10px; border: 1px solid #ffcccc; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
    </style>

</head>
<body>
    <form id="form2" runat="server">
        <uc:Header ID="Header1" runat="server" />
        
        <div class="container">

            <div class="form-panel" id="formPanel" runat="server">

                <div class="top-bar">

                    <!-- ROW 1: TITLE + COUNTS -->
                    <div class="top-header">                        
                        <h2>Service Disconnection & Recovery Report</h2>                        
                    </div>                    

                    <!-- ROW 2: DATE FILTERS -->
                   <div class="filter-row">

                        <!-- LEFT SIDE -->
                        <div class="filters-left">

                            <div class="date-group">
                                <!-- Barcode -->
                                <div class="row">
                                    <label>Barcode</label>
                                    <div class="col">
                                        <asp:TextBox ID="txtSearchBarcode" runat="server" CssClass="input"
                                            placeholder="🔍 Barcode" AutoPostBack="true" OnTextChanged="SearchChanged" />
                                    </div>
                                </div>

                                <!-- Precinct -->
                                <div class="row">
                                    <label>Precinct</label>
                                    <div class="col">
                                        <asp:TextBox ID="txtSearchPrecinct" runat="server" CssClass="input"
                                            placeholder="📍 Precinct" AutoPostBack="true" OnTextChanged="SearchChanged" />
                                    </div>
                                </div>
                            </div>

                            <div class="date-group">
                                <!-- DC From -->
                                <div class="row">
                                    <label>DC From</label>
                                    <div class="col">
                                        <asp:TextBox ID="txtDCFrom" runat="server" CssClass="input"
                                            TextMode="Date" AutoPostBack="true" OnTextChanged="SearchChanged" />
                                    </div>
                                </div>

                                <!-- DC To -->
                                <div class="row">
                                    <label>DC To</label>
                                    <div class="col">
                                        <asp:TextBox ID="txtDCTo" runat="server" CssClass="input"
                                            TextMode="Date" AutoPostBack="true" OnTextChanged="SearchChanged" />
                                    </div>
                                </div>

                                <!-- Reset -->
                                <div class="row">
                                    <label></label>
                                    <div class="col">
                                        <asp:Button ID="btnClear" runat="server" Text="Reset"
                                            CssClass="btn btnCancel" OnClick="btnClear_Click" />
                                    </div>
                                </div>
                            </div>

                        </div>

                        <!-- RIGHT SIDE (COUNTS) -->
                        <div class="filters-right">
                            <div class="count-section">

                                <asp:LinkButton ID="btnExportDC" runat="server"
                                    CssClass="count-box export-btn"
                                    OnClick="btnExportDC_Click">
                                    <span class="default-text">
                                        DC: <asp:Label ID="lblDCCount" runat="server" />
                                    </span>
                                    <span class="hover-text">EXPORT</span>
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnExportRC" runat="server"
                                    CssClass="count-box export-btn"
                                    OnClick="btnExportRC_Click">
                                    <span class="default-text">
                                        RC: <asp:Label ID="lblRCCount" runat="server" />
                                    </span>
                                    <span class="hover-text">EXPORT</span>
                                </asp:LinkButton>

                            </div>
                        </div>

                    </div>

                </div>

                <!-- COLOR LEGEND -->
                <div class="legend-bar">
                    <div class="legend-item">
                        <span class="legend-color disconnected"></span> Disconnected
                    </div>
                    <div class="legend-item">
                        <span class="legend-color updated"></span> Illegal Connection
                    </div>
                    <div class="legend-item">
                        <span class="legend-color adc"></span> Already Disconnected
                    </div>
                    <div class="legend-item">
                        <span class="legend-color approved"></span> Reconnection Approved
                    </div>
                </div>

                <%-- GRID --%>
                <div class="grid-wrapper">
                    <div class="table-responsive">
                        <asp:GridView ID="gvRecords" runat="server"
                            CssClass="grid-style"
                            AutoGenerateColumns="false"
                            AllowPaging="true"
                            AllowCustomPaging="true"
                            PageSize="15"
                            PagerStyle-CssClass="grid-pager"
                            DataKeyNames="BARCODE"
                            OnPageIndexChanging="gvRecords_PageIndexChanging"
                            OnRowCreated="gvRecords_RowCreated"
                            OnRowDataBound="gvResults_RowDataBound">

                            <Columns>
                                <asp:BoundField DataField="RN" HeaderText="SR NO" />
                                <asp:BoundField DataField="BARCODE" HeaderText="BARCODE" />
                                <asp:BoundField DataField="RESNAME" HeaderText="NAME" />
                                <asp:BoundField DataField="ADDRESS" HeaderText="ADDRESS" />
                                <asp:BoundField DataField="PRCNT_NM" HeaderText="PRECINCT" />
                                <asp:BoundField DataField="BLOCK_NM" HeaderText="BLOCK" />
                                <asp:BoundField DataField="TBIL_AMT" HeaderText="BILL AMNT" />
                                <asp:BoundField DataField="TBIL_AMT_REC" HeaderText="REC AMNT" />
                                <asp:BoundField DataField="TBIL_AMT_DIF" HeaderText="DIFFERENCE" />

                                <asp:BoundField DataField="MBIL_AMT" HeaderText="MAINT" />
                                <asp:BoundField DataField="EBIL_AMT" HeaderText="ELEC" />
                                <asp:BoundField DataField="WBIL_AMT" HeaderText="WATER" />
                                <asp:BoundField DataField="GBIL_AMT" HeaderText="GAS" />
                                <asp:BoundField DataField="RBIL_AMT" HeaderText="RENT" />
                                <asp:BoundField DataField="BBIL_AMT" HeaderText="BNB" />

                                <asp:BoundField DataField="MBIL_AMT_REC" HeaderText="MAINT" />
                                <asp:BoundField DataField="EBIL_AMT_REC" HeaderText="ELEC" />
                                <asp:BoundField DataField="WBIL_AMT_REC" HeaderText="WATER" />
                                <asp:BoundField DataField="GBIL_AMT_REC" HeaderText="GAS" />
                                <asp:BoundField DataField="RBIL_AMT_REC" HeaderText="RENT" />
                                <asp:BoundField DataField="BBIL_AMT_REC" HeaderText="BNB" />

                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

            </div>

        </div>

        <div class="auth-message">
            <asp:Label ID="lblNoAccess" runat="server" />
        </div>

        <!-- STATUS -->
        <div class="row">
            <div class="col">
                <asp:Label ID="lblStatus" runat="server" />
            </div>
        </div>

    </form>
</body>
</html>
 