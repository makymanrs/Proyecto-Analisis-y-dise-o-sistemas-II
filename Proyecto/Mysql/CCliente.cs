using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto.Mysql
{
    internal class CCliente
    {
        public void guardarcliente(TextBox cod, TextBox id, TextBox primernom, TextBox segundonom, TextBox primerape, TextBox segundoape, TextBox edad, RichTextBox direccion, TextBox telefono)
        {
            // Verificar si alguno de los campos está vacío
            if (string.IsNullOrWhiteSpace(cod.Text) || string.IsNullOrWhiteSpace(id.Text) ||
                string.IsNullOrWhiteSpace(primernom.Text) || string.IsNullOrWhiteSpace(segundonom.Text) ||
                string.IsNullOrWhiteSpace(primerape.Text) || string.IsNullOrWhiteSpace(segundoape.Text) ||
                string.IsNullOrWhiteSpace(edad.Text) || string.IsNullOrWhiteSpace(direccion.Text) || string.IsNullOrWhiteSpace(telefono.Text))
            {
                MessageBox.Show("Todos los campos deben estar llenos.");
                return;
            }

            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Verificar si ya existe el cliente con el mismo cli_cod
                string queryVerificarcliente = "SELECT COUNT(*) FROM cliente WHERE cli_cod = @clicod";
                MySqlCommand commandVerificarcliente = new MySqlCommand(queryVerificarcliente, conexion);
                commandVerificarcliente.Parameters.AddWithValue("@clicod", id.Text);
                int countcliente = Convert.ToInt32(commandVerificarcliente.ExecuteScalar());

                if (countcliente > 0)
                {
                    MessageBox.Show("Ya existe un cliente con el mismo código.");
                    return; // Salir del método sin guardar
                }

                // Insertar en tabla cliente
                string query = "INSERT INTO cliente(cli_cod, cli_iden, cli_pnom, cli_snom, cli_pape, cli_sape, cli_edad, cli_dir, cli_tel) " +
                               "VALUES (@clicod, @cliiden, @clipnom, @clisnom, @clipape, @clisape, @cliedad, @clidir, @clitel)";
                MySqlCommand myCommandCliente = new MySqlCommand(query, conexion);
                myCommandCliente.Parameters.AddWithValue("@clicod", cod.Text);
                myCommandCliente.Parameters.AddWithValue("@cliiden", id.Text);
                myCommandCliente.Parameters.AddWithValue("@clipnom", primernom.Text);
                myCommandCliente.Parameters.AddWithValue("@clisnom", segundonom.Text);
                myCommandCliente.Parameters.AddWithValue("@clipape", primerape.Text);
                myCommandCliente.Parameters.AddWithValue("@clisape", segundoape.Text);
                myCommandCliente.Parameters.AddWithValue("@cliedad", edad.Text);
                myCommandCliente.Parameters.AddWithValue("@clidir", direccion.Text);
                myCommandCliente.Parameters.AddWithValue("@clitel", telefono.Text);

                myCommandCliente.ExecuteNonQuery();
                MessageBox.Show("Se guardó el registro");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el registro: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        // 
        public void mostrarCliente(DataGridView tablacliente)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();
                string query = "SELECT cli_cod as 'Codigo', cli_iden as 'Identidad', " +
               "CONCAT(cli_pnom, ' ', COALESCE(cli_snom, ''), ' ', cli_pape, ' ', COALESCE(cli_sape, '')) as 'NombreCompleto', " +
               "cli_edad as 'Edad', cli_dir as 'Direccion', cli_tel as 'Telefono' " +
               "FROM cliente";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                tablacliente.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se encontraron los datos de la base de datos, error: " + ex.ToString());
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }


        }

        // mostrar base de datos cliente
        public void buscarclienteporcodigo(DataGridView tablaCliente, TextBox cod)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Query para buscar Cliente por ID
                string query = "SELECT cli_cod as 'Codigo', cli_iden as 'Identidad', " +
                              "CONCAT(cli_pnom, ' ', COALESCE(cli_snom, ''), ' ', cli_pape, ' ', COALESCE(cli_sape, '')) as 'NombreCompleto', " +
                              "cli_edad as 'Edad', cli_dir as 'Direccion', cli_tel as 'Telefono' " + "FROM cliente " +
                               "WHERE cli_cod = @cli";

                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@cli", cod.Text.Trim());

                // Adaptador para llenar los datos en un DataTable
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Limpiar el DataGridView antes de mostrar los resultados
                tablaCliente.DataSource = null;
                tablaCliente.Rows.Clear();

                // Asignar el DataTable con los resultados al DataGridView
                tablaCliente.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar proveedor: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        // Eliminar Clientes
        public void eliminarCliente(TextBox cod, DataGridView tablaCliente)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Eliminar relación con órdenes
                /*
                string queryOrden = "DELETE FROM orden WHERE prove_id = @proveId";
                MySqlCommand commandOrden = new MySqlCommand(queryOrden, conexion);
                commandOrden.Parameters.AddWithValue("@proveId", cod.Text);
                commandOrden.ExecuteNonQuery();
                */

                // Eliminar el proveedor de la tabla proveedor
                string queryProveedor = "DELETE FROM cliente WHERE cli_cod = @clicod";
                MySqlCommand commandProveedor = new MySqlCommand(queryProveedor, conexion);
                commandProveedor.Parameters.AddWithValue("@clicod", cod.Text);

                int rowsAffected = commandProveedor.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Se eliminó el registro del Cliente y las entradas relacionadas en la tabla 'orden'.");
                }
                else
                {
                    MessageBox.Show("No se encontró ningún Cliente con ese Codigo.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el registro del Cliente y/o las entradas relacionadas en la tabla 'orden'. Error: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }


        // buscar para editar

        public DataRow buscarCliente(int id)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = "SELECT cli_cod, cli_iden, cli_pnom, cli_snom, cli_pape, cli_sape, cli_edad, cli_dir, cli_tel FROM cliente WHERE cli_cod = @clicod";
                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@clicod", id);

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]; // Devuelve la primera fila encontrada
                }
                else
                {
                    return null; // No se encontraron resultados
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cliente: " + ex.Message);
                return null;
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }


        public void modificarCliente(TextBox cod, TextBox id, TextBox primernom, TextBox segundonom, TextBox primerape, TextBox segundoape, TextBox edad, RichTextBox direccion, TextBox telefono)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = "UPDATE cliente SET cli_iden=@cliiden, cli_pnom=@clipnom, cli_snom=@clisnom, cli_pape=@clipape, cli_sape=@clisape, cli_edad=@cliedad, cli_dir=@clidir, cli_tel=@clitel WHERE cli_cod = @clicod";
                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@clicod", cod.Text);
                command.Parameters.AddWithValue("@cliiden", id.Text);
                command.Parameters.AddWithValue("@clipnom", primernom.Text);
                command.Parameters.AddWithValue("@clisnom", segundonom.Text);
                command.Parameters.AddWithValue("@clipape", primerape.Text);
                command.Parameters.AddWithValue("@clisape", segundoape.Text);
                command.Parameters.AddWithValue("@cliedad", edad.Text);
                command.Parameters.AddWithValue("@clidir", direccion.Text);
                command.Parameters.AddWithValue("@clitel", telefono.Text);

                command.ExecuteNonQuery();
                MessageBox.Show("Se modificaron los registros correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo modificar el registro: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

    }

}
