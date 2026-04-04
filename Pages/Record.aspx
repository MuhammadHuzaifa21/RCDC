<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Record.aspx.cs" Inherits="Pages_Record" %> 

<%@ Register Src="../Components/Header.ascx" TagName="Header" TagPrefix="uc" %> 

<!DOCTYPE html> 
<html> 
    <head id="Head1" runat="server"> 
        <meta name="viewport" content="width=device-width, initial-scale=1.0" /> 
        <title>DCRC - Records</title> 
        <link href="../CSS/style.css" rel="stylesheet" /> 
        
        <style>             
            /* CONTAINER */ 
            .container { display: flex; justify-content: center; padding: 40px; } 

            /* FORM PANEL */ 
            .form-panel { width: 100%; max-width: 1800px; margin: auto; overflow: hidden; background: white; padding: 15px; border-radius: 8px; flex: 2; box-shadow: 0 2px 5px rgba(0,0,0,0.1); } 
            
            /* ── TOP BAR ───────────────────────────────────────────── */
            .top-bar { 
                display: flex;
                align-items: center;
                justify-content: space-between;
                flex-wrap: wrap;
                gap: 12px;
                margin-bottom: 15px;
                background: #fff;
                padding: 14px 20px;
                border-radius: 8px;
                box-shadow: 0 1px 4px rgba(0,0,0,0.08);
            }

            /* ── LEFT: SEARCH ──────────────────────────────────────── */
            .search-section {
                display: flex;
                gap: 10px;
                align-items: center;
            }

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

            .search-box:focus {
                border-color: #2c7be5;
                box-shadow: 0 0 0 3px rgba(44,123,229,0.15);
            }

            .btn {
                font-size: 14px;
                font-weight: 400;
                padding: 8px 16px;
                background: #2c7be5;
                border: none;
                color: white;
                border-radius: 5px;
                cursor: pointer;
                transition: background 0.2s ease, transform 0.1s ease;
            }

            .btn:hover  { background: #1a6ad4; }
            .btn:active { transform: scale(0.97); }

            .btnCancel {
                background: #e8e8e8;
                color: #555;
                font-weight: bold;
                border: 1px solid #333;
            }

            .btnCancel:hover { background: #d5d5d5; }

            /* ── CENTER: TITLE ─────────────────────────────────────── */
            .title-section {
                text-align: center;
                flex: 1;
            }

            .title-section h2 {
                margin: 0;
                font-size: 28px;
                font-weight: 600;
                color: #2f4050;
                letter-spacing: -0.3px;
            }

            /* ── RIGHT: COUNTS ─────────────────────────────────────── */
            .count-section {
                display: flex;
                gap: 10px;
                align-items: center;
            }

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
                transition:
                    background 0.25s ease,
                    box-shadow 0.25s ease;
            }

            .export-btn:hover {
                background: #1ab394;
                box-shadow: 0 3px 10px rgba(26,179,148,0.35);
            }

            /* DEFAULT TEXT — slides out to the LEFT */
            .export-btn .default-text {
                display: flex;
                align-items: center;
                gap: 4px;
                transition: transform 0.32s cubic-bezier(0.4, 0, 0.2, 1),
                            opacity   0.25s ease;
                transform: translateX(0);
                opacity: 1;
            }

            .export-btn:hover .default-text {
                transform: translateX(-110%);
                opacity: 0;
            }

            /* HOVER TEXT — slides IN from the RIGHT */
            .export-btn .hover-text {
                position: absolute;
                left: 0; right: 0;
                display: flex;
                align-items: center;
                justify-content: center;
                gap: 6px;
                font-size: 12px;
                font-weight: 700;
                letter-spacing: 0.08em;
                transform: translateX(110%);
                opacity: 0;
                transition: transform 0.32s cubic-bezier(0.4, 0, 0.2, 1),
                            opacity   0.25s ease;
                pointer-events: none;
            }

            .export-btn:hover .hover-text {
                transform: translateX(0);
                opacity: 1;
            }

            .export-btn .hover-text svg {
                width: 13px;
                height: 13px;
                flex-shrink: 0;
            }
            
            /* ─────────────────── */

            /* GRID */ 
            .grid-wrapper { width: 100%; max-height: 80vh; overflow-x: auto; overflow-y: auto; } 
            
            .table-responsive { width: 100%; overflow-x: auto; }
            /* Prevent column squeezing */ 
            .grid-style { border-collapse: collapse; min-width: 1600px; width: max-content; font-family: Segoe UI, Arial; font-size: 13px; table-layout: auto; } 
            .grid-style th { background-color: #2f4050; color: white; text-align: center; padding: 6px; border: 1px solid #d0d0d0; white-space: nowrap; } 
            .grid-style td { padding: 6px; border: 1px solid #d0d0d0; white-space: nowrap; } 
            .grid-style tr:hover { background: #f2f2f2; } 

            .grid-style td:nth-child(9) {
                background-color: #fff7cc; /* light yellow */
            }

            .grid-wrapper::-webkit-scrollbar { height: 8px; }
            .grid-wrapper::-webkit-scrollbar-thumb { background: #888; border-radius: 4px; }
            
            .grid-pager { text-align: center; padding: 10px; background: #f3f4f6; position: sticky; bottom: 0; }
            .grid-pager a,
            .grid-pager span { margin: 0 5px; padding: 6px 10px; border-radius: 5px; text-decoration: none; border: 1px solid #ccc; color: #333; } 
            .grid-pager span { background: #2f4050; color: white; font-weight: bold; }

            .grid-pager td:nth-child(9){ background: #f3f4f6; color: white; }


            /* COLOR */ 
            .disconnected-row { background-color: #ffe6e6 !important; }
            .approved-reconnect-row { background-color: #cffbb8 !important; }

        </style> 

    </head> 
    <body> 
        <form id="form2" runat="server"> 
            <uc:Header ID="Header1" runat="server" /> 

            <div class="container"> 
                <div class="form-panel"> 
                    <div class="top-bar">

                        <!-- LEFT: SEARCH -->
                        <div class="search-section">
                            <asp:TextBox ID="txtSearchBarcode" runat="server" CssClass="search-box" placeholder="Search Barcode..." AutoPostBack="true" OnTextChanged="SearchChanged" />        
                            <asp:TextBox ID="txtSearchPrecinct" runat="server" CssClass="search-box" placeholder="Search Precinct..." AutoPostBack="true" OnTextChanged="SearchChanged" />
                            
                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btnCancel" OnClick="btnClear_Click" />
                        </div>

                        <!-- CENTER: TITLE -->
                        <div class="title-section">
                            <h2>Service Disconnection and Recovery Report</h2>
                        </div>

                        <!-- RIGHT: COUNTS -->
                        <div class="count-section">

                            <a href="#" class="count-box export-btn">
                                <span class="default-text">DC: <asp:Label ID="lblDCCount" runat="server" /></span>
                                <span class="hover-text">
                                    <svg viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M8 1v9M4 7l4 4 4-4M2 14h12" stroke="white" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/>
                                    </svg>
                                    EXPORT
                                </span>
                            </a>

                            <a href="#" class="count-box export-btn">
                                <span class="default-text">RC: <asp:Label ID="lblRCCount" runat="server" /></span>
                                <span class="hover-text">
                                    <svg viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M8 1v9M4 7l4 4 4-4M2 14h12" stroke="white" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/>
                                    </svg>
                                    EXPORT
                                </span>
                            </a>

                            <span class="total-count">
                               Total: <asp:Label ID="lblTotalRecords" runat="server" />
                            </span>

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
                                    <asp:BoundField DataField="BARCODE" HeaderText="BARCODE" /> 
                                    <asp:BoundField DataField="RESNAME" HeaderText="NAME" /> 
                                    <asp:BoundField DataField="ADDRESS" HeaderText="ADDRESS" /> 
                                    <asp:BoundField DataField="RCAT_NM" HeaderText="CATEGORY" /> 
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
                                    <asp:BoundField DataField="RIS_AMT" HeaderText="RIS" /> 
                                    <asp:BoundField DataField="SECURITY_AMT" HeaderText="SEC" /> 
                                    <asp:BoundField DataField="ENFORCEMENT_AMT" HeaderText="ENFOR" /> 
                                                                        
                                    <asp:BoundField DataField="MBIL_AMT_REC" HeaderText="MAINT" /> 
                                    <asp:BoundField DataField="EBIL_AMT_REC" HeaderText="ELEC" /> 
                                    <asp:BoundField DataField="WBIL_AMT_REC" HeaderText="WATER" /> 
                                    <asp:BoundField DataField="GBIL_AMT_REC" HeaderText="GAS" /> 
                                    <asp:BoundField DataField="RBIL_AMT_REC" HeaderText="RENT" /> 
                                    <asp:BoundField DataField="BBIL_AMT_REC" HeaderText="BNB" /> 
                                    <asp:BoundField DataField="RIS_AMT_REC" HeaderText="RIS" /> 
                                    <asp:BoundField DataField="SECURITY_AMT_REC" HeaderText="SEC" /> 
                                    <asp:BoundField DataField="ENFORCEMENT_AMT_REC" HeaderText="ENFOR" />                                
                                </Columns> 
                            </asp:GridView> 

                        </div> 
                    </div> 
                </div> 

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