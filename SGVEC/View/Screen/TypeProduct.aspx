﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TypeProduct.aspx.cs" Inherits="SGVEC.View.Screen.TypeProduct" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>

    <link href="../../Styles/dashboard.css" rel="stylesheet" />
    <link href="../../Styles/forms.css" rel="stylesheet" />
    <title>SGVEC | Tipo de Produto</title>
</head>
<body>
    <div class="flex-dashboard">
        <aside>
            <div class="sidebar-title">
                <img src="/images/logo.png" alt="SGVEC" />
            </div>
            <div class="menu">
                <ul>
                    <li>
                        <a href="/View/Dashboard">
                            <img src="/Images/Dashboard/casa.png" alt="Ícone de Dashboard pela icons8" />
                            Dashboard
                        </a>
                    </li>
                    <li>
                        <a href="/View/Screen/Storage">
                            <img src="/images/Dashboard/openbox.png" alt="Ícone de Estoque pela icons8" />
                            Estoque
                        </a>
                    </li>
                    <li>
                        <a href="/View/Screen/Supplier">
                            <img src="/images/Dashboard/fornecedor.png" alt="Ícone de Fornecedor pela icons8" />
                            Fornecedor
                        </a>
                    </li>
                    <li>
                        <a href="/View/Screen/Employee">
                            <img src="/images/Dashboard/businessman.png" alt="Ícone de Funcionários pela icons8" />
                            Funcionários
                        </a>
                    </li>
                    <li>
                        <a href="/View/Screen/Product">
                            <img src="/images/Dashboard/codbarras.png" alt="Ícone de Produto pela icons8" />
                            Produtos
                        </a>
                    </li>
                    <li class="selected">
                        <a href="/View/Screen/TypeProduct">
                            <img src="/images/Dashboard/tipoproduto.png" alt="Ícone de Tipo de Produto pela icons8" />
                            Tipo de produto
                        </a>
                    </li>
                    <li>
                        <a>
                            <img src="/images/Dashboard/shopify.png" alt="Ícone de Vendas pela icons8" />
                            Vendas                           
                        </a>
                        <ul class="sub-menu">
                            <li>
                                <a href="/View/Screen/Sales_Insert">Inclusão de Vendas
                                </a>
                            </li>
                            <li>
                                <a href="/View/Screen/Sales">Consulta de Vendas
                                </a>
                            </li>
                        </ul>
                    </li>
                </ul>
                <footer class="footer">
                  <a href="/View/Manual.pdf" class="manual" target="_blank">
                        <img src="/images/Dashboard/info.png" alt="Ícone de Manual de informações pela icons8" />
                        <span>&copy; <%: DateTime.Now.Year %> - SGVEC - Sistema Gerenciador de Vendas e Estoque</span>
                    </a>
                </footer>
            </div>
        </aside>

        <main>
            <header>
                <span>Tipo de Produto</span>
                <a href="/Login" class="logout">
                    <img src="/images/Dashboard/sair.png" alt="Ícone de Sair pela icons8" />
                    sair
                </a>
            </header>

            <div class="conteudo">
                <form id="form1" runat="server">
                    <div class="container shadow bg-white p-3">
                        <div class="row clearfix form-space">
                            <div class="col-md-2">
                                <div class="input-group">
                                    <asp:TextBox ID="txtCode" runat="server" type="text" placeholder="Código" MaxLength="4"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-8">
                                <div class="input-group">
                                    <asp:TextBox ID="txtName" type="text" runat="server" placeholder="Nome" MaxLength="30"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <asp:Button ID="btnSearch" runat="server" Text="Pesquisar" CssClass="btn btn-outline-primary btn-sm" BorderStyle="Solid" OnClick="btnSearch_Click" />
                            </div>
                        </div>
                        <div class="row clearfix form-space">
                            <asp:GridView CssClass="col-md-12" ID="gvTypeProduct" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#E7E7FF" BorderStyle="Double" BorderWidth="2px" CellPadding="5" GridLines="Horizontal">
                                <HeaderStyle BackColor="#4E20A9" ForeColor="#ffffff" />
                                <AlternatingRowStyle BackColor="#b8a6dd" />
                                <Columns>
                                    <asp:BoundField DataField="COD_TIPO_PROD" HeaderText="Código" />
                                    <asp:BoundField DataField="NOME_TIPO_PROD" HeaderText="Nome" />
                                    <asp:TemplateField HeaderText="-">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkSelect" Text="Selecionar" runat="server" CommandArgument='<%# Eval("COD_TIPO_PROD") %>' OnClick="gvTypeProduct_SelectedIndexChanged"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="botoes-forms">
                            <asp:Button ID="btnCreatePDF" runat="server" Text="Gerar Relatório" CssClass="btn btn-outline-secondary" BorderStyle="Solid" OnClick="btnCreatePDF_Click" />

                            <button id="btnSearchTypeProduct" type="button" class="btn btn-outline-primary" data-bs-toggle="modal" data-bs-target="#typeProductModal">Consultar</button>

                            <button id="btnInsertTypeProduct" type="button" class="btn btn-outline-primary" data-bs-toggle="modal" data-bs-target="#typeProductModal">Incluir</button>

                            <button id="btnUpdateTypeProduct" type="button" class="btn btn-outline-primary" data-bs-toggle="modal" data-bs-target="#typeProductModal">Alterar</button>

                            <asp:Button ID="btnDeleteTypeProduct" runat="server" Text="Deletar" CssClass="btn btn-outline-danger" BorderStyle="Solid" OnClick="btnSendDelete_Click" />
                        </div>
                        <div class="row clearfix">
                            <div class="col-md-12">
                                <div id="divAlertDanger" style="display: none" class="alert alert-danger alert-dismissible">
                                    <asp:Label runat="server" ID="lblError"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-12">
                                <div id="divAlertSucess" style="display: none" class="alert alert-success alert-dismissible">
                                    <asp:Label runat="server" ID="lblSucess"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Modal -->
                    <div class="modal fade" id="typeProductModal" tabindex="-1" aria-labelledby="lblTypeProductModal" aria-hidden="true">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title" id="exampleModalLabel">Tipo de Produto</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="row clearfix">
                                        <div class="col-md-2">
                                            Código
                                            <div class="input-group">
                                                <asp:TextBox runat="server" ID="txtCodTpProduct" disabled="true" CssClass="form-control" type="text" MaxLength="5" placeholder="Código"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-10">
                                            Nome
                                            <div class="input-group">
                                                <asp:TextBox runat="server" ID="txtNameTpProduct" CssClass="form-control is-invalid" type="text" MaxLength="50" placeholder="Nome"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="modal-footer">
                                    <div class="col-footer">
                                        <button id="btnClearComponents" type="button" class="btn btn-primary">Limpar</button>
                                    </div>
                                    <div class="col-footer">
                                        <button type="button" class="btn btn-danger" data-bs-dismiss="modal" runat="server">Fechar</button>
                                        <asp:Button ID="btnSave" runat="server" Text="Salvar" CssClass="btn btn-success" BorderStyle="Solid" OnClick="btnSendSave_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
    </main>
    </div>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js"></script>
    <script src="../../Scripts/Screen/TypeProduct.js"></script>
</body>
</html>
