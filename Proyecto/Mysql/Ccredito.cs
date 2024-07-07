﻿using System;
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
                            cliente c ON f.cli_id = c.cli_cod";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                tablaCredito.DataSource = dt;
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

    }
}
