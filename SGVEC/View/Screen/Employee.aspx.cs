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
    public partial class Employee : System.Web.UI.Page
    {
        private Connect cnt = new Connect();
        private ComponentError ce = new ComponentError();
        private DataManipulation dtManip = new DataManipulation();
        private GeneralComponent gc = new GeneralComponent();
        private string strCode = "0";
        private string strDtDesligamento = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                lblSucess.Text = "";

                EnableComponents(false);

                if (txtCode.Text != "") strCode = txtCode.Text;

                //Atualiza o grid
                gvEmployee.DataSource = dtManip.ExecDtTableStringQuery("CALL PROC_SELECT_FUNC('" + strCode + "', '" + txtCPF.Text.ToString() + "', '" + txtName.Text.ToString() + "')");
                gvEmployee.DataBind();

                if (gvEmployee.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há funcionários com essas informações no sistema!"; }
                else lblError.Visible = false;

                //Preenche o ComboBox com os cadastros da Tabela - Cargo
                ddlCargoEmployee.DataSource = dtManip.ExecDtTableStringQuery("SELECT * FROM CARGO");
                ddlCargoEmployee.DataTextField = "NOME_CARGO";
                ddlCargoEmployee.DataValueField = "COD_CARGO";
                ddlCargoEmployee.DataBind();                
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
                lblError.Text = "";
                lblSucess.Text = "";

                gc.strCodEmployee = "0";

                if (txtCode.Text != "") strCode = txtCode.Text;

                //Atualiza o grid
                gvEmployee.DataSource = dtManip.ExecDtTableStringQuery("CALL PROC_SELECT_FUNC('" + strCode + "', '" + txtCPF.Text.ToString() + "', '" + txtName.Text.ToString() + "')");
                gvEmployee.DataBind();

                if (gvEmployee.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há funcionários com essas informações no sistema!"; }
                else lblError.Visible = false;

                txtCode.Text = ""; txtCPF.Text = ""; txtName.Text = "";
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnSearchEmployee_Click()
        {
            try
            {
                if (gc.strCodEmployee != "0")
                {
                    cnt = new Connect();
                    cnt.DataBaseConnect();
                    MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_FUNC('" + gc.strCodEmployee + "', '', '')");

                    if (leitor.Read())
                    {
                        txtCodEmployee.Text = leitor[0].ToString();
                        txtCpfEmployee.Text = leitor[1].ToString();
                        txtNomeEmployee.Text = leitor[2].ToString();
                        txtRGEmployee.Text = leitor[3].ToString();
                        txtDtNascEmployee.Text = Convert.ToDateTime(leitor[4].ToString()).ToString("yyyy-MM-dd");
                        txtTelEmployee.Text = leitor[5].ToString();
                        txtCelEmployee.Text = leitor[6].ToString();
                        txtEnderecoEmployee.Text = leitor[7].ToString();
                        txtNumEndecEmployee.Text = leitor[8].ToString();
                        txtBairroEmployee.Text = leitor[9].ToString();
                        txtCepEmployee.Text = leitor[10].ToString();
                        txtCidadeEmployee.Text = leitor[11].ToString();
                        txtUFEmployee.Text = leitor[12].ToString();
                        txtEmailEmployee.Text = leitor[13].ToString();
                        txtSenhaEmployee.Text = leitor[14].ToString();
                        if (leitor[15].ToString() != "") { txtDtDeslig.Text = Convert.ToDateTime(leitor[15].ToString()).ToString("yyyy-MM-dd"); } else { txtDtDeslig.Text = ""; };
                        ddlCargoEmployee.SelectedValue = leitor[16].ToString();
                    }
                    else { lblError.Text = "Não há funcionários com essas informações no sistema!"; }
                }
                else { lblError.Text = "É necessário selecionar um funcionário!"; ClearComponents(); }

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
                lblError.Text = "";
                lblSucess.Text = "";

                if (gc.strCodEmployee != "0")
                {
                    if (txtDtDeslig.Text != "")
                    {
                        strDtDesligamento = Convert.ToDateTime((txtDtDeslig.Text).Replace("-", "/")).ToString("dd/MM/yyyy");
                    }
                    else { strDtDesligamento = ""; }

                    cnt = new Connect();
                    cnt.DataBaseConnect();
                    MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_FUNC('" + gc.strCodEmployee + "', '" + txtCPF.Text.ToString() + "', '')");

                    if (leitor != null)
                    {
                        if (leitor.Read())
                        {
                            if (gc.strCodEmployee == leitor[0].ToString() && txtCpfEmployee.Text == leitor[1].ToString())
                            {
                                btnSendUpdate_Click();
                            }
                            else
                            {
                                if (ValidateComponents())
                                {
                                    var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_INSERT_FUNC('" + txtCpfEmployee.Text + "', '" + txtNomeEmployee.Text + "', '" + txtRGEmployee.Text + "', '"
                                         + Convert.ToDateTime((txtDtNascEmployee.Text).Replace("-", "/")).ToString("dd/MM/yyyy") + "', '" + txtTelEmployee.Text + "', '" + txtCelEmployee.Text + "', '" + txtEnderecoEmployee.Text + "', '"
                                         + txtNumEndecEmployee.Text + "', '" + txtBairroEmployee.Text + "', '" + txtCepEmployee.Text + "', '" + txtCidadeEmployee.Text + "', '"
                                         + txtUFEmployee.Text + "', '" + txtEmailEmployee.Text + "', '" + txtSenhaEmployee.Text + "', '" + strDtDesligamento + "', '"
                                         + ddlCargoEmployee.SelectedItem.Value + "')");

                                    if (objRetorno != null)
                                    {
                                        if (objRetorno == true)
                                        {
                                            lblSucess.Text = "Funcionário cadastrado com sucesso!";
                                            lblSucess.Visible = true;
                                            ClearComponents();
                                        }
                                        else
                                        {
                                            lblError.Text = "Atenção! Funcionário não cadastrado, verifique os dados digitados!";
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
                        var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_INSERT_FUNC('" + txtCpfEmployee.Text + "', '" + txtNomeEmployee.Text + "', '" + txtRGEmployee.Text + "', '"
                             + Convert.ToDateTime((txtDtNascEmployee.Text).Replace("-", "/")).ToString("dd/MM/yyyy") + "', '" + txtTelEmployee.Text + "', '" + txtCelEmployee.Text + "', '" + txtEnderecoEmployee.Text + "', '"
                             + txtNumEndecEmployee.Text + "', '" + txtBairroEmployee.Text + "', '" + txtCepEmployee.Text + "', '" + txtCidadeEmployee.Text + "', '"
                             + txtUFEmployee.Text + "', '" + txtEmailEmployee.Text + "', '" + txtSenhaEmployee.Text + "', '" + strDtDesligamento + "', '"
                             + ddlCargoEmployee.SelectedItem.Value + "')");

                        if (objRetorno == true)
                        {
                            lblSucess.Text = "Funcionário cadastrado com sucesso!";
                            lblSucess.Visible = true;
                            ClearComponents();
                        }
                        else
                        {
                            lblError.Text = "Atenção! Funcionário não cadastrado, verifique os dados digitados!";
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
                if (txtDtDeslig.Text != "")
                {
                    strDtDesligamento = Convert.ToDateTime((txtDtDeslig.Text).Replace("-", "/")).ToString("dd/MM/yyyy");
                }
                else { strDtDesligamento = ""; }

                lblError.Text = "";
                lblSucess.Text = "";

                if (ValidateComponents())
                {
                    var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_UPDATE_FUNC('" + txtCpfEmployee.Text + "', '" + txtNomeEmployee.Text + "', '" + txtRGEmployee.Text + "', '"
                         + Convert.ToDateTime((txtDtNascEmployee.Text).Replace("-", "/")).ToString("dd/MM/yyyy") + "', '" + txtTelEmployee.Text + "', '" + txtCelEmployee.Text + "', '" + txtEnderecoEmployee.Text + "', '"
                         + txtNumEndecEmployee.Text + "', '" + txtBairroEmployee.Text + "', '" + txtCepEmployee.Text + "', '" + txtCidadeEmployee.Text + "', '"
                         + txtUFEmployee.Text + "', '" + txtEmailEmployee.Text + "', '" + txtSenhaEmployee.Text + "', '" + strDtDesligamento + "', '"
                         + ddlCargoEmployee.SelectedItem.Value + "')");

                    if (objRetorno != null)
                    {
                        if (objRetorno == true)
                        {
                            lblSucess.Text = "Funcionário alterado com sucesso!";
                            lblSucess.Visible = true;
                            ClearComponents();
                        }
                        else
                        {
                            lblError.Text = "Atenção! O Funcionário não foi alterado, verifique os dados digitados!";
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
                lblError.Text = "";
                lblSucess.Text = "";

                if (gc.strCodEmployee != "0")
                {
                    if (ValidateComponents())
                    {
                        var objRetorno = dtManip.ExecuteStringQuery("CALL PROC_UPDATE_FUNC('" + txtCpfEmployee.Text + "', '" + txtNomeEmployee.Text + "', '" + txtRGEmployee.Text + "', '"
                     + (txtDtNascEmployee.Text).Replace("-", "/") + "', '" + txtTelEmployee.Text + "', '" + txtCelEmployee.Text + "', '" + txtEnderecoEmployee.Text + "', '"
                     + txtNumEndecEmployee.Text + "', '" + txtBairroEmployee.Text + "', '" + txtCepEmployee.Text + "', '" + txtCidadeEmployee.Text + "', '"
                     + txtUFEmployee.Text + "', '" + txtEmailEmployee.Text + "', '" + txtSenhaEmployee.Text + "', '" + DateTime.Now.ToString("dd/MM/yyyy") + "', '"
                     + ddlCargoEmployee.SelectedItem.Value + "')");

                        if (objRetorno != null)
                        {
                            if (objRetorno == true)
                            {
                                lblSucess.Text = "Funcionário desligado com sucesso!";
                                lblSucess.Visible = true;
                                ClearComponents();
                            }
                            else
                            {
                                lblError.Text = "Atenção! O Funcionário não foi desligado, verifique os dados selecionados!";
                                lblError.Visible = true;
                                ClearComponents();
                            }
                        }
                    }
                    else { lblError.Visible = true; }
                }
                else
                {
                    lblError.Text = "Atenção! É necessário selecionar um Funcionáro.";
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
            gc.strCodEmployee = "0"; txtCodEmployee.Enabled = true;
            txtCodEmployee.Text = ""; txtNomeEmployee.Text = ""; txtCpfEmployee.Text = "";
            txtRGEmployee.Text = ""; txtDtNascEmployee.Text = ""; txtTelEmployee.Text = "";
            txtCelEmployee.Text = ""; txtEnderecoEmployee.Text = ""; txtNumEndecEmployee.Text = "";
            txtBairroEmployee.Text = ""; txtCepEmployee.Text = ""; txtCidadeEmployee.Text = "";
            txtUFEmployee.Text = ""; txtEmailEmployee.Text = ""; txtSenhaEmployee.Text = ""; txtDtDeslig.Text = "";
        }

        private void EnableComponents(bool value)
        {
            txtCodEmployee.Enabled = false; txtNomeEmployee.Enabled = value; txtCpfEmployee.Enabled = value;
            txtRGEmployee.Enabled = value; txtDtNascEmployee.Enabled = value; txtTelEmployee.Enabled = value;
            txtCelEmployee.Enabled = value; txtEnderecoEmployee.Enabled = value; txtNumEndecEmployee.Enabled = value;
            txtBairroEmployee.Enabled = value; txtCepEmployee.Enabled = value; txtCidadeEmployee.Enabled = value;
            txtUFEmployee.Enabled = value; txtEmailEmployee.Enabled = value; txtSenhaEmployee.Enabled = value; txtDtDeslig.Enabled = value;
        }
        #endregion

        #region Validate
        private bool ValidateComponents()
        {
            if (txtCpfEmployee.Text == "") { lblError.Text = ce.ComponentsValidation("CPF", gc.MSG_NECESSARIO); return false; }
            else if (txtNomeEmployee.Text == "") { lblError.Text = ce.ComponentsValidation("Nome", gc.MSG_NECESSARIO); return false; }
            else if (txtRGEmployee.Text == "") { lblError.Text = ce.ComponentsValidation("RG", gc.MSG_NECESSARIO); return false; }
            else if (txtDtNascEmployee.Text == "") { lblError.Text = ce.ComponentsValidation("Data de Nascimento", gc.MSG_NECESSARIO); return false; }
            else if (txtEmailEmployee.Text == "") { lblError.Text = ce.ComponentsValidation("Email", gc.MSG_NECESSARIO); return false; }
            else if (txtSenhaEmployee.Text == "") { lblError.Text = ce.ComponentsValidation("Senha", gc.MSG_NECESSARIO); return false; }
            else if (ddlCargoEmployee.SelectedItem.Text == "") { lblError.Text = ce.ComponentsValidation("Cargo", gc.MSG_NECESSARIO); return false; }
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
        protected void gvEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearComponents();

            gc.strCodEmployee = (sender as LinkButton).CommandArgument; //Código do funcionário selecionado no grid
            if (gc.strCodEmployee != "0") { btnSearchEmployee_Click(); }
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
            string caminho = AppDomain.CurrentDomain.BaseDirectory + @"\PDF\Employee.pdf";

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
            titulo.Add("\n\n Funcionários\n\n");
            doc.Add(titulo);

            Paragraph paragrafo = new Paragraph("", new Font(Font.BOLD, 10));
            string conteudo = "Este arquivo contém uma lista de todos os funcionários cadastrados no sistema!\n\n\n";
            paragrafo.Alignment = Element.ALIGN_CENTER;
            paragrafo.Add(conteudo);
            doc.Add(paragrafo);

            PdfPTable table = new PdfPTable(6);
            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_FUNC('" + strCode + "', '" + txtCPF.Text.ToString() + "', '" + txtName.Text.ToString() + "')");

            table.AddCell("Código");
            table.AddCell("CPF");
            table.AddCell("Nome");
            table.AddCell("Data Nascimento");
            table.AddCell("Cidade");
            table.AddCell("Data de Desligamento");

            if (leitor != null)
            {

                while (leitor.Read())
                {
                    table.AddCell(leitor[0].ToString());
                    table.AddCell(leitor[1].ToString());
                    table.AddCell(leitor[2].ToString());
                    table.AddCell(leitor[4].ToString());
                    table.AddCell(leitor[11].ToString());
                    table.AddCell(leitor[15].ToString());
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