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

        public void InsertarFactura(DateTimePicker dateTimePicker, TextBox textBoxClienteCodigo, Label labelTotalPagar, DataGridView dataGridFactura)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Verificar el stock de los productos antes de insertar la factura
                foreach (DataGridViewRow row in dataGridFactura.Rows)
                {
                    if (row.Cells["codigoProducto"].Value != null && row.Cells["cantidad"].Value != null)
                    {
                        string codigoProducto = row.Cells["codigoProducto"].Value.ToString();
                        int cantidadSolicitada = Convert.ToInt32(row.Cells["cantidad"].Value);

                        string stockQuery = "SELECT pro_can FROM producto WHERE pro_cod = @ProductoCodigo";

                        using (MySqlCommand cmdStock = new MySqlCommand(stockQuery, conexion))
                        {
                            cmdStock.Parameters.AddWithValue("@ProductoCodigo", codigoProducto);

                            int stockDisponible = Convert.ToInt32(cmdStock.ExecuteScalar());

                            if (stockDisponible < cantidadSolicitada)
                            {
                                MessageBox.Show($"No hay suficiente stock para el producto con código {codigoProducto}. Stock disponible: {stockDisponible}");
                                return; // Salir del método sin insertar la factura
                            }
                        }
                    }
                }

                // Insertar la factura
                string query = "INSERT INTO factura (fac_fec, cli_id, fac_total) VALUES (@Fecha, @ClienteId, @TotalPagar)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Fecha", dateTimePicker.Value);
                    cmd.Parameters.AddWithValue("@ClienteId", textBoxClienteCodigo.Text);
                    cmd.Parameters.AddWithValue("@TotalPagar", decimal.Parse(labelTotalPagar.Text.Replace("Total a Pagar: ", "").Trim(), System.Globalization.NumberStyles.Currency));

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Factura registrada exitosamente.");

                    // Obtener el código de la factura recién insertada
                    string facturaCodigo = cmd.LastInsertedId.ToString();

                    // Insertar detalles de la factura
                    InsertarDetalleFactura(facturaCodigo, dataGridFactura, conexion);
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

        public void InsertarDetalleFactura(string facturaCodigo, DataGridView dataGridFactura, MySqlConnection conexion)
        {
            try
            {
                string insertQuery = "INSERT INTO detalle_factura (fac_cod, pro_cod, fac_can, fac_sub, fac_impu, fac_total) VALUES (@FacturaCodigo, @ProductoCodigo, @Cantidad, @Subtotal, @Impuesto, @Total)";
                string updateQuery = "UPDATE producto SET pro_can = pro_can - @Cantidad WHERE pro_cod = @ProductoCodigo";

                using (MySqlCommand cmdInsert = new MySqlCommand(insertQuery, conexion))
                using (MySqlCommand cmdUpdate = new MySqlCommand(updateQuery, conexion))
                {
                    foreach (DataGridViewRow row in dataGridFactura.Rows)
                    {
                        if (row.Cells["codigoProducto"].Value != null && row.Cells["cantidad"].Value != null && row.Cells["Subtotal"].Value != null && row.Cells["impuesto"].Value != null && row.Cells["total"].Value != null)
                        {
                            int cantidad = Convert.ToInt32(row.Cells["cantidad"].Value);

                            // Insertar detalle factura
                            cmdInsert.Parameters.Clear();
                            cmdInsert.Parameters.AddWithValue("@FacturaCodigo", facturaCodigo);
                            cmdInsert.Parameters.AddWithValue("@ProductoCodigo", row.Cells["codigoProducto"].Value);
                            cmdInsert.Parameters.AddWithValue("@Cantidad", cantidad);
                            cmdInsert.Parameters.AddWithValue("@Subtotal", row.Cells["Subtotal"].Value);
                            cmdInsert.Parameters.AddWithValue("@Impuesto", row.Cells["impuesto"].Value);
                            cmdInsert.Parameters.AddWithValue("@Total", row.Cells["total"].Value);

                            cmdInsert.ExecuteNonQuery();

                            // Actualizar cantidad de producto
                            cmdUpdate.Parameters.Clear();
                            cmdUpdate.Parameters.AddWithValue("@ProductoCodigo", row.Cells["codigoProducto"].Value);
                            cmdUpdate.Parameters.AddWithValue("@Cantidad", cantidad);

                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar los detalles de la factura y actualizar las cantidades de productos: " + ex.Message);
            }
        }
    }
}
