using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace Proyecto.Mysql
{
    internal class Ccredito
    {
        public void mostrarCredito(DataGridView tablaCredito)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = @"SELECT 
                        cre.cre_cod AS 'Credito ID',
                        cre.fac_cod AS 'Factura Codigo',
                        CONCAT(c.cli_pnom, ' ', IFNULL(c.cli_snom, ''), ' ', c.cli_pape, ' ', IFNULL(c.cli_sape, '')) AS 'Nombre Cliente',
                        cre.cre_monto as 'Monto a pagar',
                        f.fac_fec AS 'Fecha Factura',
                        cre.cre_fecpagado as 'Fecha Pagada',   
                        cre.cre_pagado AS 'Pagado'
                     FROM 
                        credito cre
                     JOIN 
                        factura f ON cre.fac_cod = f.fac_cod
                     JOIN 
                        cliente c ON f.cli_id = c.cli_cod
                     ORDER BY cre.cre_cod ASC"; // Orden ascendente por cre_cod

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                tablaCredito.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar los datos de crédito: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public DataRow buscarCredito(int id)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = @"SELECT 
                            cre.cre_cod,
                            CONCAT(c.cli_pnom, ' ', IFNULL(c.cli_snom, ''), ' ', c.cli_pape, ' ', IFNULL(c.cli_sape, '')) AS 'Nombre Cliente',
                            cre.cre_monto
                         FROM 
                            credito cre
                         JOIN 
                            factura f ON cre.fac_cod = f.fac_cod
                         JOIN 
                            cliente c ON f.cli_id = c.cli_cod
                         WHERE 
                            cre.cre_cod = @id";

                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id);

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
                MessageBox.Show("Error al buscar crédito: " + ex.Message);
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

        public void actualizarCredito(TextBox cre_cod, decimal abono, DateTime fechaPagado)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Obtener el monto actual
                string querySelect = "SELECT cre_monto FROM credito WHERE cre_cod = @cre_cod";
                MySqlCommand commandSelect = new MySqlCommand(querySelect, conexion);
                commandSelect.Parameters.AddWithValue("@cre_cod", cre_cod.Text);
                decimal montoActual = Convert.ToDecimal(commandSelect.ExecuteScalar());

                // Validar si el monto actual ya es 0
                if (montoActual == 0)
                {
                    MessageBox.Show("El crédito ya está pagado.");
                    return;
                }

                // Calcular el nuevo monto
                decimal nuevoMonto = montoActual - abono;

                // Actualizar el crédito
                string queryUpdate = @"UPDATE credito 
                               SET cre_monto = @nuevoMonto, 
                                   cre_fecpagado = @fechaPagado,
                                   cre_pagado = @pagado
                               WHERE cre_cod = @cre_cod";
                MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, conexion);
                commandUpdate.Parameters.AddWithValue("@nuevoMonto", nuevoMonto);
                commandUpdate.Parameters.AddWithValue("@fechaPagado", fechaPagado);
                commandUpdate.Parameters.AddWithValue("@pagado", nuevoMonto <= 0);
                commandUpdate.Parameters.AddWithValue("@cre_cod", cre_cod.Text);

                commandUpdate.ExecuteNonQuery();

                MessageBox.Show(nuevoMonto <= 0 ? "Crédito actualizado y marcado como pagado." : "Crédito actualizado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar crédito: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        public void BuscarCreditosporFiltros(DataGridView tablaFactura, TextBox textBoxFiltro, ComboBox comboBoxFiltro)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Construir la consulta base
                string query = "SELECT cre.cre_cod as 'Credito Codigo', cre.fac_cod as 'Factura Codigo'," +
                               "CONCAT(cli.cli_pnom, ' ', IFNULL(cli.cli_snom, ''), ' ', cli.cli_pape, ' ', IFNULL(cli.cli_sape, '')) AS 'Nombre Cliente', " +
                               "cre.cre_monto as 'Monto a pagar',fac.fac_fec as 'Fecha Factura', cre.cre_fecpagado as 'Fecha Pagado', cre.cre_pagado as 'Pagado' " +
                               "FROM credito cre " +
                               "INNER JOIN factura fac ON cre.fac_cod = fac.fac_cod " +
                               "INNER JOIN cliente cli ON fac.cli_id = cli.cli_cod " +
                               "WHERE 1=1";

                // Determinar el filtro basado en la selección del ComboBox
                if (comboBoxFiltro.SelectedItem.ToString() == "Nombre del cliente" && !string.IsNullOrWhiteSpace(textBoxFiltro.Text))
                {
                    query += " AND CONCAT(cli.cli_pnom, ' ', IFNULL(cli.cli_snom, ''), ' ', cli.cli_pape, ' ', IFNULL(cli.cli_sape, '')) LIKE @filtro";
                }
                else if (comboBoxFiltro.SelectedItem.ToString() == "Codigo Factura" && !string.IsNullOrWhiteSpace(textBoxFiltro.Text))
                {
                    query += " AND cre.fac_cod = @filtro";
                }
                else if (comboBoxFiltro.SelectedItem.ToString() == "Codigo Credito" && !string.IsNullOrWhiteSpace(textBoxFiltro.Text))
                {
                    query += " AND cre.cre_cod = @filtro";
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
                    else if (comboBoxFiltro.SelectedItem.ToString() == "Codigo Credito")
                    {
                        command.Parameters.AddWithValue("@filtro", textBoxFiltro.Text.Trim());
                    }
                }

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









    }
}
