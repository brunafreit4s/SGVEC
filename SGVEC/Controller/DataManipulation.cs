using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace SGVEC.Controller
{
    public class DataManipulation
    {
        private Connect cnt = new Connect(); //Classe que possui o método de conexão com o Banco de Dados
        private MySqlConnection cn = new MySqlConnection();
        private MySqlCommand cm = new MySqlCommand();

        public DataTable ExecDtTableStringQuery(string CommandText)
        {
            cnt = new Connect();
            cn = new MySqlConnection();

            cn = cnt.DataBaseConnect();
            return ExecDtTableQuery(CommandText);
        }

        //Retorna uma tabela de dados, podendo ser usada no gridview ou no dropdowlist
        private DataTable ExecDtTableQuery(string CommandText)
        {
            try
            {
                MySqlDataAdapter mySqlDtAdpt = new MySqlDataAdapter(CommandText, cn);
                DataTable dtTable = new DataTable();
                mySqlDtAdpt.Fill(dtTable);
                cnt.closeConection();
                return dtTable;
            }
            catch
            {
                cnt.closeConection();
                return null;
            }
        }        

        public MySqlDataReader ExecuteDataReader(string CommandText)
        {           
            cnt = new Connect();
            cn = new MySqlConnection();

            cn = cnt.DataBaseConnect();
            return ExecuteDtReader(CommandText);
        }

        //Retorna uma tabela de dados, podendo ser usada no gridview
        private MySqlDataReader ExecuteDtReader(string CommandText)
        {
            try
            {
                cm = new MySqlCommand(CommandText, cn);
                cm.ExecuteNonQuery();

                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cm);

                da.Fill(dt);

                MySqlDataReader leitor = cm.ExecuteReader();                
                return leitor;
            }
            catch
            {
                return null;
            }
        }

        public bool ExecuteStringQuery(string CommandText)
        {
            cnt.closeConection();
            cnt = new Connect();
            cn = new MySqlConnection();

            cn = cnt.DataBaseConnect();
            return ExecuteQuery(CommandText);
        }

        private bool ExecuteQuery(string CommandText)
        {
            try
            {
                cm.Connection = cn;
                cm.CommandType = CommandType.Text;
                cm.CommandText = CommandText;
                cm.ExecuteNonQuery();
                cnt.closeConection();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}