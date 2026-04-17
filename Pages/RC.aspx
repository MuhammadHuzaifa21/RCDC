<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RC.aspx.cs" Inherits="Pages_RC" %>

<%@ Register Src="../Components/Header.ascx" TagName="Header" TagPrefix="uc" %> 

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">

    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>DCRC - RC</title>

    <link href="../CSS/style.css" rel="stylesheet"/>

    <style>

        .form-panel{ background:#f5f5f5; padding:20px; margin: auto;}

        .record-card{
            background:white;
            padding:15px;
            border-radius:8px;
            box-shadow:0 2px 6px rgba(0,0,0,0.1);
            max-width:700px;
            margin:auto;
            }

        .label{ font-weight:bold; }
        .input{ width:100%;padding:8px;margin-top:5px;margin-bottom:10px;border:1px solid #ccc;border-radius:4px; }

        /*.btn{
            font-size:15px;
            font-weight:600;
            width:100%;
            padding:10px;
            background:#2c7be5;
            border:none;
            color:white;
            border-radius:4px;
            cursor:pointer;
            }*/

        .grid-style{width:100%;border-collapse:collapse;margin-top:15px;}
        .grid-style th{background:#2f4050;color:white;padding:8px;}
        .grid-style td{padding:8px;border:1px solid #ddd;}

        .nav{text-align:center;margin-top:15px;}

        .nav .button{
            width:20%;
            padding:10px;
            background:#2c7be5;
            border:none;
            color:white;
            border-radius:4px;
            cursor:pointer;
            margin:0 10px;
        }

        /* BUTTONS */
        .btn-section { display: flex; gap: 10px; }

        .btn {
            font-size: 15px;
            font-weight: 600;
            padding: 10px;
            background: #2c7be5;
            border: none;
            color: white;
            border-radius: 4px;
            cursor: pointer;
        }

        /* Specific widths */
        .btn-proceed { flex: 7;   /* 70% */ }

        .btn-clear { background-color: #ee7070; flex: 3;   /* 30% */ }

        .btnRC { background-color: #a4ef86; margin-top: 10px; color: #000; width: 100%;}

        /* ROW - COL */
        .row { display: flex; gap: 20px; margin: 10px 0 5px 0;  }
        .col { flex: 1; }  


        /* TABLE */
        .info-table,
        .totals-table{ width:100%; border-collapse:collapse; margin-top:10px; text-align:center; }

        .info-table th,
        .totals-table th{ background:#2f4050; color:white; padding:8px; }

        .info-table td,
        .totals-table td{ padding:8px; border:1px solid #ddd; font-size: 15px}

        .totals-table{ margin-top:15px; }

        /* COLLAPSIBLE */
        .collapse-header {
            background: #2f4050;
            color: white;
            padding: 10px;
            border-radius: 6px;
            cursor: pointer;
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 10px;
            font-weight: 600;
        }

        .collapse-header:hover { background: #1f2937; }
        .collapse-body { display: none; /* hidden by default */ margin-top: 10px; }

    </style>

</head>

<body>

    <form id="form1" runat="server">

        <uc:Header ID="Header1" runat="server" />

        <div class="form-panel">

            <h2 style="text-align:center;">RECONNECTION</h2>

            <div class="record-card">

                <!-- STATUS --> 
                <div class="row"> 
                    <div class="col"> 
                        <asp:Label ID="lblNoRecord" runat="server" /> 
                    </div> 
                </div>

                <asp:Label ID="lblPrecinct" runat="server" Text="Select Precinct" CssClass="label"/>
                <asp:DropDownList ID="ddlPrecinct" runat="server"
                                    CssClass="input"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlPrecinct_SelectedIndexChanged" />

                <asp:Label ID="lblBlock" runat="server" Text="Select Block" CssClass="label"/>
                <asp:DropDownList ID="ddlBlock" runat="server" CssClass="input"/>                

                <div class="btn-section">
                    <asp:Button ID="btnProceed" runat="server"
                            Text="Proceed"
                            CssClass="btn btn-proceed"
                            OnClick="btnProceed_Click"/>

                    <asp:Button ID="btnClear" runat="server" 
                                Text="Clear" 
                                CssClass="btn btn-clear" 
                                OnClick="btnClear_Click" />
                </div>

                <!-- BELOW PROCEED --> 
                <div class="row"> 
                    <div class="col"> 
                        <asp:Label ID="lblBelowProceed" runat="server" /> 
                    </div> 
                </div>

            </div>

        </div>

        <!-- RECORD VIEWER -->
        <asp:Panel ID="pnlRecords" runat="server" Visible="false">
            <div class="form-panel">

                <div class="record-card" id="recordCard">

                    <table class="info-table">
                        <tr>
                            <th>Name</th>
                            <th>Address</th>
                            <th>Meter No</th>
                            <th>Arrears</th>
                        </tr>

                        <tr>
                            <td><asp:Label ID="lblName" runat="server"/></td>
                            <td><asp:Label ID="lblAddress" runat="server"/></td>
                            <td><asp:Label ID="lblMeterNo" runat="server"/></td>
                            <td><asp:Label ID="lblArrears" runat="server"/></td>
                        </tr>
                    </table>

                    <%-- Hide This Part - Drop Down --%>                     
                    <!-- COLLAPSIBLE HEADER -->
                    <div class="collapse-header" onclick="toggleSection()">
                        <span>📄 View Details</span>
                        <span id="toggleIcon">▼</span>
                    </div>

                    <!-- COLLAPSIBLE CONTENT -->
                    <div id="billSection" class="collapse-body">

                        <table class="info-table">
                            <tr>
                                <th>Barcode</th>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblBarcode" runat="server"/></td>
                            </tr>
                        </table>

                        <asp:GridView ID="gvBillBreakup"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="grid-style">

                            <Columns>
                                <asp:BoundField DataField="BillType" HeaderText="Bill Type"/>
                                <asp:BoundField DataField="BillAmount" HeaderText="Amount"/>
                                <asp:BoundField DataField="ReceivedAmount" HeaderText="Received"/>
                            </Columns>

                        </asp:GridView>  

                    </div>                                    

                    <!-- STATUS --> 
                    <div class="row"> 
                        <div class="col"> 
                            <asp:Label ID="lblStatus" runat="server" /> 
                        </div> 
                    </div>

                    <%-- DC BUTTON --%>
                    <asp:Button ID="btnReconnect"
                                runat="server"
                                Text="Reconnect"
                                OnClick="btnReconnect_Click"
                                CssClass="btn btnRC"/>

                    <%-- PAGINATION --%>
                    <div class="nav">
                        <asp:Button ID="btnPrev"
                                    runat="server"
                                    Text="Previous"
                                    OnClick="btnPrev_Click"
                                    CssClass="button"/>

                        <asp:Label ID="lblCounter" runat="server"/>

                        <asp:Button ID="btnNext"
                                    runat="server"
                                    Text="Next"
                                    OnClick="btnNext_Click"
                                    CssClass="button"/>
                    </div>
                     
                </div>

            </div>
            
        </asp:Panel>        
    </form>

    <%-- SCRIPT --%>
    <script>
        var startX = 0;

        document.getElementById("recordCard").addEventListener("touchstart", function (e) {
            startX = e.changedTouches[0].screenX;
        });

        document.getElementById("recordCard").addEventListener("touchend", function (e) {
            var endX = e.changedTouches[0].screenX;

            if (endX - startX > 80) {
                __doPostBack('SwipePrev', '');
            }

            if (startX - endX > 80) {
                __doPostBack('SwipeNext', '');
            }

        });

        /* HIDDEN DETAILS */
        function toggleSection() {
            var section = document.getElementById("billSection");
            var icon = document.getElementById("toggleIcon");

            if (section.style.display === "none" || section.style.display === "") {
                section.style.display = "block";
                icon.innerHTML = "▲";
            } else {
                section.style.display = "none";
                icon.innerHTML = "▼";
            }
        }
    </script>
</body>
</html>