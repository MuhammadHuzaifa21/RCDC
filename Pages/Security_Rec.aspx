<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Security_Rec.aspx.cs" Inherits="Pages_Security_Rec" %>

<%@ Register Src="../Components/Header.ascx" TagName="Header" TagPrefix="uc" %> 
<%@ Register Src="../Components/Footer.ascx" TagName="Footer" TagPrefix="uc" %> 

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">

    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>DCRC - Security Receiving</title>

    <link href="../CSS/style.css" rel="stylesheet"/>

    <style>

        .form-panel { background:#f5f5f5; padding:20px; margin: auto;}

        .record-card {
            background:white;
            padding:10px;
            border-radius:8px;
            box-shadow:0 2px 6px rgba(0,0,0,0.1);
            max-width:710px;
            margin:auto;
        }

        .label { font-weight:bold; margin: 10px 0; }
        .input { width:100%; padding:2px; min-height: 30px; margin:5px 10px 5px 0; border:1px solid #ccc; border-radius:4px; }

        .input-remarks { 
            width: 100%;
            min-height: 90px; /* ~3 rows */
            padding: 2px;
            margin-top: 6px;
            margin-bottom: 5px;
            border: 1px solid #ccc;
            border-radius: 4px;

            resize: vertical;        /* user can expand */
            white-space: pre-wrap;   /* wrap text */
            word-break: break-word;
            overflow-wrap: break-word;
        }

        .btn {
            font-size:15px;
            font-weight:600;
            width:auto;
            padding:10px;
            background:#2c7be5;
            border:none;
            color:white;
            border-radius:4px;
            cursor:pointer;
            margin-bottom: 10px;
        }

        .btnCancel { background-color: #ee7070; margin-top: 10px; }

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

        /* ROW - COL */
        .row { display: flex; gap: 15px; margin: 10px 0; } 
        .col { flex: 1; }  

        /* GRID CONTAINER */
        .grid-container { width: 100%; overflow-x: scroll; overflow-y: auto; max-height: 250px; border: 1px solid #ddd; border-radius: 6px; margin-top: 10px; }

        /* TABLE */
        .info-table { border-collapse: collapse; width: max-content; min-width: 600px;  }       
        .info-table th { background:#2f4050; color:white; padding:8px; font-size: 13px; white-space: nowrap; }
        .info-table td { padding:8px; border:1px solid #ddd; font-size: 13px; vertical-align: top;  min-height: 50px; }

        /* COLUMN WIDTH CONTROL */
        .info-table th:nth-child(1),
        .info-table td:nth-child(1) { max-width: 90px; /* NAME */ }

        .info-table th:nth-child(2),
        .info-table td:nth-child(2) { max-width: 180px; /* ADDRESS */ }

        .info-table th:nth-child(3),
        .info-table td:nth-child(3),
        .info-table th:nth-child(4),
        .info-table td:nth-child(4),
        .info-table th:nth-child(5),
        .info-table td:nth-child(5),
        .info-table th:nth-child(6),
        .info-table td:nth-child(6) { 
            min-width: 100px;
        }

        /* TEXT WRAP (clean) */
        .info-table td { white-space: normal; word-break: break-word; }

        .grid-container::-webkit-scrollbar {
            width: 12px;
        }

        .grid-container::-webkit-scrollbar-track {
            background: #f1f1f1;
        }

        .grid-container::-webkit-scrollbar-thumb {
            background: #888;
            border-radius: 6px;
        }

        .grid-container::-webkit-scrollbar-thumb:hover {
            background: #555;
        }

        @media (max-width: 768px){

            .record-card{
                padding:15px;
            }

            .row{
                flex-direction: column;
            }

            .input{
                width: 100%;
                margin: 5px 0;
            }

            .btn{
                width: 100%;
                margin-top: 10px;
            }

            .grid-container{
                max-height: 220px;
            }

            .info-table th,
            .info-table td{
                padding: 6px;
                font-size: 12px;
            }
        }
    </style>

</head>

<body>
    <form id="form1" runat="server">

        <uc:Header ID="Header1" runat="server" />

        <div class="form-panel">
            <h2 style="text-align:center;">Security - Receiving</h2>

            <div class="record-card">
                 <!-- TEXTBOXES --> 
                <%-- ROW 1 --%>
                <div class="row"> 
                    <div class="col"> 
                        <asp:Label ID="lblRegNo" runat="server" Text="Reg No" CssClass="label"></asp:Label>
                        <asp:TextBox ID="txtRegNo" runat="server" CssClass="input" AutoPostBack="true" OnTextChanged="txtRegNo_TextChanged" />                        
                    </div>
                    <div class="col">  
                        <asp:Label ID="lblAmount" runat="server" Text="Amount" CssClass="label"></asp:Label>
                        <asp:TextBox ID="txtAmount" runat="server" CssClass="input"></asp:TextBox>
                    </div>
                </div>
                
                <asp:Button ID="btnHidden" runat="server" Style="display:none;" OnClick="btnHidden_Click" /> 
                
                <%-- ROW 3 --%>
                <div class="row"> 
                    <div class="col"> 
                        <asp:Label ID="lblRemarks" runat="server" Text="Remarks" CssClass="label"></asp:Label>
                        <asp:TextBox ID="txtRemarks" runat="server" CssClass="input-remarks" TextMode="MultiLine" Rows="3" />
                    </div> 
                </div>
                
                
                <%-- PROCEED BUTTON --%>
                <asp:Button ID="btnPost" runat="server"
                        Text="Post"
                        CssClass="btn"
                        OnClick="btnPost_Click"/> 
                
                <%-- CANCEL BUTTON --%>
                <asp:Button ID="btnCancel" runat="server"
                        Text="Cancel"
                        CssClass="btn btnCancel"
                        OnClick="btnCancel_Click"/>    


                <%-- GRID --%>
                <div class="grid-container">
                    <asp:GridView 
                            ID="gvData" 
                            CssClass="info-table" 
                            runat="server" 
                            AutoGenerateColumns="false" >
                        <Columns> 
                            <asp:BoundField DataField="OWNER_NAME" HeaderText="NAME" /> 
                            <asp:BoundField DataField="OWNER_ADDRESS" HeaderText="ADDRESS" /> 
                            <asp:BoundField DataField="CAT_NM" HeaderText="CATEGORY" /> 
                            <asp:BoundField DataField="PRECENT_NM" HeaderText="PRECINCT" /> 
                            <asp:BoundField DataField="BLOCK_NM" HeaderText="BLOCK" /> 
                            <asp:BoundField DataField="STREET_ID" HeaderText="STREET" /> 
                        </Columns>

                    </asp:GridView>
                </div>
                                                
                <!-- STATUS --> 
                <div class="row"> 
                    <div class="col"> 
                        <asp:Label ID="lblStatus" runat="server" /> 
                    </div> 
                </div>
            </div>
            
        </div>
                
    </form>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
            var txt = document.getElementById("<%= txtRegNo.ClientID %>");

        txt.addEventListener("keypress", function (e) {
            if (e.key === "Enter") {
                e.preventDefault();

                // force postback even if value same
                __doPostBack('<%= btnHidden.UniqueID %>', '');
            }
        });
        });
    </script>
</body>
</html>