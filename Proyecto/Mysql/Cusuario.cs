using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Proyecto.Mysql
{
    internal class Cusuario
    {
        public void inicioSesion(TextBox Usuario, TextBox Contra, Form loginForm)
        {
            if (string.IsNullOrEmpty(Usuario.Text) || string.IsNullOrEmpty(Contra.Text))
            {
                MessageBox.Show("Favor llenar todos los campos", "Campos vacíos!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Salir del método si hay campos vacíos
            }

            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();
                string queryUsuario = "SELECT COUNT(*) FROM usuario WHERE usu_nom LIKE @usu AND usu_contra LIKE @contra";
                MySqlCommand commandUsuario = new MySqlCommand(queryUsuario, conexion);
                commandUsuario.Parameters.AddWithValue("@usu", Usuario.Text);
                commandUsuario.Parameters.AddWithValue("@contra", Contra.Text);
                int countUsuario = Convert.ToInt32(commandUsuario.ExecuteScalar());

                if (countUsuario > 0)
                {
                    MessageBox.Show("Inicio de sesión exitoso", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loginForm.Hide(); // Cerrar el formulario de inicio de sesión
                    MostrarForm1(); // Mostrar el Form1 después del inicio de sesión exitoso
                }
                else
                {
                    MessageBox.Show("No existe este usuario, verificar datos correctos.", "Datos errados", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    // Limpiar los campos de usuario y contraseña para volver a intentar
                    Usuario.Text = "";
                    Contra.Text = "";
                    Usuario.Focus(); // Colocar el foco en el campo de usuario
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logró el acceso, intente en un momento: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public bool RegistrarUsuario(TextBox Usuario, TextBox Contra, TextBox confirmarContra)
        {
            if (string.IsNullOrEmpty(Usuario.Text) || string.IsNullOrEmpty(Contra.Text) || string.IsNullOrEmpty(confirmarContra.Text))
            {
                MessageBox.Show("Favor llenar todos los campos", "Campos vacíos!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.Compare(Contra.Text, confirmarContra.Text) != 0)
            {
                MessageBox.Show("Ambas contraseñas deben ser IGUALES, favor intente nuevamente", "Contraseñas Diferentes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Contra.Text = "";
                confirmarContra.Text = "";
                Contra.Focus();
                return false;
            }

            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string queryVerificarUsuario = "SELECT COUNT(*) FROM usuario WHERE usu_nom LIKE @usu;";
                MySqlCommand commandVerificarUsuario = new MySqlCommand(queryVerificarUsuario, conexion);
                commandVerificarUsuario.Parameters.AddWithValue("@usu", Usuario.Text);
                int countUsuario = Convert.ToInt32(commandVerificarUsuario.ExecuteScalar());

                if (countUsuario > 0)
                {
                    MessageBox.Show("Ya existe un Usuario con el mismo nombre. Intente uno nuevo", "Duplicación de Usuario", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return false;
                }

                string query = "INSERT INTO usuario (usu_nom, usu_contra) VALUES (@usu_nom, @usu_contra)";
                MySqlCommand myCommandCliente = new MySqlCommand(query, conexion);
                myCommandCliente.Parameters.AddWithValue("@usu_nom", Usuario.Text);
                myCommandCliente.Parameters.AddWithValue("@usu_contra", confirmarContra.Text);
                myCommandCliente.ExecuteNonQuery();
                MessageBox.Show("Usuario registrado con éxito", "Registro exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar campos después del registro exitoso si es necesario
                Usuario.Text = "";
                Contra.Text = "";
                confirmarContra.Text = "";

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el registro: " + ex.Message);
                return false;
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        private void MostrarForm1()
        {
            // Show Form1 after login
            Form1 form1 = new Form1();
            form1.Show();
        }
    }
}