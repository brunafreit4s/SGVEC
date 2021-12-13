using System;
using SGVEC.Models;
using SGVEC.Controller;
using MySql.Data.MySqlClient;

namespace SGVEC
{
    public partial class Login : System.Web.UI.Page
    {
        private DataManipulation dtManip = new DataManipulation();
        private ComponentError ce = new ComponentError();
        private Connect cnt = new Connect();        

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                cnt = new Connect();
                ce = new ComponentError();

                lblError.Text = "";

                if (txtLogin.Text == "")
                {
                    lblError.Text = ce.ComponentsValidation("Email", "É necessário preencher o campo ");
                }
                else if (txtPassword.Text == "")
                {
                    lblError.Text = ce.ComponentsValidation("Senha", "É necessário preencher o campo ");
                }
                else
                {
                    cnt.DataBaseConnect();
                    if (cnt.ExecuteStringQuery("CALL PROC_LOGIN_FUNC('" + txtLogin.Text.ToString() + "', '" + txtPassword.Text.ToString() + "')"))
                    {
                        dtManip = new DataManipulation();
                        cnt = new Connect();
                        cnt.DataBaseConnect();
                        MySqlDataReader leitor = dtManip.ExecuteDataReader("CALL PROC_LOGIN_FUNC('" + txtLogin.Text.ToString() + "', '" + txtPassword.Text.ToString() + "')");

                        if (leitor.Read())
                        {
                            if (leitor[3].ToString() != "")
                            {
                                lblError.Text = "Atenção! Seu acesso encontra-se Desligado. Entrar em contato com um Gerente!";
                            }
                            else
                            {
                                //Acessar Dashboard
                                Response.Redirect("http://localhost:52149/View/Dashboard");
                            }
                        }
                    }
                    else
                    {
                        cnt.closeConection();
                        lblError.Text = "Email ou Senha inválidos.";
                    }

                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
    }
}