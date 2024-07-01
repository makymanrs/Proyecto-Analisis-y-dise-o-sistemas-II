using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Proyecto.Mysql
{
    internal class Corden
    {
        public void mostrarOrden(DataGridView tablaOrden)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();
                string query = "SELECT orden_id as 'Orden Id', prove_id as 'Proveedor ID', pro_cod as 'Producto ID', orden_can as 'Cantidad', orden_pre as 'Precio', orden_tot as 'Total', " +
                    "orden_fecen as 'Fecha' " + "FROM orden";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                tablaOrden.DataSource = dt;
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

        public void buscarOrdenPorId(DataGridView tablaOrden, TextBox id)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Query para buscar órdenes por ID
                string query = "SELECT orden_id as 'Orden Id', prove_id as 'Proveedor ID', pro_cod as 'Producto ID', orden_can as 'Cantidad', orden_pre as 'Precio', orden_tot as 'Total', orden_fecen as 'Fecha' " +
                               "FROM orden " +
                               "WHERE orden_id = @id";

                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id.Text.Trim());

                // Adaptador para llenar los datos en un DataTable
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Limpiar el DataGridView antes de mostrar los resultados
                tablaOrden.DataSource = null;
                tablaOrden.Rows.Clear();

                // Asignar el DataTable con los resultados al DataGridView
                tablaOrden.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar orden a proveedor: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }


        public void eliminarOrden(TextBox cod, DataGridView tablaOrden)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Eliminar orden de la tabla
                string queryOrden = "DELETE FROM orden WHERE orden_id = @ordenId";
                MySqlCommand commandOrden = new MySqlCommand(queryOrden, conexion);
                commandOrden.Parameters.AddWithValue("@ordenId", cod.Text);

                int rowsAffected = commandOrden.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Se eliminó el registro de la orden correspondiente y las entradas relacionadas en la tabla 'orden'.");
                }
                else
                {
                    MessageBox.Show("No se encontró ninguna orden con ese ID.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el registro del orden correspondiente y/o las entradas relacionadas en la tabla 'orden'. Error: " + ex.Message);
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
