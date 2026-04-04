<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header.ascx.cs" Inherits="Components_Header" %>

<div class="header">

    <a href="#" class="logo-link">
        <div class="logo">
            DCRC
        </div>
    </a>    

    <div class="menu" ID="menuID">
        <asp:Label ID="lblMessage" runat="server" Text="Label" CssClass="label" Style="display:none; font-size: 12px;"></asp:Label>

        <asp:LinkButton ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" class="hide-on-mobile">Update</asp:LinkButton>
        <asp:HiddenField ID="hdnPassword" runat="server" />

        <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="../Pages/Login.aspx" Visible="false">Login</asp:HyperLink>        

        <!-- 🔽 Charges GROUP -->
        <div ID="divCharges" runat="server" class="dropdown hide-on-mobile">
            <button type="button" class="dropbtn" onclick="toggleDropdown(this)">Charges ▼</button>
            <div class="dropdown-content">
                <asp:HyperLink ID="lnkRis" runat="server" NavigateUrl="../Pages/RIS.aspx">RIS</asp:HyperLink>
                <asp:HyperLink ID="lnkSecurity" runat="server" NavigateUrl="../Pages/Security.aspx">Security</asp:HyperLink>
                <asp:HyperLink ID="lnkEnforcement" runat="server" NavigateUrl="../Pages/Enforcement.aspx">Enforcement</asp:HyperLink>
            </div>
        </div>

        <!-- 🔽 Receiving GROUP -->
        <div ID="divReceiving" runat="server" class="dropdown hide-on-mobile">
            <button type="button" class="dropbtn" onclick="toggleDropdown(this)">Receiving ▼</button>
            <div class="dropdown-content">
                <asp:HyperLink ID="lnkRisRec" runat="server" NavigateUrl="../Pages/RIS_Rec.aspx">RIS</asp:HyperLink>
                <asp:HyperLink ID="lnkSecurityRec" runat="server" NavigateUrl="../Pages/Security_Rec.aspx">Security</asp:HyperLink>
                <asp:HyperLink ID="lnkEnforcementRec" runat="server" NavigateUrl="../Pages/Enforcement_Rec.aspx">Enforcement</asp:HyperLink>
            </div>
        </div>

        <!-- 🔽 DC / RC GROUP -->
        <div ID="divConnection" runat="server" class="dropdown">
            <button type="button" class="dropbtn" onclick="toggleDropdown(this)">Connection ▼</button>
            <div class="dropdown-content">
                <asp:HyperLink ID="lnkDC" runat="server" NavigateUrl="../Pages/DC.aspx">DC</asp:HyperLink>
                <asp:HyperLink ID="lnkRC" runat="server" NavigateUrl="../Pages/RC.aspx">RC</asp:HyperLink>
            </div>
        </div>
        
        <asp:HyperLink ID="lnkRecords" runat="server" NavigateUrl="../Pages/Record.aspx" class="hide-on-mobile">Records</asp:HyperLink>
        
        <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click">Logout</asp:LinkButton>     
           
    </div>

</div>


<script>
    function toggleDropdown(btn) {
        var dropdown = btn.nextElementSibling;

        // Close all other dropdowns
        document.querySelectorAll(".dropdown-content").forEach(function (el) {
            if (el !== dropdown) {
                el.style.display = "none";
            }
        });

        // Toggle current
        dropdown.style.display = (dropdown.style.display === "block") ? "none" : "block";
    }

    // Close when clicking outside
    document.addEventListener("click", function (e) {
        if (!e.target.closest(".dropdown")) {
            document.querySelectorAll(".dropdown-content").forEach(function (el) {
                el.style.display = "none";
            });
        }
    });

    // Ask password on Update button click
    //function askPassword() {
        //var pwd = prompt("Enter password to continue:");

        //if (pwd === null || pwd.trim() === "") {
            //alert("Password is required.");
            //return false; // stop postback
        //}

        //document.getElementById('<%= hdnPassword.ClientID %>').value = pwd;
        //return true; // allow postback
    //}
</script>
