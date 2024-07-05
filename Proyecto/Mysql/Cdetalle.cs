using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Proyecto.Mysql
{
    internal class Cdetalle
    {
        public void MostrarDetalleFactura(DataGridView tablaDetalleFactura, TextBox fac_cod)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Consulta SQL para obtener el detalle de la factura junto con el nombre del producto
                string query = @"SELECT  
                                df.pro_cod AS 'Código Producto', 
                                p.pro_nom AS 'Nombre Producto',
                                p.pro_pre AS 'Precio',
                                df.fac_can AS 'Cantidad', 
                                df.fac_impu AS 'Impuesto', 
                                df.fac_total AS 'Total'
                         FROM detalle_factura df
                         JOIN producto p ON df.pro_cod = p.pro_cod
                         WHERE df.fac_cod = @fac_cod";

                MySqlCommand cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@fac_cod", fac_cod.Text);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                tablaDetalleFactura.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el detalle de la factura: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        public void MostrarFactura(DataGridView tablaFactura)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Consulta SQL para obtener los datos de la factura con alias para las columnas
                string query = "SELECT fac_cod as 'Código Factura', fac_fec as 'Fecha', " +
                               "CONCAT(cli_pnom, ' ', cli_snom, ' ', cli_pape, ' ', cli_sape) as 'Cliente', " +
                               "fac_total as 'Total' " +
                               "FROM factura " +
                               "INNER JOIN cliente ON factura.cli_id = cliente.cli_cod";

                MySqlCommand cmd = new MySqlCommand(query, conexion);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                tablaFactura.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los datos de la factura: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void BuscarFacturaPorId(DataGridView tablaFactura, TextBox idFactura)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Query para buscar facturas por su código y mostrar nombre completo del cliente concatenado
                string query = "SELECT fac_cod as 'Código Factura', fac_fec as 'Fecha', " +
                               "CONCAT(cli_pnom, ' ', IFNULL(cli_snom, ''), ' ', cli_pape, ' ', IFNULL(cli_sape, '')) AS 'Cliente', " +
                               "fac_total as 'Total' " +
                               "FROM factura " +
                               "INNER JOIN cliente ON factura.cli_id = cliente.cli_cod " +  // Join con la tabla cliente para obtener el nombre completo
                               "WHERE fac_cod = @idFactura";

                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@idFactura", idFactura.Text.Trim());

                // Adaptador para llenar los datos en un DataTable
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Limpiar el DataGridView antes de mostrar los resultados
                tablaFactura.DataSource = null;
                tablaFactura.Rows.Clear();

                // Asignar el DataTable con los resultados al DataGridView
                tablaFactura.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar factura: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void eliminarFactura(TextBox codFactura)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Eliminar detalles de la factura de la tabla detalle_factura
                string queryDetalle = "DELETE FROM detalle_factura WHERE fac_cod = @facCod";
                MySqlCommand commandDetalle = new MySqlCommand(queryDetalle, conexion);
                commandDetalle.Parameters.AddWithValue("@facCod", codFactura.Text);

                int rowsAffectedDetalle = commandDetalle.ExecuteNonQuery();

                // Eliminar la factura de la tabla factura
                string queryFactura = "DELETE FROM factura WHERE fac_cod = @facCod";
                MySqlCommand commandFactura = new MySqlCommand(queryFactura, conexion);
                commandFactura.Parameters.AddWithValue("@facCod", codFactura.Text);

                int rowsAffectedFactura = commandFactura.ExecuteNonQuery();

                if (rowsAffectedFactura > 0)
                {
                    MessageBox.Show("Se eliminó la factura y sus detalles correctamente.");
                }
                else
                {
                    MessageBox.Show("No se encontró ninguna factura con ese código.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar la factura y/o sus detalles. Error: " + ex.Message);
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
