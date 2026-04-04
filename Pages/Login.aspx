<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Pages_Login" %> 

<%@ Register Src="../Components/Header.ascx" TagName="Header" TagPrefix="uc" %> 
<%@ Register Src="../Components/Footer.ascx" TagName="Footer" TagPrefix="uc" %> 

<!DOCTYPE html> 
<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server"> 
        <meta name="viewport" content="width=device-width, initial-scale=1.0" /> 
        <title>Login</title> 
        <link href="../CSS/style.css" rel="stylesheet" /> 

        <style>            
            /* LOGIN */ 
            .loginBody { min-height: calc(100vh - 120px); /* prevents footer overlap */
                          display: flex; justify-content: center; align-items: center; padding: 20px; } 
            .loginBox { width: 100%; max-width: 420px; } 

            /* CARD */ 
            .card { width: 100%; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 4px 14px rgba(0,0,0,0.15); transition:0.3s; } 
            .card:hover { box-shadow: 0 8px 20px 0 rgba(0,0,0,0.2); } 

            /* CONTAINER */
            .container { width: 100%; } 
            .container h2 { text-align: center; margin-bottom: 20px; } 
            .container label{ font-weight:600; }

            /* INPUTS & BUTTONS */ 
            .input { width: 100%; padding: 10px; margin-top: 6px; margin-bottom: 15px; border: 1px solid #ccc; border-radius: 4px; font-size:15px; box-sizing:border-box; } 
            .btn { width: 100%; padding: 10px; background: #2c7be5; border: none; color: white; border-radius: 4px; cursor: pointer; font-size: 16px; } 
            .btn:hover { background: #1a68d1; } 

            /* ERROR MESSAGE */
            #lblMessage { display:block; text-align:center; margin-top:10px; }

            /* -------- TABLET -------- */
            @media (max-width:768px){
                .loginBody{
                    padding:30px 20px;
                    align-items: center;
                }

                .card{
                    width: 80%;
                    padding:25px;
                }
            }

            /* -------- MOBILE -------- */
            @media (max-width:480px){
                .loginBody{
                    display: flex; justify-content: center; align-items: center;
                    padding:20px 15px;
                }

                .card{
                    width: 90%;
                    padding:20px;
                    border-radius:6px;
                }

                .container h2{
                    font-size:22px;
                }

                .input{
                    padding:9px;
                    font-size:14px;
                }

                .btn{
                    padding:10px;
                    font-size:15px;
                }
            }

        </style>

    </head> 
    <body> 
        <form id="form1" runat="server"> 
            <uc:Header ID="Header1" runat="server" /> 

            <div class="loginBody"> 
                <div class="loginBox"> 

                    <div class="card"> 
                        <div class="container"> 
                            <h2>Login</h2> 
                            
                            <label>Login ID</label> 
                            <asp:TextBox ID="txtID" runat="server" CssClass="input" autofocus="autofocus" /> 
                            
                            <label>Password</label> 
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input"/> 
                            
                            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn" OnClick="btnLogin_Click"/> 
                            <br /> <br /> 
                            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label> 

                        </div>
                    </div> 
                </div> 
            </div> 
            
            <%--<uc:Footer ID="Footer1" runat="server"/> --%>

        </form> 
    </body> 
</html>