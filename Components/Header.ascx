<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header.ascx.cs" Inherits="Components_Header" %>

<div class="header">

    <a href="#" class="logo-link">
        <div class="logo">
            RCDC
        </div>
    </a>    

    <div class="menu" ID="menuID">
        <asp:Label ID="lblMessage" runat="server" Text="Label" CssClass="label" Style="display:none; font-size: 12px;"></asp:Label>

        <asp:LinkButton ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" class="hide-on-mobile">Update</asp:LinkButton>
        <asp:HiddenField ID="hdnPassword" runat="server" />

        <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="../Pages/Login.aspx" Visible="false">Login</asp:HyperLink>        

        <!-- 🔽 DC / RC GROUP -->
        <div ID="divConnection" runat="server" class="dropdown">
            <button type="button" class="dropbtn" onclick="toggleDropdown(this)">Connection ▼</button>
            <div class="dropdown-content">
                <asp:HyperLink ID="lnkDC" runat="server" NavigateUrl="../Pages/DC.aspx">DC</asp:HyperLink>
                <asp:HyperLink ID="lnkRC" runat="server" NavigateUrl="../Pages/RC.aspx">RC</asp:HyperLink>
            </div>
        </div>
        
        <asp:HyperLink ID="lnkStatus" runat="server" NavigateUrl="../Pages/Status.aspx" class="hide-on-mobile">Check Status</asp:HyperLink>
        <asp:HyperLink ID="lnkRecords" runat="server" NavigateUrl="../Pages/Record.aspx" class="hide-on-mobile">Records</asp:HyperLink>
                
        <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click">Logout</asp:LinkButton>     
           
    </div>

</div>

<script>
    function toggleDropdown(btn) {
        var dropdown = btn.nextElementSibling;

        // Close all other dropdowns --
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
</script>
