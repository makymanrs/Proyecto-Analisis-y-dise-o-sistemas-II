using MySql.Data.MySqlClient;
using Proyecto.Forms1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                string query = @"SELECT pro_nom AS NombreProducto, pro_pre AS Precio
                                 FROM producto
                                 WHERE pro_cod = @CodigoProducto";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@CodigoProducto", textBoxCodigoProducto.Text.Trim()); // Asegúrate de trim() para eliminar espacios adicionales

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBoxNombreProducto.Text = reader["NombreProducto"].ToString();
                            numericUpDownPrecio.Value = Convert.ToDecimal(reader["Precio"]);
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
        public void InsertarFactura(DateTimePicker facFec, TextBox cliId, Label facTotal)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = @"INSERT INTO factura (fac_fec, cli_id, fac_total)
                         VALUES (@facFec, @cliId, @facTotal);
                         SELECT LAST_INSERT_ID();"; // Obtener el último ID insertado

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@facFec", facFec.Value);
                    cmd.Parameters.AddWithValue("@cliId", cliId.Text.Trim());
                    decimal facTotalValue = decimal.Parse(facTotal.Text.Replace("Total a Pagar: ", "").Trim(), System.Globalization.NumberStyles.Currency);
                    cmd.Parameters.AddWithValue("@facTotal", facTotalValue);

                    // Obtener el último ID insertado en la factura
                    int facCod = Convert.ToInt32(cmd.ExecuteScalar());

                    // Insertar detalle de la factura usando el último ID de factura obtenido
                    //    InsertarDetalleFactura(facCod, FormFactura.dataGridFactura);
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

        // Método para insertar detalles de la factura
        public void InsertarDetalleFactura(TextBox facCod, DataGridView dataGridView,decimal subtotal)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string queryInsert = @"INSERT INTO detalle_factura (fac_cod, pro_cod, fac_can, fac_sub, fac_impu, fac_total)
                               VALUES (@facCod, @proCod, @facCan, @facSub, @facImpu, @facTotal)";

                string queryUpdate = @"UPDATE producto 
                               SET pro_can = pro_can - @facCan 
                               WHERE pro_cod = @proCod";

                using (MySqlTransaction transaction = conexion.BeginTransaction())
                {
                    try
                    {
                        foreach (DataGridViewRow row in dataGridView.Rows)
                        {
                            int proCod = Convert.ToInt32(row.Cells["CodigoProducto"].Value);
                            int facCan = Convert.ToInt32(row.Cells["Cantidad"].Value);
                            // Obtener el subtotal desde el Label en lugar de DataGridView
                            decimal facSub = subtotal;
                            decimal facImpu = Convert.ToDecimal(row.Cells["Impuesto"].Value);
                            decimal facTotal = Convert.ToDecimal(row.Cells["Total"].Value);

                            using (MySqlCommand cmdInsert = new MySqlCommand(queryInsert, conexion, transaction))
                            {
                                cmdInsert.Parameters.AddWithValue("@facCod", facCod.Text);
                                cmdInsert.Parameters.AddWithValue("@proCod", proCod);
                                cmdInsert.Parameters.AddWithValue("@facCan", facCan);
                                cmdInsert.Parameters.AddWithValue("@facSub", facSub);
                                cmdInsert.Parameters.AddWithValue("@facImpu", facImpu);
                                cmdInsert.Parameters.AddWithValue("@facTotal", facTotal);

                                cmdInsert.ExecuteNonQuery();
                            }

                            using (MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, conexion, transaction))
                            {
                                cmdUpdate.Parameters.AddWithValue("@facCan", facCan);
                                cmdUpdate.Parameters.AddWithValue("@proCod", proCod);

                                cmdUpdate.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw; // Re-throw the exception to be caught by the outer catch block
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar detalle de la factura: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        private bool DetallesInsertadosParaFactura(MySqlConnection conexion, int facCod)
        {
            string query = "SELECT COUNT(*) FROM detalle_factura WHERE fac_cod = @facCod";
            MySqlCommand cmd = new MySqlCommand(query, conexion);
            cmd.Parameters.AddWithValue("@facCod", facCod);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0; // Devuelve true si ya hay detalles insertados para esta factura
        }


    }

}

