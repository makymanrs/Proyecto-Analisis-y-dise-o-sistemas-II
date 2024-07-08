using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Proyecto.Mysql
{
    internal class Cdetalle
    {
        public void MostrarDetalleFactura(DataGridView tablaDetalleFactura, TextBox fac_cod, Label lblSubtotal, Label lblImpuesto, Label lblTotal)
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
                                df.fac_sub AS 'Subtotal', 
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

                // Calcular el subtotal, impuesto y total
                decimal subtotal = 0;
                decimal impuesto = 0;
                decimal total = 0;

                foreach (DataRow row in dt.Rows)
                {
                    subtotal += Convert.ToDecimal(row["Subtotal"]);
                    impuesto += Convert.ToDecimal(row["Impuesto"]);
                    total += Convert.ToDecimal(row["Total"]);
                }

                // Actualizar los labels con los valores calculados
                lblSubtotal.Text = subtotal.ToString("C");
                lblImpuesto.Text = impuesto.ToString("C");
                lblTotal.Text = total.ToString("C");
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

                // Consulta SQL para obtener los datos de la factura con alias para las columnas, ordenada por código de factura ascendente
                string query = "SELECT fac_cod as 'Código Factura', fac_fec as 'Fecha', " +
                               "CONCAT(cli_pnom, ' ', cli_snom, ' ', cli_pape, ' ', cli_sape) as 'Cliente', " +
                               "fac_total as 'Total' " +
                               "FROM factura " +
                               "INNER JOIN cliente ON factura.cli_id = cliente.cli_cod " +
                               "ORDER BY fac_cod ASC";

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

        public void BuscarFacturaPorFiltros(DataGridView tablaFactura, TextBox textBoxFiltro, ComboBox comboBoxFiltro)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Construir la consulta base
                string query = "SELECT fac_cod as 'Código Factura', fac_fec as 'Fecha', " +
                               "CONCAT(cli_pnom, ' ', IFNULL(cli_snom, ''), ' ', cli_pape, ' ', IFNULL(cli_sape, '')) AS 'Cliente', " +
                               "fac_total as 'Total' " +
                               "FROM factura " +
                               "INNER JOIN cliente ON factura.cli_id = cliente.cli_cod " +
                               "WHERE 1=1";

                // Mensaje de depuración para el filtro seleccionado
             //   MessageBox.Show("Filtro seleccionado: " + comboBoxFiltro.SelectedItem.ToString());

                // Determinar el filtro basado en la selección del ComboBox
                if (comboBoxFiltro.SelectedItem.ToString() == "Nombre del cliente" && !string.IsNullOrWhiteSpace(textBoxFiltro.Text))
                {
                    query += " AND CONCAT(cli_pnom, ' ', IFNULL(cli_snom, ''), ' ', cli_pape, ' ', IFNULL(cli_sape, '')) LIKE @filtro";
                }
                else if (comboBoxFiltro.SelectedItem.ToString() == "Codigo Factura" && !string.IsNullOrWhiteSpace(textBoxFiltro.Text))
                {
                    query += " AND fac_cod = @filtro";
                }

                MySqlCommand command = new MySqlCommand(query, conexion);

                // Asignar valores a los parámetros
                if (!string.IsNullOrWhiteSpace(textBoxFiltro.Text))
                {
                    if (comboBoxFiltro.SelectedItem.ToString() == "Nombre del cliente")
                    {
                        command.Parameters.AddWithValue("@filtro", "%" + textBoxFiltro.Text.Trim() + "%");
                    }
                    else if (comboBoxFiltro.SelectedItem.ToString() == "Codigo Factura")
                    {
                        command.Parameters.AddWithValue("@filtro", textBoxFiltro.Text.Trim());
                    }

                    // Mensaje de depuración para el valor del filtro
             //       MessageBox.Show("Valor del filtro: " + textBoxFiltro.Text.Trim());
                }

                // Mensaje de depuración para la consulta SQL
                //MessageBox.Show("Consulta SQL: " + command.CommandText);

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
