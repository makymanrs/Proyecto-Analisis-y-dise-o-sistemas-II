using MySql.Data.MySqlClient;
using Proyecto.Forms1;
using System;
using System.Windows.Forms;

namespace Proyecto.Mysql
{
    internal class Cfactura
    {
        public void BuscarClientePorCodigo(TextBox textBoxCodigoCliente, TextBox textBoxNombreCompleto)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = @"SELECT CONCAT(cli_pnom, ' ', IFNULL(cli_snom, ''), ' ', cli_pape, ' ', IFNULL(cli_sape, '')) AS NombreCompleto
                                 FROM cliente
                                 WHERE cli_cod = @CodigoCliente";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@CodigoCliente", textBoxCodigoCliente.Text.Trim()); // Asegúrate de trim() para eliminar espacios adicionales

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBoxNombreCompleto.Text = reader["NombreCompleto"].ToString();
                        }
                        else
                        {
                            textBoxNombreCompleto.Text = "";
                            MessageBox.Show("No se encontró ningún cliente con el código especificado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los datos del cliente: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void BuscarProductoPorCodigo(TextBox textBoxCodigoProducto, TextBox textBoxNombreProducto, NumericUpDown numericUpDownPrecio)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = @"SELECT pro_nom, pro_pre 
                                 FROM producto 
                                 WHERE pro_cod = @CodigoProducto";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@CodigoProducto", textBoxCodigoProducto.Text.Trim()); // Asegúrate de trim() para eliminar espacios adicionales

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBoxNombreProducto.Text = reader["pro_nom"].ToString();
                            numericUpDownPrecio.Value = Convert.ToDecimal(reader["pro_pre"]);
                        }
                        else
                        {
                            textBoxNombreProducto.Text = "";
                            numericUpDownPrecio.Value = 0;
                            MessageBox.Show("No se encontró ningún producto con el código especificado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los datos del producto: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void InsertarFactura(DateTimePicker dateTimePicker, TextBox textBoxClienteCodigo, Label labelTotalPagar)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = "INSERT INTO factura (fac_fec, cli_id, fac_total) VALUES (@Fecha, @ClienteId, @TotalPagar)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Fecha", dateTimePicker.Value);
                    cmd.Parameters.AddWithValue("@ClienteId", textBoxClienteCodigo.Text);
                    cmd.Parameters.AddWithValue("@TotalPagar", decimal.Parse(labelTotalPagar.Text.Replace("Total a Pagar: ", "").Trim(), System.Globalization.NumberStyles.Currency));

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Factura registrada exitosamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar la factura: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void InsertarDetalleFactura(TextBox textBoxFacturaCodigo, DataGridView dataGridFactura)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = "INSERT INTO detalle_factura (fac_cod, pro_cod, fac_can, fac_sub, fac_impu, fac_total) VALUES (@FacturaCodigo, @ProductoCodigo, @Cantidad, @Subtotal, @Impuesto, @Total)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    foreach (DataGridViewRow row in dataGridFactura.Rows)
                    {
                        if (row.Cells["codigoProducto"].Value != null && row.Cells["cantidad"].Value != null && row.Cells["Subtotal"].Value != null && row.Cells["impuesto"].Value != null && row.Cells["total"].Value != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@FacturaCodigo", textBoxFacturaCodigo.Text);
                            cmd.Parameters.AddWithValue("@ProductoCodigo", row.Cells["codigoProducto"].Value);
                            cmd.Parameters.AddWithValue("@Cantidad", row.Cells["cantidad"].Value);
                            cmd.Parameters.AddWithValue("@Subtotal", row.Cells["Subtotal"].Value);
                            cmd.Parameters.AddWithValue("@Impuesto", row.Cells["impuesto"].Value);
                            cmd.Parameters.AddWithValue("@Total", row.Cells["total"].Value);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar los detalles de la factura: " + ex.Message);
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
