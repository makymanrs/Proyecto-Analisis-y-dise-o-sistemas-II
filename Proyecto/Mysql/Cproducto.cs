using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


namespace Proyecto.Mysql
{
    internal class Cproducto
    {
        public void mostrarproductos(DataGridView tablaproductos)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();
                string query = "SELECT pro_cod as 'Codigo', pro_nom as 'Nombre', pro_cad as 'Caducidad', pro_cos as 'Costo', pro_pre as 'Precio', pro_can as 'Cantidad', pro_img FROM producto";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Add a new DataColumn for the image
                dt.Columns.Add("Imagen", typeof(Image));
                foreach (DataRow row in dt.Rows)
                {
                    if (row["pro_img"] != DBNull.Value)
                    {
                        byte[] imgBytes = (byte[])row["pro_img"];
                        using (MemoryStream ms = new MemoryStream(imgBytes))
                        {
                            Image img = Image.FromStream(ms);
                            row["Imagen"] = img;
                        }
                    }
                }
                tablaproductos.DataSource = dt;

                // Hide the original pro_img column
                if (tablaproductos.Columns["pro_img"] != null)
                {
                    tablaproductos.Columns["pro_img"].Visible = false;
                }

                // Check if the Image column already exists to prevent duplication
                if (tablaproductos.Columns["Imagen"] == null)
                {
                    DataGridViewImageColumn imageColumn = new DataGridViewImageColumn
                    {
                        Name = "Imagen",
                        HeaderText = "Imagen",
                        ImageLayout = DataGridViewImageCellLayout.Zoom
                    };
                    tablaproductos.Columns.Add(imageColumn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los datos de la base de datos: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        private void CalcularPrecio(NumericUpDown costo, NumericUpDown precio)
        {
            decimal costoProducto = costo.Value;
            decimal precioProducto = costoProducto * 1.1m; // Calcular el precio con el 10% de incremento
            precio.Value = precioProducto; // Actualizar el valor del precio
        }

        // Método para guardar el producto
        public void guardarproductos(TextBox cod, TextBox producto, DateTimePicker cad, NumericUpDown costo, NumericUpDown precio, NumericUpDown can, TextBox numbod, PictureBox imgBox)
        {
            MySqlConnection conexion = null;
            MySqlTransaction transaction = null;
            try
            {
                string fechaCaducidadFormateada = cad.Value.ToString("yyyy-MM-dd");
                string fechaIngreso = DateTime.Now.ToString("yyyy-MM-dd");

                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();
                transaction = conexion.BeginTransaction();

                // Verificar si ya existe el bo_id en la tabla bodega
                string queryVerificarBodega = "SELECT COUNT(*) FROM bodega WHERE bo_id = @boId";
                MySqlCommand commandVerificarBodega = new MySqlCommand(queryVerificarBodega, conexion, transaction);
                commandVerificarBodega.Parameters.AddWithValue("@boId", numbod.Text);
                int countBodega = Convert.ToInt32(commandVerificarBodega.ExecuteScalar());

                if (countBodega > 0)
                {
                    MessageBox.Show("Ya existe un registro con el mismo número de bodega.");
                    return; // Salir del método sin guardar
                }

                // Obtener el valor máximo actual de pro_cod en la tabla producto
                string queryMaxProCod = "SELECT IFNULL(MAX(pro_cod), 0) FROM producto";
                MySqlCommand commandMaxProCod = new MySqlCommand(queryMaxProCod, conexion, transaction);
                int maxProCod = Convert.ToInt32(commandMaxProCod.ExecuteScalar());
                int newProCod = maxProCod + 1;

                // Calcular el precio sumando el costo más el 10% del costo
                CalcularPrecio(costo, precio); // Asegurarse de que el precio esté actualizado

                // Obtener la imagen del PictureBox y convertirla a byte[]
                byte[] imgBytes = null;
                if (imgBox.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        imgBox.Image.Save(ms, imgBox.Image.RawFormat);
                        imgBytes = ms.ToArray();
                    }
                }

                // Insertar el producto en la tabla producto
                string queryProducto = "INSERT INTO producto(pro_cod, pro_nom, pro_cad, pro_cos, pro_pre, pro_can, pro_img, bo_id) VALUES (@proCod, @proNom, @proCad, @proCos, @proPre, @proCan, @proImg, @boId)";
                MySqlCommand myCommandProducto = new MySqlCommand(queryProducto, conexion, transaction);
                myCommandProducto.Parameters.AddWithValue("@proCod", newProCod);
                myCommandProducto.Parameters.AddWithValue("@proNom", producto.Text);
                myCommandProducto.Parameters.AddWithValue("@proCad", fechaCaducidadFormateada);
                myCommandProducto.Parameters.AddWithValue("@proCos", costo.Value); // Guardar el costo original
                myCommandProducto.Parameters.AddWithValue("@proPre", precio.Value); // Guardar el precio calculado
                myCommandProducto.Parameters.AddWithValue("@proCan", can.Value);
                myCommandProducto.Parameters.AddWithValue("@proImg", imgBytes);
                myCommandProducto.Parameters.AddWithValue("@boId", numbod.Text);
                myCommandProducto.ExecuteNonQuery();

                // Insertar la bodega en la tabla bodega con fecha de ingreso
                string queryBodega = "INSERT INTO bodega(bo_id, bo_fecing) VALUES (@boId, @boFecing)";
                MySqlCommand myCommandBodega = new MySqlCommand(queryBodega, conexion, transaction);
                myCommandBodega.Parameters.AddWithValue("@boId", numbod.Text);
                myCommandBodega.Parameters.AddWithValue("@boFecing", fechaIngreso);
                myCommandBodega.ExecuteNonQuery();

                // Confirmar la transacción
                transaction.Commit();

                MessageBox.Show("Se guardó el producto en la base de datos correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el registro: " + ex.Message);
                // Revertir la transacción si ocurre un error
                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        public DataRow buscarProducto(string codigo)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = "SELECT pro_cod, pro_nom, pro_cad, pro_cos, pro_pre, pro_can, bo_id, pro_img FROM producto WHERE pro_cod = @codigo";
                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@codigo", codigo);

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
                MessageBox.Show("Error al buscar producto: " + ex.Message);
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
        public void modificarProducto(TextBox cod, TextBox producto, DateTimePicker cad, NumericUpDown costo, NumericUpDown precio, NumericUpDown can, TextBox numbod, PictureBox imgBox)
        {
            MySqlConnection conexion = null;
            try
            {
                string fechaFormateada = cad.Value.ToString("yyyy-MM-dd");

                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Verificar si el producto existe antes de modificarlo
                string queryVerificarProducto = "SELECT COUNT(*) FROM producto WHERE pro_cod = @proCod";
                MySqlCommand commandVerificarProducto = new MySqlCommand(queryVerificarProducto, conexion);
                commandVerificarProducto.Parameters.AddWithValue("@proCod", cod.Text);
                int countProducto = Convert.ToInt32(commandVerificarProducto.ExecuteScalar());

                if (countProducto == 0)
                {
                    MessageBox.Show("No existe ningún producto con el código especificado.");
                    return; // Salir del método si el producto no existe
                }

                // Obtener la imagen del PictureBox y convertirla a byte[]
                byte[] imgBytes = null;
                if (imgBox.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        imgBox.Image.Save(ms, imgBox.Image.RawFormat);
                        imgBytes = ms.ToArray();
                    }
                }

                // Calcular el precio basado en el costo
                CalcularPrecio(costo, precio); // Asegurarse de que el precio esté actualizado

                // Actualizar el producto en la tabla producto
                string queryActualizarProducto = "UPDATE producto SET pro_nom = @proNom, pro_cad = @proCad, pro_cos = @proCos, pro_pre = @proPre, pro_can = @proCan, pro_img = @proImg, bo_id = @boId WHERE pro_cod = @proCod";
                MySqlCommand myCommandActualizarProducto = new MySqlCommand(queryActualizarProducto, conexion);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCod", cod.Text);
                myCommandActualizarProducto.Parameters.AddWithValue("@proNom", producto.Text);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCad", fechaFormateada);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCos", costo.Value);
                myCommandActualizarProducto.Parameters.AddWithValue("@proPre", precio.Value);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCan", can.Value);
                myCommandActualizarProducto.Parameters.AddWithValue("@proImg", imgBytes);
                myCommandActualizarProducto.Parameters.AddWithValue("@boId", numbod.Text);
                myCommandActualizarProducto.ExecuteNonQuery();

                MessageBox.Show("Se actualizó el producto en la base de datos correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo actualizar el producto: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void eliminarProductos(TextBox cod, DataGridView tablaproductos)
        {
            MySqlConnection conexion = null;
            MySqlTransaction transaccion = null;
            try
            {
                if (string.IsNullOrEmpty(cod.Text))
                    throw new Exception("ID de producto no válido");

                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Iniciar la transacción
                transaccion = conexion.BeginTransaction();

                // Eliminar el registro correspondiente en la tabla bodega
                string eliminarBodegaQuery = "DELETE FROM bodega WHERE bo_id = @bo_Id;";
                MySqlCommand eliminarBodegaCommand = new MySqlCommand(eliminarBodegaQuery, conexion);
                eliminarBodegaCommand.Parameters.AddWithValue("@bo_Id", cod.Text);
                eliminarBodegaCommand.Transaction = transaccion;
                eliminarBodegaCommand.ExecuteNonQuery();

                // Eliminar el producto de la tabla producto
                string eliminarProductoQuery = "DELETE FROM producto WHERE pro_cod = @pro_Cod;";
                MySqlCommand eliminarProductoCommand = new MySqlCommand(eliminarProductoQuery, conexion);
                eliminarProductoCommand.Parameters.AddWithValue("@pro_Cod", cod.Text);
                eliminarProductoCommand.Transaction = transaccion;
                eliminarProductoCommand.ExecuteNonQuery();

                // Reiniciar el autoincremento de pro_cod
                string reiniciarAutoincrementoQuery = "ALTER TABLE producto AUTO_INCREMENT = 1;";
                MySqlCommand reiniciarAutoincrementoCommand = new MySqlCommand(reiniciarAutoincrementoQuery, conexion);
                reiniciarAutoincrementoCommand.Transaction = transaccion;
                reiniciarAutoincrementoCommand.ExecuteNonQuery();

                // Confirmar la transacción
                transaccion.Commit();

                MessageBox.Show("Se eliminó correctamente el registro de la tabla producto y bodega");
            }
            catch (Exception ex)
            {
                // Revertir la transacción en caso de error
                if (transaccion != null)
                    transaccion.Rollback();

                MessageBox.Show("Error al borrar el registro: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void buscarProductoPorId(DataGridView tablaProductos, TextBox id)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Query para buscar productos por ID
                string query = "SELECT pro_cod as 'Codigo', pro_nom as 'Nombre', pro_cad as 'Caducidad', pro_cos as 'Costo', pro_pre as 'Precio', pro_can as 'Cantidad', bo_id as 'Bodega Id', pro_img as 'Imagen' " +
                               "FROM producto " +
                               "WHERE pro_cod = @id";

                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id.Text.Trim());

                // Adaptador para llenar los datos en un DataTable
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Limpiar el DataGridView antes de mostrar los resultados
                tablaProductos.DataSource = null;
                tablaProductos.Rows.Clear();

                // Asignar el DataTable con los resultados al DataGridView
                tablaProductos.DataSource = dt;

                // Verificar si la columna de Imagen ya existe y configurarla
                if (tablaProductos.Columns.Contains("Imagen"))
                {
                    tablaProductos.Columns["Imagen"].Visible = true;
                }
                else
                {
                    // Configurar la columna de Imagen si no existe
                    DataGridViewImageColumn imageColumn = new DataGridViewImageColumn
                    {
                        Name = "Imagen",
                        HeaderText = "Imagen",
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        Visible = true
                    };
                    tablaProductos.Columns.Add(imageColumn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar producto: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        public void exportarExcel(DataGridView tabla)
        {
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                // Crear una instancia de Excel y abrir una nueva aplicación
                excelApp = new Excel.Application();
                excelApp.Visible = true; // Mostrar Excel

                // Crear un nuevo libro de Excel
                workbook = excelApp.Workbooks.Add();
                worksheet = (Excel.Worksheet)workbook.Sheets[1]; // Obtener la primera hoja de trabajo
                worksheet.Name = "Datos de Productos"; // Nombre de la hoja de trabajo

                // Obtener las columnas que no sean pro_img o Imagen
                var columnasAExportar = tabla.Columns.Cast<DataGridViewColumn>()
                                                     .Where(col => col.Name != "pro_img" && col.Name != "Imagen")
                                                     .ToList();

                // Encabezados de las columnas
                for (int i = 0; i < columnasAExportar.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = columnasAExportar[i].HeaderText;
                }

                // Datos de las filas
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    for (int j = 0; j < columnasAExportar.Count; j++)
                    {
                        // Manejo seguro de valores nulos
                        if (tabla.Rows[i].Cells[columnasAExportar[j].Index].Value != null)
                        {
                            worksheet.Cells[i + 2, j + 1] = tabla.Rows[i].Cells[columnasAExportar[j].Index].Value.ToString();
                        }
                        else
                        {
                            worksheet.Cells[i + 2, j + 1] = string.Empty; // Opcional: Puedes usar otro valor por defecto si es null
                        }
                    }
                }

                // Diseño de tabla sencillo
                Excel.Range headerRange = worksheet.Range["A1", worksheet.Cells[1, columnasAExportar.Count]];
                headerRange.Font.Bold = true;
                headerRange.Interior.Color = Excel.XlRgbColor.rgbLightBlue;

                Excel.Range tableRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[tabla.Rows.Count + 1, columnasAExportar.Count]];
                tableRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                tableRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                // Ajustar el ancho de las columnas automáticamente
                worksheet.Columns.AutoFit();

                // Guardar el archivo Excel
                workbook.SaveAs("DatosProductos.xlsx");

                // Mostrar un mensaje opcional
                // MessageBox.Show("Se exportaron los datos a Excel correctamente. Puedes encontrar el archivo en la carpeta del proyecto.");
            }
            catch (Exception ex)
            {
                // MessageBox.Show("Error al exportar a Excel: " + ex.Message);
            }
            finally
            {
                // Liberar recursos de Excel
                if (worksheet != null)
                {
                    Marshal.ReleaseComObject(worksheet);
                }
                if (workbook != null)
                {
                    // No cerrar el libro, solo liberar recursos
                    Marshal.ReleaseComObject(workbook);
                }

                // No cerrar la aplicación Excel aquí
                // No se invoca excelApp.Quit() ni se libera el objeto excelApp
            }
        }
        public void seleccionarProducto(DataGridView tablaProducto, TextBox textboxproductoId)
        {
            try
            {
                // Asume que la columna del ID de bodega es la primera columna (índice 0)
                textboxproductoId.Text = tablaProducto.CurrentRow.Cells[0].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logró seleccionar, error: " + ex.ToString());
            }
        }




    }

}
