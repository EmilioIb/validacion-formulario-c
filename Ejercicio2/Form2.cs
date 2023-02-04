using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; //libreria para base datos
using System.Security.Cryptography; //libreria criptografia rca

namespace Ejercicio2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            string usuario = txtUser.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (isValidPassword(usuario, password))
            {
                labelError.Text = "Bienvenido " +  usuario + " credenciales validas";
            }
            else
            {
                labelError.Text = "Credenciales invalidas";
            }

            
        }

        private bool isValidPassword(string username, string password)
        {
            userBE user = getUserFromDB(username);

            bool isValid = false;

            if (!string.IsNullOrEmpty(user.user))
            {
                byte[] hashedPassword = encriptar.HashPasswordWithSalth(Encoding.UTF8.GetBytes(password),user.salt);

                if (hashedPassword.SequenceEqual(user.pass))
                    isValid = true;
            }
            return isValid;
            
        }

  
        private userBE getUserFromDB(string username)
        {
            userBE user = new userBE();
            string connectionString = @"user id=sa; password=utlaguna1.; server=DESKTOP-E72ID0T\SEGURIDADB;database=securityb; connection timeout=30";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string saltSaved = "select username, salt, password from students where username = @username";
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = saltSaved;
                    command.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = username;
                    try
                    {
                        connection.Open();
                        using (SqlDataReader oReader = command.ExecuteReader())
                        {
                            if (oReader.Read())
                            {
                                user.user = oReader["username"].ToString();
                                user.salt = (byte[])oReader["salt"];
                                user.pass = (byte[])oReader["password"];
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        labelError.Text = ex.Message;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return user;
        }
    }
}
