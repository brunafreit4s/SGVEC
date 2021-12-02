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
    public partial class Sales : System.Web.UI.Page
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

                if (txtCode.Text != "") strCode = txtCode.Text;

                //Atualiza o grid
                gvSales.DataSource = dtManip.ExecDtTableStringQuery("CALL PROC_SELECT_SALE('" + strCode + "', '" + txtCpfCli.Text.ToString() + "', '" + txtCpfFunc.Text.ToString() + "', '" + txtDateSales.Text.ToString() + "')");
                gvSales.DataBind();

                if (gvSales.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há vendas com essas informações no sistema!"; }
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
                string strDtSales = "";

                if (txtDateSales.Text != "") { strDtSales = Convert.ToDateTime((txtDateSales.Text).Replace("-", "/")).ToString("dd/MM/yyyy"); }

                gc.strCodSales = "0";

                if (txtCode.Text != "") strCode = txtCode.Text;

                //Atualiza o grid
                gvSales.DataSource = dtManip.ExecDtTableStringQuery("CALL PROC_SELECT_SALE('" + strCode + "', '" + txtCpfCli.Text.ToString() + "', '" + txtCpfFunc.Text.ToString() + "', '" + strDtSales + "')");
                gvSales.DataBind();

                if (gvSales.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há vendas com essas informações no sistema!"; }
                else lblError.Visible = false;

                txtCode.Text = ""; txtCpfCli.Text = ""; txtCpfFunc.Text = "";
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnSearchSales_Click()
        {
            try
            {
                if (gc.strCodSales != "0")
                {
                    cnt = new Connect();
                    cnt.DataBaseConnect();
                    gvProdutos.DataSource = dtManip.ExecDtTableStringQuery(@"SELECT PV.COD_PROD_VENDA, PV.QUANTIDADE_PROD, PV.VALOR_UNITARIO_PROD, P.NOME_PROD, 
                                                                            PV.FK_COD_VENDA FROM PRODUTO_VENDA AS PV 
                                                                            INNER JOIN PRODUTO AS P ON P.COD_BARRAS = PV.FK_COD_PRODUTO 
                                                                            WHERE FK_COD_VENDA = '" + gc.strCodSales + "'");
                    gvProdutos.DataBind();

                    if (gvProdutos.Rows.Count == 0) { lblError.Visible = true; lblError.Text = "Não há produtos registrados nessa venda no sistema!"; }
                    else lblError.Visible = false;

                    cnt.closeConection();
                }
                else { lblError.Text = "É necessário selecionar um Produto!"; }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }
        #endregion

        #region SelectedIndex
        protected void gvSales_SelectedIndexChanged(object sender, EventArgs e)
        {
            gc.strCodSales = (sender as LinkButton).CommandArgument; //Código da Venda selecionada no grid
            if (gc.strCodSales != "0") { btnSearchSales_Click(); }
        }
        #endregion

        #region PDF
        protected void btnCreatePDF_Click(object sender, EventArgs e)
        {
            if (txtCode.Text != "") strCode = txtCode.Text;

            Document doc = new Document(PageSize.A3);
            doc.SetMargins(40, 40, 20, 80);
            doc.AddCreationDate();
            string caminho = AppDomain.CurrentDomain.BaseDirectory + @"\PDF\Sales.pdf";

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
            titulo.Add("\n\n Vendas\n\n");
            doc.Add(titulo);

            Paragraph paragrafo = new Paragraph("", new Font(Font.BOLD, 10));
            string conteudo = "Este arquivo contém uma lista de todas as vendas cadastradas no sistema!\n\n\n";
            paragrafo.Alignment = Element.ALIGN_CENTER;
            paragrafo.Add(conteudo);
            doc.Add(paragrafo);

            PdfPTable table = new PdfPTable(6);
            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_SALE('" + strCode + "', '" + txtCpfCli.Text.ToString() + "', '" + txtCpfFunc.Text.ToString() + "', '" + txtDateSales.Text.ToString() + "')");

            table.AddCell("Código");
            table.AddCell("CPF Cliente");
            table.AddCell("CPF Funcionário");
            table.AddCell("Data");
            table.AddCell("Desconto");
            table.AddCell("Total");

            if (leitor != null)
            {

                while (leitor.Read())
                {
                    table.AddCell(leitor[0].ToString());
                    table.AddCell(leitor[2].ToString());
                    table.AddCell(leitor[8].ToString());
                    table.AddCell(leitor[3].ToString());
                    table.AddCell(leitor[6].ToString());
                    table.AddCell(leitor[7].ToString());
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