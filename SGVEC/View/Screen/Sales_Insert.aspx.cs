using System;
using SGVEC.Models;
using SGVEC.Controller;
using MySql.Data.MySqlClient;
using System.Web.UI.WebControls;

namespace SGVEC.View.Screen
{
    public partial class Sales_Insert : System.Web.UI.Page
    {
        private Connect cnt = new Connect();
        private ComponentError ce = new ComponentError();
        private DataManipulation dtManip = new DataManipulation();
        private GeneralComponent gc = new GeneralComponent();
        private int intCodVenda = 0;
        private double inVlTotal = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                lblSucess.Text = "";
                lblError.Visible = false;

                EnableComponents(false);

                //Preenche o ComboBox com os cadastros da Tabela - Funcionário
                ddlFuncSales.DataSource = dtManip.ExecDtTableStringQuery("SELECT * FROM FUNCIONARIO WHERE COD_FUNC = '" + gc.CodEmployeeLog + "'");
                ddlFuncSales.DataTextField = "NOME_FUNC";
                ddlFuncSales.DataValueField = "CPF_FUNC";
                ddlFuncSales.DataBind();

                //Preenche o ComboBox com os cadastros da Tabela - Tipo de Pagamento
                ddlTipoPagSales.DataSource = dtManip.ExecDtTableStringQuery("SELECT * FROM TIPO_PAGAMENTO");
                ddlTipoPagSales.DataTextField = "NOME_TIPO_PAG";
                ddlTipoPagSales.DataValueField = "COD_TIPO_PAG";
                ddlTipoPagSales.DataBind();

                txtDtSales.Text = DateTime.Now.ToString("yyyy-MM-dd");

                cnt = new Connect();
                cnt.DataBaseConnect();
                MySqlDataReader leitor = dtManip.ExecuteDataReader("SELECT COUNT(COD_VENDA) FROM VENDA");

                if (leitor.Read())
                {
                    intCodVenda = (Convert.ToInt32(leitor[0].ToString()) + 1);
                }

                //Atualiza o grid
                gvProducts.DataSource = dtManip.ExecDtTableStringQuery(@"SELECT PV.COD_PROD_VENDA, PV.QUANTIDADE_PROD, PV.VALOR_UNITARIO_PROD, P.NOME_PROD, 
                                                                            LPAD(P.COD_BARRAS, 10,'0') AS COD_BARRAS, PV.FK_COD_VENDA FROM PRODUTO_VENDA AS PV 
                                                                            INNER JOIN PRODUTO AS P ON P.COD_BARRAS = PV.FK_COD_PRODUTO 
                                                                            WHERE FK_COD_VENDA = '" + intCodVenda + "'");
                gvProducts.DataBind();

                if (gvProducts.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há produtos adicionados a esta venda atual no sistema!"; }
                else lblError.Visible = false;

                //Atualiza o valor total da venda
                cnt = new Connect();
                cnt.DataBaseConnect();
                MySqlDataReader leitor2 = dtManip.ExecuteDataReader("SELECT * FROM PRODUTO_VENDA WHERE FK_COD_VENDA = '" + intCodVenda + "'");

                txtTotalSales.Text = "0";
                inVlTotal = 0;

                while (leitor2.Read())
                {
                    inVlTotal = inVlTotal + (Convert.ToInt32(leitor2[1].ToString()) * Convert.ToDouble(leitor2[2].ToString()));
                }

                txtTotalSales.Text = Convert.ToString(inVlTotal);

                cnt.closeConection();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }

        #region Add
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                lblSucess.Text = "";
                lblError.Visible = false;

                if (((txtCodProduct.Text != "") && (txtQuantProduct.Text != "")) || (txtNomeProduct.Text != "" && (txtQuantProduct.Text != "")))
                {
                    cnt = new Connect();
                    cnt.DataBaseConnect();
                    MySqlDataReader leitor = dtManip.ExecuteDataReader("SELECT * FROM PRODUTO WHERE COD_BARRAS = '" + txtCodProduct.Text + "' OR NOME_PROD = '" + txtNomeProduct.Text + "'");

                    if (leitor.Read())
                    {
                        if (Convert.ToInt32(leitor[6].ToString()) > Convert.ToInt32(txtQuantProduct.Text))
                        {
                            var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_INSERT_PRODUTO_VENDA('" + txtQuantProduct.Text + "', '"
                                                                        + leitor[3].ToString().Replace(',', '.') + "', '" + leitor[0].ToString() + "', '" + intCodVenda + "')");

                            if (objRetorno == true)
                            {
                                //Atualiza o grid
                                gvProducts.DataSource = dtManip.ExecDtTableStringQuery(@"SELECT PV.COD_PROD_VENDA, PV.QUANTIDADE_PROD, PV.VALOR_UNITARIO_PROD, P.NOME_PROD, 
                                                                            LPAD(P.COD_BARRAS, 10,'0') AS COD_BARRAS, PV.FK_COD_VENDA FROM PRODUTO_VENDA AS PV 
                                                                            INNER JOIN PRODUTO AS P ON P.COD_BARRAS = PV.FK_COD_PRODUTO 
                                                                            WHERE FK_COD_VENDA = '" + intCodVenda + "'");
                                gvProducts.DataBind();

                                //Atualiza o valor total da venda
                                cnt = new Connect();
                                cnt.DataBaseConnect();
                                MySqlDataReader leitor2 = dtManip.ExecuteDataReader("SELECT * FROM PRODUTO_VENDA WHERE FK_COD_VENDA = '" + intCodVenda + "'");

                                txtTotalSales.Text = "0";
                                inVlTotal = 0;

                                while (leitor2.Read())
                                {
                                    inVlTotal = inVlTotal + (Convert.ToInt32(leitor2[1].ToString()) * Convert.ToDouble(leitor2[2].ToString()));                                    
                                }

                                txtTotalSales.Text = Convert.ToString(inVlTotal);

                                //atualiza a quantidade de produtos na base de dados  
                                int intQntProduct = Convert.ToInt32(leitor[6].ToString());
                                int intVlNewProduct = (intQntProduct - Convert.ToInt32(txtQuantProduct.Text));
                                dtManip.ExecuteStringQuery("UPDATE PRODUTO SET QUANTIDADE_PROD = '" + intVlNewProduct + "' WHERE COD_BARRAS = '" + txtCodProduct.Text + "'");
                            }
                        }
                        else { lblError.Visible = true; lblError.Text = "Atenção! A quantidade de produtos digitada não existe no estoque!"; }
                    }
                    else { lblError.Visible = true; lblError.Text = "Atenção! Produto não encontrado, verifique os dados digitados!"; }
                }
                else { lblError.Visible = true; lblError.Text = "Atenção! É preciso digitar um Código de Barras ou Nome e a Quantidade de Produtos para adicionar na Venda!"; }

                cnt.closeConection();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }
        #endregion

        #region Remove
        protected void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                lblSucess.Text = "";
                lblError.Visible = false;

                if (gc.strCodProductSales != "")
                {
                    //atualiza a quantidade de produtos na base de dados  
                    cnt = new Connect();
                    cnt.DataBaseConnect();
                    MySqlDataReader leitor = dtManip.ExecuteDataReader("SELECT QUANTIDADE_PROD, FK_COD_PRODUTO FROM PRODUTO_VENDA WHERE COD_PROD_VENDA = '" + gc.strCodProductSales + "'");

                    if (leitor.Read())
                    {
                        int intQntProductRemoved = Convert.ToInt32(leitor[0].ToString());
                        int intCodProduct = Convert.ToInt32(leitor[1].ToString());

                        cnt = new Connect();
                        cnt.DataBaseConnect();
                        MySqlDataReader leitor2 = dtManip.ExecuteDataReader("SELECT QUANTIDADE_PROD FROM PRODUTO WHERE COD_BARRAS = '" + intCodProduct + "'");

                        if (leitor2.Read())
                        {
                            int intQntProduct = Convert.ToInt32(leitor2[0].ToString());

                            int intVlNewProduct = (intQntProduct + intQntProductRemoved);
                            dtManip.ExecuteStringQuery("UPDATE PRODUTO SET QUANTIDADE_PROD = '" + intVlNewProduct + "' WHERE COD_BARRAS = '" + intCodProduct + "'");

                            //remove o item selecionado da base de dados
                            var objRetorno = dtManip.ExecuteStringQuery("DELETE FROM PRODUTO_VENDA WHERE COD_PROD_VENDA = '" + gc.strCodProductSales + "'");

                            if (objRetorno == true)
                            {
                                //Atualiza o grid
                                gvProducts.DataSource = dtManip.ExecDtTableStringQuery(@"SELECT PV.COD_PROD_VENDA, PV.QUANTIDADE_PROD, PV.VALOR_UNITARIO_PROD, P.NOME_PROD, 
                                                                            LPAD(P.COD_BARRAS, 10,'0') AS COD_BARRAS, PV.FK_COD_VENDA FROM PRODUTO_VENDA AS PV 
                                                                            INNER JOIN PRODUTO AS P ON P.COD_BARRAS = PV.FK_COD_PRODUTO 
                                                                            WHERE FK_COD_VENDA = '" + intCodVenda + "'");
                                gvProducts.DataBind();

                                //Atualiza o valor total da venda
                                cnt = new Connect();
                                cnt.DataBaseConnect();
                                MySqlDataReader leitor3 = dtManip.ExecuteDataReader("SELECT * FROM PRODUTO_VENDA WHERE FK_COD_VENDA = '" + intCodVenda + "'");

                                txtTotalSales.Text = "0";
                                inVlTotal = 0;

                                while (leitor3.Read())
                                {
                                    inVlTotal = inVlTotal + (Convert.ToInt32(leitor3[1].ToString()) * Convert.ToDouble(leitor3[2].ToString()));
                                }

                                txtTotalSales.Text = Convert.ToString(inVlTotal);
                            }
                        }
                    }
                }

                cnt.closeConection();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }
        #endregion

        #region Insert
        protected void btnSendInsertSales_Click(object sender, EventArgs e)
        {
            try
            {
                int intDesconto = 0;
                int intNumParc = 0;
                double dblValParc = 0, dblTotal = 0;
                lblError.Text = "";
                lblSucess.Text = "";
                lblError.Visible = false;

                if (txtTotalSales.Text != "")
                {
                    dblTotal = Double.Parse(txtTotalSales.Text);
                }

                if (txtDescontoSales.Text != "") {
                    intDesconto = Convert.ToInt32(txtDescontoSales.Text.Replace("%", ""));
                    dblTotal = (((Double.Parse(txtTotalSales.Text) / 100) * intDesconto));
                    dblTotal = (Double.Parse(txtTotalSales.Text) - dblTotal);
                }                

                if (txtNumParcSales.Text != "") {
                    intNumParc = Convert.ToInt32(txtNumParcSales.Text);
                    dblValParc = (dblTotal / intNumParc);
                }                

                if (ValidateComponents())
                {
                    var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_INSERT_SALE('" + intCodVenda + "', '" + txtNomeCliSales.Text + "', '"
                         + txtCpfCliSales.Text + "', '" + Convert.ToDateTime(txtDtSales.Text).ToString("dd/MM/yyyy") + "', '" + intNumParc + "', '"
                         + dblValParc.ToString().Replace(',', '.') + "', '" + intDesconto.ToString().Replace(',', '.') + "', '" + dblTotal.ToString().Replace(',', '.') 
                         + "', '" + ddlFuncSales.SelectedItem.Value + "', '" + ddlTipoPagSales.SelectedItem.Value + "')");

                    if (objRetorno != null)
                    {
                        if (objRetorno == true)
                        {
                            cnt = new Connect();
                            cnt.DataBaseConnect();
                            MySqlDataReader leitor = dtManip.ExecuteDataReader("SELECT COUNT(COD_VENDA) FROM VENDA");

                            if (leitor.Read())
                            {
                                intCodVenda = (Convert.ToInt32(leitor[0].ToString()) + 1);
                            }

                            //Atualiza o grid
                            gvProducts.DataSource = dtManip.ExecDtTableStringQuery(@"SELECT PV.COD_PROD_VENDA, PV.QUANTIDADE_PROD, PV.VALOR_UNITARIO_PROD, P.NOME_PROD, 
                                                                            LPAD(P.COD_BARRAS, 10,'0') AS COD_BARRAS, PV.FK_COD_VENDA FROM PRODUTO_VENDA AS PV 
                                                                            INNER JOIN PRODUTO AS P ON P.COD_BARRAS = PV.FK_COD_PRODUTO 
                                                                            WHERE FK_COD_VENDA = '" + intCodVenda + "'");
                            gvProducts.DataBind();

                            lblSucess.Text = "Venda registrada com sucesso!";
                            lblSucess.Visible = true;
                            ClearComponents();
                        }
                        else
                        {
                            lblError.Text = "Atenção! Venda não registrada, verifique os dados digitados!";
                            lblError.Visible = true;
                            ClearComponents();
                        }
                    }
                }
                else { lblError.Visible = true; }

                cnt.closeConection();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }
        #endregion

        protected void btnClearComponents_Click(object sender, EventArgs e)
        {
            ClearComponents();
        }

        #region Components
        private void ClearComponents()
        {
            txtNomeCliSales.Text = ""; txtCpfCliSales.Text = ""; txtDtSales.Text = "";
            txtNumParcSales.Text = ""; txtValParcSales.Text = ""; txtDescontoSales.Text = ""; txtTotalSales.Text = "";
        }

        private void EnableComponents(bool value)
        {
            txtNomeCliSales.Enabled = value; txtCpfCliSales.Enabled = value;
            ddlFuncSales.Enabled = value; txtDtSales.Enabled = value; txtNumParcSales.Enabled = value;
            txtValParcSales.Enabled = value; txtDescontoSales.Enabled = value; txtTotalSales.Enabled = value; ddlTipoPagSales.Enabled = value;
        }
        #endregion

        #region Validate
        private bool ValidateComponents()
        {
            if (gvProducts.Rows.Count == 0) { lblError.Text = ce.ComponentsValidation("Atenção!", "Deve-se adicionar pelo um produto na venda!"); return false; }
            else if (gc.CodEmployee == 5) { lblError.Text = ce.ComponentsValidation("", gc.MSG_SEUPERFIL); return false; } //Treinador
            else if (gc.CodEmployee == 6) { lblError.Text = ce.ComponentsValidation("", gc.MSG_SEUPERFIL); return false; } //Técnico de Qualidade         
            //1 -- Atendente
            //2 -- Caixa
            //3 -- Gerente de Loja
            //4 -- Gerente de Área

            return true;
        }
        #endregion

        #region SelectedIndex
        protected void gvProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            gc.strCodProductSales = (sender as LinkButton).CommandArgument; //Código do Produto/Venda selecionado no grid
        }
        #endregion       
    }
}