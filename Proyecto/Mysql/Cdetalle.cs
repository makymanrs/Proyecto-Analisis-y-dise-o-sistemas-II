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
    }
}
