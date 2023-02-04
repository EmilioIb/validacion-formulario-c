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
    public partial class Form1 : Form
    {
        UnicodeEncoding ByteConverter = new UnicodeEncoding(); //instanciamos objeto de clase unicodeencoing
        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(); //instanciamos objeto de clase rsa
        byte[] plaintext;
        byte[] encryptedtext;

        public Form1()
        {
            InitializeComponent();
        }

        public void conexion()
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //BLOQUE PARA CONECTARESE A LA BD
            //Cadena de conexion a SQL
            SqlConnection myConnection = new SqlConnection(@"user id=sa; password=utlaguna1.; server=DESKTOP-E72ID0T\SEGURIDADB;database=securityb; connection timeout=30");
            //SqlConnection myConnection = new SqlConnection(@"user id=sa; password=utlaguna1.; server=DESKTOP-E72ID0T\SEGURIDADB; Trusted_Connection=yes ;database=securityb; connection timeout=30");

            //se pone un try cada vez que se conecte a la BD
            try
            {
                myConnection.Open();
                txterror.Visible = true;
                txterror.Text = "SUCCESS!!";
            }
            catch(SqlException sqle1)
            {
                txterror.Visible = true;
                txterror.Text = "ERROR 20000. Something failed while trying to connect";
                //txterror.Text = sqle1.Message; //No es recomendable enviar los mensajes de error
                myConnection.Close();
            }
            catch (Exception ex1)
            {
                txterror.Visible = true;
                txterror.Text = "ERROR 10000. Call to technical support";
                //txterror.Text = ex1.Message; //No es recomendable enviar los mensajes de error
                myConnection.Close();
            }

            //BLOQUE PARA INSERTAR
            //Definimos el query para insertar
            string query1 = "INSERT INTO students VALUES(@enrollmentid ,@name, @lastname, @birthday, @age, @irsn, @username, @password, @salt);";

            //Creamos la variable para el comando y le pasamos el query y la conexion
            SqlCommand cmd1 = new SqlCommand(query1, myConnection);

            //Añadimos los parametros y su tipo de dato sql
            cmd1.Parameters.Add("@enrollmentid", SqlDbType.Char);
            cmd1.Parameters.Add("@name", SqlDbType.VarChar);
            cmd1.Parameters.Add("@lastname", SqlDbType.VarChar);
            cmd1.Parameters.Add("@birthday", SqlDbType.Date);
            cmd1.Parameters.Add("@age", SqlDbType.SmallInt);
            cmd1.Parameters.Add("@irsn", SqlDbType.Char);
            cmd1.Parameters.Add("@username", SqlDbType.Char);
            cmd1.Parameters.Add("@password", SqlDbType.VarBinary);
            cmd1.Parameters.Add("@salt", SqlDbType.VarBinary);

            //Les damos valor a los parametros
            cmd1.Parameters["@enrollmentid"].Value = "12345678";
            cmd1.Parameters["@name"].Value = "PANFILO";
            cmd1.Parameters["@lastname"].Value = "ASTORGA";
            cmd1.Parameters["@birthday"].Value = "2001-05-10";
            cmd1.Parameters["@age"].Value = 21;
            cmd1.Parameters["@irsn"].Value = "HECT010510T43";

            string user = textBox6.Text.Trim();
            cmd1.Parameters["@username"].Value = user;

            byte[] salt = encriptar.GenerateSalt();
            cmd1.Parameters["@salt"].Value = salt;

            string password = textBox7.Text.Trim();
            var hashedPassword = encriptar.HashPasswordWithSalth(Encoding.UTF8.GetBytes(password),salt);
            cmd1.Parameters["@password"].Value = hashedPassword; 

            

            txterror.Visible = true;

            try
            {
                //ejecutamos el comando
                cmd1.ExecuteNonQuery();
                txterror.Text = "Query executed succesfully";
            }
            catch (SqlException sqle1)
            {
                //txterror.Text = "ERROR 20000. Something failed during the insert";
                txterror.Text = sqle1.Message; //No es recomendable enviar los mensajes de error
            }
            catch(Exception ex1){
                //txterror.Text = "ERROR 10001. Call to technical support";
                txterror.Text = ex1.Message; //No es recomendable enviar los mensajes de error
            }
            finally
            {
                myConnection.Close();
            }
            
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txterror.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            plaintext = ByteConverter.GetBytes(textBox7.Text);
            encryptedtext = Encryption(plaintext, RSA.ExportParameters(false), false);
            textBox8.Text = ByteConverter.GetString(encryptedtext);
        }

        //Encriptacion rsa
        static public byte[] Encryption(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    encryptedData = RSA.Encrypt(Data, DoOAEPPadding);
                } return encryptedData;

            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        //Desencripcion rsa
        static public byte[] Decryption(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try{
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    decryptedData = RSA.Decrypt(Data, DoOAEPPadding);
                }

                return decryptedData;
            }
            catch(CryptographicException e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }   

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] decryptedtext = Decryption(encryptedtext, RSA.ExportParameters(true), false);
            textBox9.Text = ByteConverter.GetString(decryptedtext);
        }

        private void button4_Click(object sender, EventArgs e)
        {
           textBox10.Text = ByteConverter.GetString(encriptar.GenerateSalt());
        }
    }
}
