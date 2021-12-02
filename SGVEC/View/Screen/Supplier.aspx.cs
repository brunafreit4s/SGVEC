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
    public partial class Supplier : System.Web.UI.Page
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

                //EnableComponents(false);

                if (txtCode.Text != "") strCode = txtCode.Text;

                //Atualiza o grid
                gvSupplier.DataSource = dtManip.ExecDtTableStringQuery("CALL PROC_SELECT_FORNEC('" + strCode + "', '" + txtCNPJ.Text.ToString() + "', '" + txtRazao.Text.ToString() + "')");
                gvSupplier.DataBind();

                if (gvSupplier.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há fornecedores com essas informações no sistema!"; }
                else lblError.Visible = false;
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
                gc.strCodSupplier = "0";

                if (txtCode.Text != "") strCode = txtCode.Text;

                //Atualiza o grid
                gvSupplier.DataSource = dtManip.ExecDtTableStringQuery("CALL PROC_SELECT_FORNEC('" + strCode + "', '" + txtCNPJ.Text.ToString() + "', '" + txtRazao.Text.ToString() + "')");
                gvSupplier.DataBind();

                if (gvSupplier.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há Fornecedores com essas informações no sistema!"; }
                else lblError.Visible = false;

                txtCode.Text = ""; txtCNPJ.Text = ""; txtRazao.Text = "";
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnSearchSupplier_Click()
        {
            try
            {
                if (gc.strCodSupplier != "0")
                {
                    cnt = new Connect();
                    cnt.DataBaseConnect();
                    MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_FORNEC('" + gc.strCodSupplier + "', '', '')");

                    if (leitor.Read())
                    {
                        txtCodSupplier.Text = leitor[0].ToString();
                        txtRazaoSupplier.Text = leitor[1].ToString();
                        txtCNPJSupplier.Text = leitor[2].ToString();
                        txtEndecSupplier.Text = leitor[3].ToString();
                        txtNumSupplier.Text = leitor[4].ToString();
                        txtBairroSupplier.Text = leitor[5].ToString();
                        txtCEPSupplier.Text = leitor[6].ToString();
                        txtCidadeSupplier.Text = leitor[7].ToString();
                        txtUFSupplier.Text = leitor[8].ToString();
                        txtNumTelSupplier.Text = leitor[9].ToString();
                    }
                    else { lblError.Text = "Não há Fornecedores com essas informações no sistema!"; }
                }
                else { lblError.Text = "É necessário selecionar um Fornecedor!"; ClearComponents(); }

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
                if (gc.strCodSupplier != "0")
                {
                    cnt = new Connect();
                    cnt.DataBaseConnect();
                    MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_FORNEC('" + gc.strCodSupplier + "', '" + txtCNPJSupplier.Text.ToString() + "', '')");

                    if (leitor != null)
                    {
                        if (leitor.Read())
                        {
                            if (gc.strCodSupplier == leitor[0].ToString() && txtCNPJSupplier.Text == leitor[2].ToString())
                            {
                                btnSendUpdate_Click();
                            }
                            else
                            {
                                lblError.Text = "";
                                lblSucess.Text = "";

                                if (ValidateComponents())
                                {
                                    var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_INSERT_FORNEC('" + txtRazaoSupplier.Text + "', '" + txtCNPJSupplier.Text + "', '" 
                                         + txtEndecSupplier.Text + "', '" + txtNumSupplier.Text + "', '" + txtBairroSupplier.Text + "', '" + txtCEPSupplier.Text + "', '"                                          
                                         + txtCidadeSupplier.Text + "', '" + txtUFSupplier.Text + "', '" + txtNumTelSupplier.Text + "')");

                                    if (objRetorno != null)
                                    {
                                        if (objRetorno == true)
                                        {
                                            lblSucess.Text = "Fornecedor cadastrado com sucesso!";
                                            lblSucess.Visible = true;
                                            ClearComponents();
                                        }
                                        else
                                        {
                                            lblError.Text = "Atenção! Fornecedor não cadastrado, verifique os dados digitados!";
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

                        var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_INSERT_FORNEC('" + txtRazaoSupplier.Text + "', '" + txtCNPJSupplier.Text + "', '"
                                         + txtEndecSupplier.Text + "', '" + txtNumSupplier.Text + "', '" + txtBairroSupplier.Text + "', '" + txtCEPSupplier.Text + "', '"
                                         + txtCidadeSupplier.Text + "', '" + txtUFSupplier.Text + "', '" + txtNumTelSupplier.Text + "')");


                        if (objRetorno == true)
                        {
                            lblSucess.Text = "Fornecedor cadastrado com sucesso!";
                            lblSucess.Visible = true;
                            ClearComponents();
                        }
                        else
                        {
                            lblError.Text = "Atenção! Fornecedor não cadastrado, verifique os dados digitados!";
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
                    var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_UPDATE_FORNEC('" + gc.strCodSupplier + "', '" + txtRazaoSupplier.Text + "', '" + txtCNPJSupplier.Text + "', '"
                                                            + txtEndecSupplier.Text + "', '" + txtNumSupplier.Text + "', '" + txtBairroSupplier.Text + "', '" + txtCEPSupplier.Text + "', '"
                                                            + txtCidadeSupplier.Text + "', '" + txtUFSupplier.Text + "', '" + txtNumTelSupplier.Text + "')");

                    if (objRetorno != null)
                    {
                        if (objRetorno == true)
                        {
                            lblSucess.Text = "Fornecedor alterado com sucesso!";
                            lblSucess.Visible = true;
                            ClearComponents();
                        }
                        else
                        {
                            lblError.Text = "Atenção! O Fornecedor não foi alterado, verifique os dados digitados!";
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

        #region Delete
        protected void btnSendDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (gc.strCodSupplier != "0")
                {
                    if (ValidateComponents())
                    {
                        var objRetorno = dtManip.ExecuteStringQuery("DELETE FROM FORNECEDORES WHERE COD_FORNEC = '" + gc.strCodSupplier + "'");

                        if (objRetorno != null)
                        {
                            if (objRetorno == true)
                            {
                                lblSucess.Text = "Fornecedor excluído com sucesso!";
                                lblSucess.Visible = true;
                                ClearComponents();
                            }
                            else
                            {
                                lblError.Text = "Atenção! O Fornecedor não foi excluído, verifique os dados selecionados!";
                                lblError.Visible = true;
                                ClearComponents();
                            }
                        }
                    }
                    else { lblError.Visible = true; }
                }
                else
                {
                    lblError.Text = "Atenção! É necessário selecionar um Fornecedor.";
                    lblError.Visible = true;
                    ClearComponents();
                }
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
            gc.strCodSupplier = "0"; txtCodSupplier.Enabled = true; txtRazaoSupplier.Text = "";
            txtCNPJSupplier.Text = ""; txtNumTelSupplier.Text = ""; txtEndecSupplier.Text = "";
            txtNumSupplier.Text = ""; txtBairroSupplier.Text = ""; txtCEPSupplier.Text = "";
            txtCidadeSupplier.Text = ""; txtUFSupplier.Text = "";
        }

        private void EnableComponents(bool value)
        {
            txtCodSupplier.Enabled = false; txtRazaoSupplier.Enabled = value; txtCNPJSupplier.Enabled = value;
            txtNumTelSupplier.Enabled = value; txtEndecSupplier.Enabled = value; txtNumSupplier.Enabled = value;
            txtBairroSupplier.Enabled = value; txtCEPSupplier.Enabled = value;
            txtCidadeSupplier.Enabled = value; txtUFSupplier.Enabled = value;
        }
        #endregion

        #region Validate
        private bool ValidateComponents()
        {
            if (txtRazaoSupplier.Text == "") { lblError.Text = ce.ComponentsValidation("Razão Social", gc.MSG_NECESSARIO); return false; }
            else if (txtCNPJSupplier.Text == "") { lblError.Text = ce.ComponentsValidation("CNPJ", gc.MSG_NECESSARIO); return false; }
            else if (txtNumTelSupplier.Text == "") { lblError.Text = ce.ComponentsValidation("Telefone", gc.MSG_NECESSARIO); return false; }
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
        protected void gvSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearComponents();

            gc.strCodSupplier = (sender as LinkButton).CommandArgument; //Código do fornecedor selecionado no grid
            if (gc.strCodSupplier != "0") { btnSearchSupplier_Click(); }
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
            string caminho = AppDomain.CurrentDomain.BaseDirectory + @"\PDF\Supplier.pdf";

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
            titulo.Add("\n\n Fornecedor\n\n");
            doc.Add(titulo);

            Paragraph paragrafo = new Paragraph("", new Font(Font.BOLD, 10));
            string conteudo = "Este arquivo contém uma lista de todos os fonecedores cadastrados no sistema!\n\n\n";
            paragrafo.Alignment = Element.ALIGN_CENTER;
            paragrafo.Add(conteudo);
            doc.Add(paragrafo);

            PdfPTable table = new PdfPTable(6);
            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_FORNEC('" + strCode + "', '" + txtCNPJ.Text.ToString() + "', '" + txtRazao.Text.ToString() + "')");

            table.AddCell("Código");
            table.AddCell("Razão Social");
            table.AddCell("CNPJ");
            table.AddCell("CEP");
            table.AddCell("Cidade");
            table.AddCell("Telefone");

            if (leitor != null)
            {

                while (leitor.Read())
                {
                    table.AddCell(leitor[0].ToString());
                    table.AddCell(leitor[1].ToString());
                    table.AddCell(leitor[2].ToString());
                    table.AddCell(leitor[6].ToString());
                    table.AddCell(leitor[7].ToString());
                    table.AddCell(leitor[9].ToString());
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