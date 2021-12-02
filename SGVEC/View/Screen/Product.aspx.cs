using System;
using SGVEC.Models;
using SGVEC.Controller;
using MySql.Data.MySqlClient;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace SGVEC.View.Screen
{
    public partial class Product : System.Web.UI.Page
    {
        private Connect cnt = new Connect();
        private ComponentError ce = new ComponentError();
        private DataManipulation dtManip = new DataManipulation();
        private GeneralComponent gc = new GeneralComponent();
        private string strCode = "0";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                lblSucess.Text = "";

                EnableComponents(false);

                if (txtCode.Text != "") strCode = txtCode.Text;

                txtDtCadProduct.Text = DateTime.Now.ToString("yyyy-MM-dd");

                //Atualiza o grid
                gvProduct.DataSource = dtManip.ExecDtTableStringQuery("CALL PROC_SELECT_PROD('" + strCode + "', '" + txtName.Text.ToString() + "')");
                gvProduct.DataBind();

                if (gvProduct.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há produtos com essas informações no sistema!"; }
                else lblError.Visible = false;

                //Preenche o ComboBox com os cadastros da Tabela - Tipo Produto
                ddlTipoProduct.DataSource = dtManip.ExecDtTableStringQuery("SELECT * FROM TIPO_PRODUTO");
                ddlTipoProduct.DataTextField = "NOME_TIPO_PROD";
                ddlTipoProduct.DataValueField = "COD_TIPO_PROD";
                ddlTipoProduct.DataBind();

                //Preenche o ComboBox com os cadastros da Tabela - Fornecedores
                ddlFornecProduct.DataSource = dtManip.ExecDtTableStringQuery("SELECT * FROM FORNECEDORES");
                ddlFornecProduct.DataTextField = "RAZAO_SOCIAL";
                ddlFornecProduct.DataValueField = "COD_FORNEC";
                ddlFornecProduct.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }

        #region Search
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                gc.strCodProduct = "0";

                if (txtCode.Text != "") strCode = txtCode.Text;

                //Atualiza o grid
                gvProduct.DataSource = dtManip.ExecDtTableStringQuery("CALL PROC_SELECT_PROD('" + strCode + "', '" + txtName.Text.ToString() + "')");
                gvProduct.DataBind();

                if (gvProduct.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há produtos com essas informações no sistema!"; }
                else lblError.Visible = false;

                txtCode.Text = ""; txtName.Text = "";
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnSearchProduct_Click()
        {
            try
            {
                if (gc.strCodProduct != "0")
                {
                    cnt = new Connect();
                    cnt.DataBaseConnect();
                    MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_PROD('" + gc.strCodProduct + "', '')");

                    if (leitor.Read())
                    {
                        txtCodBarrasProduct.Text = leitor[0].ToString();
                        txtNomeProduct.Text = leitor[1].ToString();
                        txtMarcaProduct.Text = leitor[2].ToString();
                        txtPrecoProduct.Text = leitor[3].ToString();
                        txtCustoProduct.Text = leitor[4].ToString();
                        txtDtCadProduct.Text = Convert.ToDateTime(leitor[5].ToString()).ToString("yyyy-MM-dd");
                        txtQuantidadeProduct.Text = leitor[6].ToString();
                        txtDescProduct.Text = leitor[7].ToString();
                        ddlTipoProduct.SelectedValue = leitor[8].ToString();
                        ddlFornecProduct.SelectedValue = leitor[9].ToString();
                    }
                    else { lblError.Text = "Não há produtos com essas informações no sistema!"; }
                }
                else { lblError.Text = "É necessário selecionar um produto!"; ClearComponents(); }

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
        protected void btnSendInsert_Click()
        {
            try
            {
                if (gc.strCodProduct != "0")
                {
                    cnt = new Connect();
                    cnt.DataBaseConnect();
                    MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_PROD('" + gc.strCodProduct + "', '" + txtNomeProduct.Text.ToString() + "')");

                    if (leitor != null)
                    {
                        if (leitor.Read())
                        {
                            if (gc.strCodProduct == leitor[0].ToString() && txtNomeProduct.Text == leitor[1].ToString())
                            {
                                btnSendUpdate_Click();
                            }
                            else
                            {
                                lblError.Text = "";
                                lblSucess.Text = "";

                                if (ValidateComponents())
                                {
                                    var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_INSERT_PROD('" + txtCodBarrasProduct.Text + "', '" + txtNomeProduct.Text + "', '" + txtMarcaProduct.Text + "', '"
                                         + txtPrecoProduct.Text + "', '" + txtCustoProduct.Text + "', '" + Convert.ToDateTime(txtDtCadProduct.Text).ToString("dd/MM/yyyy") + "', '" + txtQuantidadeProduct.Text + "', '"
                                         + txtDescProduct.Text + "', '" + ddlTipoProduct.Text + "', '" + ddlFornecProduct.Text + "')");

                                    if (objRetorno != null)
                                    {
                                        if (objRetorno == true)
                                        {
                                            lblSucess.Text = "Produto cadastrado com sucesso!";
                                            lblSucess.Visible = true;
                                            ClearComponents();
                                        }
                                        else
                                        {
                                            lblError.Text = "Atenção! Produto não cadastrado, verifique os dados digitados!";
                                            lblError.Visible = true;
                                            ClearComponents();
                                        }
                                    }
                                }
                                else { lblError.Visible = true; }
                            }
                        }
                    }
                }
                else
                {
                    if (ValidateComponents())
                    {
                        lblError.Text = "";
                        lblSucess.Text = "";

                        var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_INSERT_PROD('" + txtCodBarrasProduct.Text + "', '" + txtNomeProduct.Text + "', '" + txtMarcaProduct.Text + "', '"
                                         + txtPrecoProduct.Text + "', '" + txtCustoProduct.Text + "', '" + Convert.ToDateTime(txtDtCadProduct.Text).ToString("dd/MM/yyyy") + "', '" + txtQuantidadeProduct.Text + "', '"
                                         + txtDescProduct.Text + "', '" + ddlTipoProduct.Text + "', '" + ddlFornecProduct.Text + "')");

                        if (objRetorno == true)
                        {
                            lblSucess.Text = "Produto cadastrado com sucesso!";
                            lblSucess.Visible = true;
                            ClearComponents();
                        }
                        else
                        {
                            lblError.Text = "Atenção! Produto não cadastrado, verifique os dados digitados!";
                            lblError.Visible = true;
                            ClearComponents();
                        }
                    }
                    else { lblError.Visible = true; }
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

        #region Update
        protected void btnSendUpdate_Click()
        {
            try
            {
                lblError.Text = "";
                lblSucess.Text = "";

                if (ValidateComponents())
                {
                    var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_UPDATE_PROD('" + gc.strCodProduct + "', '" + txtNomeProduct.Text + "', '" + txtMarcaProduct.Text + "', '"
                                         + txtPrecoProduct.Text + "', '" + txtCustoProduct.Text + "', '" + Convert.ToDateTime(txtDtCadProduct.Text).ToString("dd/MM/yyyy") + "', '" + txtQuantidadeProduct.Text + "', '"
                                         + txtDescProduct.Text + "', '" + ddlTipoProduct.Text + "', '" + ddlFornecProduct.Text + "')");

                    if (objRetorno != null)
                    {
                        if (objRetorno == true)
                        {
                            lblSucess.Text = "Produto alterado com sucesso!";
                            lblSucess.Visible = true;
                            ClearComponents();
                        }
                        else
                        {
                            lblError.Text = "Atenção! O Produto não foi alterado, verifique os dados digitados!";
                            lblError.Visible = true;
                            ClearComponents();
                        }
                    }
                }
                else { lblError.Visible = true; }
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
            gc.strCodProduct = "0"; txtCodBarrasProduct.Enabled = true;
            txtCodBarrasProduct.Text = ""; txtNomeProduct.Text = ""; txtMarcaProduct.Text = "";
            txtPrecoProduct.Text = ""; txtCustoProduct.Text = ""; txtDtCadProduct.Text = "";
            txtQuantidadeProduct.Text = ""; txtDescProduct.Text = "";
        }

        private void EnableComponents(bool value)
        {
            txtCodBarrasProduct.Enabled = false; txtNomeProduct.Enabled = value; txtMarcaProduct.Enabled = value;
            txtPrecoProduct.Enabled = value; txtCustoProduct.Enabled = value; txtDtCadProduct.Enabled = value;
            txtQuantidadeProduct.Enabled = value; txtDescProduct.Enabled = value; ddlTipoProduct.Enabled = value;
            ddlFornecProduct.Enabled = value;
        }
        #endregion

        #region Validate
        private bool ValidateComponents()
        {
            if (txtNomeProduct.Text == "") { lblError.Text = ce.ComponentsValidation("Nome do Produto", gc.MSG_NECESSARIO); return false; }
            else if (txtMarcaProduct.Text == "") { lblError.Text = ce.ComponentsValidation("Marca", gc.MSG_NECESSARIO); return false; }
            else if (txtPrecoProduct.Text == "") { lblError.Text = ce.ComponentsValidation("Preço", gc.MSG_NECESSARIO); return false; }
            else if (txtCustoProduct.Text == "") { lblError.Text = ce.ComponentsValidation("Custo", gc.MSG_NECESSARIO); return false; }
            else if (txtDtCadProduct.Text == "") { lblError.Text = ce.ComponentsValidation("Data de Cadastro", gc.MSG_NECESSARIO); return false; }
            else if (txtQuantidadeProduct.Text == "") { lblError.Text = ce.ComponentsValidation("Quantidade", gc.MSG_NECESSARIO); return false; }
            else if (gc.CodEmployee == 1) { lblError.Text = ce.ComponentsValidation("", gc.MSG_SEUPERFIL); return false; } //Atendente
            else if (gc.CodEmployee == 2) { lblError.Text = ce.ComponentsValidation("", gc.MSG_SEUPERFIL); return false; } //Caixa
            else if (gc.CodEmployee == 5) { lblError.Text = ce.ComponentsValidation("", gc.MSG_SEUPERFIL); return false; } //Treinador
            else if (gc.CodEmployee == 6) { lblError.Text = ce.ComponentsValidation("", gc.MSG_SEUPERFIL); return false; } //Técnico de Qualidade         
            //3 -- Gerente de Loja
            //4 -- Gerente de Área

            return true;
        }
        #endregion

        #region SelectedIndex
        protected void gvProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearComponents();

            gc.strCodProduct = (sender as LinkButton).CommandArgument; //Código do produto selecionado no grid
            if (gc.strCodProduct != "0") { btnSearchProduct_Click(); }
        }
        #endregion

        #region btnSave
        protected void btnSendSave_Click(object sender, EventArgs e)
        {
            btnSendInsert_Click();
            ClearComponents();
        }
        #endregion

        #region PDF
        protected void btnCreatePDF_Click(object sender, EventArgs e)
        {
            if (txtCode.Text != "") strCode = txtCode.Text;

            Document doc = new Document(PageSize.A3);
            doc.SetMargins(40, 40, 20, 80);
            doc.AddCreationDate();
            string caminho = AppDomain.CurrentDomain.BaseDirectory + @"\PDF\Product.pdf";

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(caminho, FileMode.Create));

            doc.Open();

            string simg = AppDomain.CurrentDomain.BaseDirectory + @"\Images\logo.png";
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(simg);
            img.Alignment = Element.ALIGN_CENTER;
            img.ScaleAbsolute(100, 80);
            doc.Add(img);

            Paragraph titulo = new Paragraph();
            titulo.Font = new Font(Font.DEFAULTSIZE, 30);
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.Add("\n\n Produtos\n\n");
            doc.Add(titulo);

            Paragraph paragrafo = new Paragraph("", new Font(Font.BOLD, 10));
            string conteudo = "Este arquivo contém uma lista de todos os produtos cadastrados no sistema!\n\n\n";
            paragrafo.Alignment = Element.ALIGN_CENTER;
            paragrafo.Add(conteudo);
            doc.Add(paragrafo);

            PdfPTable table = new PdfPTable(6);
            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_PROD('" + strCode + "', '" + txtName.Text.ToString() + "')");

            table.AddCell("Código");
            table.AddCell("Nome");
            table.AddCell("Marca");
            table.AddCell("Preço");
            table.AddCell("Custo");
            table.AddCell("Data de Cadastro");

            if (leitor != null)
            {

                while (leitor.Read())
                {
                    table.AddCell(leitor[0].ToString());
                    table.AddCell(leitor[1].ToString());
                    table.AddCell(leitor[2].ToString());
                    table.AddCell(leitor[3].ToString());
                    table.AddCell(leitor[4].ToString());
                    table.AddCell(leitor[5].ToString());
                }
            }

            doc.Add(table);
            doc.Close();
            cnt.closeConection();

            System.Diagnostics.Process.Start(caminho); //Starta o pdf
        }
        #endregion
    }
}