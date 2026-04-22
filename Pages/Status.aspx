<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Status.aspx.cs" Inherits="Pages_Status" %>

<%@ Register Src="../Components/Header.ascx" TagName="Header" TagPrefix="uc" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">

    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>RCDC - Check Status</title>

    <link href="../CSS/style.css" rel="stylesheet" />

    <style>
        /* Mobile Responsive */
        @media only screen and (max-width: 768px) {
            .row { flex-direction: column; gap: 10px !important; }

            .btn-row { flex-direction: column !important; }

            .btn-check-status, .btn-update { width: 100% !important; }

            .info-table th, .info-table td {
                padding: 6px !important;
                font-size: 12px !important;
            }

            .record-card {
                margin: 10px !important;
                padding: 12px !important;
            }
        }

        .form-panel {
            background: #f5f5f5;
            padding: 10px;
            margin: auto;
        }

        .record-card {
            background: white;
            padding: 15px;
            border-radius: 8px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
            max-width: 700px;
            margin: auto;
        }

        .label {
            font-weight: bold;
        }

        .row {
            display: flex;
            gap: 20px;
            margin: 10px 0 5px 0;
        }

        .col {
            flex: 1;
        }

        .btn-row {
            display: flex;
            gap: 20px;
            margin-top: 15px;
        }

        .btn-check-status {
            font-size: 15px;
            font-weight: 600;
            padding: 10px;
            background: #2c7be5;
            border: none;
            color: white;
            border-radius: 4px;
            cursor: pointer;
            flex: 1;
            transition: background 0.2s ease;
        }

        .btn-check-status:hover {
            background: #1a68d1;
        }

        .btn-update {
            background-color: #ffc107;
            color: #856404;
            font-size: 15px;
            font-weight: 600;
            padding: 10px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            flex: 1;
            transition: background 0.2s ease;
        }

        .btn-update:hover {
            background-color: #e0a800;
        }

        .btn-update:disabled {
            background-color: #e9ecef;
            color: #6c757d;
            cursor: not-allowed;
        }

        .input-check-status {
            width: 100%;
            padding: 8px;
            margin-top: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
        }

        .input-check-status:focus {
            border-color: #2c7be5;
            box-shadow: 0 0 0 3px rgba(44,123,229,0.15);
        }

        /* Info Table - light background for headers */
        .info-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }

        .info-table th {
            background: #f8f9fa;
            color: #333;
            padding: 10px;
            border: 1px solid #ddd;
            font-weight: 600;
            text-align: left;
        }

        .info-table td {
            padding: 10px;
            border: 1px solid #ddd;
            font-size: 14px;
        }

        .result-panel {
            margin-top: 15px;
            padding: 15px;
            background: #e9ecef;
            border-radius: 6px;
            border-left: 4px solid #2c7be5;
        }

        .error-message {
            color: #721c24;
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            padding: 12px;
            border-radius: 4px;
        }

        .success-message {
            color: #155724;
            background-color: #d4edda;
            border: 1px solid #c3e6cb;
            padding: 12px;
            border-radius: 4px;
        }

        .warning-message {
            color: #856404;
            background-color: #fff3cd;
            border: 1px solid #ffeeba;
            padding: 12px;
            border-radius: 4px;
        }

        .status-badge {
            display: inline-block;
            padding: 3px 8px;
            border-radius: 3px;
            font-weight: bold;
        }

        .status-active {
            background: #d4edda;
            color: #155724;
        }

        .status-disconnected {
            background: #f8d7da;
            color: #721c24;
        }
    </style>

</head>

<body>
    <form id="form1" runat="server">

        <uc:Header ID="Header1" runat="server" />

        <div class="form-panel">
            <div class="record-card">
                <h3 style="margin-top: 20px; color: black; text-align: center;">Check Disconnection Statuss</h3>

                <div class="row">
                    <div class="col">
                        <asp:Label ID="lblCheckMeter" runat="server" Text="Enter Meter Number" CssClass="label" />
                        <asp:TextBox ID="txtCheckMeter" runat="server" CssClass="input-check-status" placeholder="Enter Meter Number" />
                    </div>
                </div>

                <div class="btn-row">
                    <asp:Button ID="btnCheckStatus" runat="server" Text="Check Status" CssClass="btn-check-status" OnClick="btnCheckStatus_Click" />
                    <asp:Button ID="btnUpdateDisconnection" runat="server"
                        Text="Update Disconnection"
                        CssClass="btn-update"
                        OnClick="btnUpdateDisconnection_Click"
                        Enabled="false" />
                </div>

                <!-- STATUS RESULT SECTION -->
                <asp:Panel ID="pnlStatusResult" runat="server" Visible="false">
                    <div class="result-panel">
                        <asp:Label ID="lblStatusResult" runat="server" />
                    </div>
                </asp:Panel>

            </div>
        </div>

    </form>
</body>
</html>
