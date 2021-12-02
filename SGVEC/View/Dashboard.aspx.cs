using System;
using System.Web.UI;
using SGVEC.Controller;
using MySql.Data.MySqlClient;
using SGVEC.Models;
using System.Data;

namespace SGVEC.View
{
    public partial class DashBoard : System.Web.UI.Page
    {
        private Connect cnt = new Connect();
        private DataManipulation dtManip = new DataManipulation();
        private GeneralComponent gc = new GeneralComponent();

        protected void Page_Load(object sender, EventArgs e)
        {
            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_FUNC('" + 0 + "', '" + gc.CPF + "', '" + gc.Name + "')");

            if (leitor.Read()) { gc.CodEmployeeLog = Convert.ToInt32(leitor[0].ToString()); gc.CodEmployee = Convert.ToInt32(leitor[16].ToString()); lblNomeFunc.Text = leitor[2].ToString(); }

            GetTotal();

            cnt.closeConection();
        }

        protected string GetDataSales()
        {
            int janeiro = 0, fevereiro = 0, marco = 0, abril = 0, maio = 0, junho = 0,
                    julho = 0, agosto = 0, setembro = 0, outubro = 0, novembro = 0, dezembro = 0;

            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_SELECT_SALE('" + 0 + "', '', '', '')");

            DataTable data = new DataTable();

            //Título das Colunas
            data.Columns.Add(new DataColumn("Task", typeof(string)));
            data.Columns.Add(new DataColumn("Hours", typeof(string)));

            while (leitor.Read())
            {
                switch (Convert.ToDateTime(leitor[3].ToString()).ToString("MM"))
                {
                    case "01":
                        janeiro = janeiro + 1;
                        break;
                    case "02":
                        fevereiro = fevereiro + 1;
                        break;
                    case "03":
                        marco = marco + 1;
                        break;
                    case "04":
                        abril = abril + 1;
                        break;
                    case "05":
                        maio = maio + 1;
                        break;
                    case "06":
                        junho = junho + 1;
                        break;
                    case "07":
                        julho = julho + 1;
                        break;
                    case "08":
                        agosto = agosto + 1;
                        break;
                    case "09":
                        setembro = setembro + 1;
                        break;
                    case "10":
                        outubro = outubro + 1;
                        break;
                    case "11":
                        novembro = novembro + 1;
                        break;
                    case "12":
                        dezembro = dezembro + 1;
                        break;
                }
            }

            //Dados que irão aparecer no Chart
            data.Rows.Add(new Object[] { janeiro }); //Janeiro 
            data.Rows.Add(new Object[] { fevereiro }); //Fevereiro 
            data.Rows.Add(new Object[] { marco }); //Março 
            data.Rows.Add(new Object[] { abril }); //Abril 
            data.Rows.Add(new Object[] { maio }); //Maio 
            data.Rows.Add(new Object[] { junho }); //Junho 
            data.Rows.Add(new Object[] { julho }); //Julho 
            data.Rows.Add(new Object[] { agosto }); //Agosto 
            data.Rows.Add(new Object[] { setembro }); //Setembro
            data.Rows.Add(new Object[] { outubro }); //Outubro
            data.Rows.Add(new Object[] { novembro }); //Novembro
            data.Rows.Add(new Object[] { dezembro }); //Dezembro

            string strDatas = "";
            strDatas = strDatas + "[";

            foreach (DataRow dr in data.Rows)
            {
                strDatas = strDatas + "'" + dr[0] + "',";
            }

            strDatas = strDatas + "]";

            cnt.closeConection();
            return strDatas;
        }

        protected string GetDataProduct()
        {
            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor = dtManip.ExecuteDataReader(@"SELECT SUM(P.QUANTIDADE_PROD) FROM PRODUTO P INNER JOIN TIPO_PRODUTO TP ON P.FK_COD_TIPO_PROD = TP.COD_TIPO_PROD
                            GROUP BY P.FK_COD_TIPO_PROD");

            DataTable data = new DataTable();

            //Título das Colunas
            data.Columns.Add(new DataColumn("Task", typeof(string)));
            data.Columns.Add(new DataColumn("Hours", typeof(string)));

            while (leitor.Read())
            {
                data.Rows.Add(new Object[] { leitor[0].ToString() });
            }

            string strDatas = "";
            strDatas = strDatas + "[";

            foreach (DataRow dr in data.Rows)
            {
                strDatas = strDatas + "'" + dr[0] + "',";
            }

            strDatas = strDatas + "]";

            cnt.closeConection();
            return strDatas;
        }

        protected string GetNameProduct()
        {
            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor = dtManip.ExecuteDataReader(@"SELECT TP.NOME_TIPO_PROD FROM PRODUTO P INNER JOIN TIPO_PRODUTO TP ON P.FK_COD_TIPO_PROD = TP.COD_TIPO_PROD
                            GROUP BY P.FK_COD_TIPO_PROD");

            DataTable data = new DataTable();

            //Título das Colunas
            data.Columns.Add(new DataColumn("Task", typeof(string)));
            data.Columns.Add(new DataColumn("Hours", typeof(string)));

            while (leitor.Read())
            {
                data.Rows.Add(new Object[] { leitor[0].ToString() });
            }

            string strDatas = "";
            strDatas = strDatas + "[";

            foreach (DataRow dr in data.Rows)
            {
                strDatas = strDatas + "'" + dr[0] + "',";
            }

            strDatas = strDatas + "]";
            cnt.closeConection();

            return strDatas;
        }

        protected bool GetTotal()
        {
            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor = dtManip.ExecuteDataReader("SELECT SUM(TOTAL_VENDA) FROM VENDA");

            if (leitor.Read())
            {
                lblVlTotalSales.Text = leitor[0].ToString();
            }
            else
            {
                cnt.closeConection();
                return false;
            }

            cnt = new Connect();
            cnt.DataBaseConnect();
            MySqlDataReader leitor2 = dtManip.ExecuteDataReader("SELECT SUM(QUANTIDADE_PROD) FROM PRODUTO");

            if (leitor2.Read())
            {
                lblVlTotalProd.Text = leitor2[0].ToString();
            }
            else
            {
                cnt.closeConection();
                return false;
            }

            cnt.closeConection();
            return true;
        }
    }
}