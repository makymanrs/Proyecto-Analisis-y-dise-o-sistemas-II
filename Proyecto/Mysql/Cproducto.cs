using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Transactions;
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

                string query = @"
            SELECT 
                p.pro_cod as 'Código Producto', 
                p.pro_nom as 'Nombre Producto', 
                p.pro_cad as 'Caducidad', 
                p.pro_cos as 'Costo', 
                p.pro_pre as 'Precio', 
                p.pro_can as 'Cantidad', 
                pr.prove_nom as 'Proveedor', 
                
                p.pro_img 
            FROM 
                producto p
                INNER JOIN proveedor pr ON p.prove_id = pr.prove_id";

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

                // Set the DataGridView DataSource to the DataTable
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

                // Handle cell formatting event for the DataGridView
                tablaproductos.CellFormatting += new DataGridViewCellFormattingEventHandler(tablaproductos_CellFormatting);
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
        public void cargarNombresProveedores(ComboBox comboBoxProveedores)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = "SELECT prove_nom FROM proveedor";
                MySqlCommand command = new MySqlCommand(query, conexion);

                // Limpiar el ComboBox antes de cargar datos nuevos
                comboBoxProveedores.Items.Clear();

                // Abrir la conexión y ejecutar el comando
               // conexion.Open();
                MySqlDataReader reader = command.ExecuteReader();
               // comboBoxProveedores.Items.Add("No requiere");
                // Iterar a través de los resultados y agregar nombres al ComboBox
                while (reader.Read())
                {
                    string nombreProveedor = reader.GetString("prove_nom");
                    comboBoxProveedores.Items.Add(nombreProveedor);
                }
                
                // Seleccionar el primer elemento por defecto si hay elementos cargados
                if (comboBoxProveedores.Items.Count > 0)
                {
                    comboBoxProveedores.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar nombres de proveedores: " + ex.Message);
            }
            finally
            {
                if (conexion != null && conexion.State == ConnectionState.Open)
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
        public void guardarproductos(TextBox cod, TextBox producto, DateTimePicker cad, NumericUpDown costo, NumericUpDown precio, NumericUpDown can, TextBox numbod, ComboBox comboBoxProveedores, PictureBox imgBox)
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

                // Obtener el ID del proveedor seleccionado
                string queryProveedor = "SELECT prove_id FROM proveedor WHERE prove_nom = @proveNom";
                MySqlCommand commandProveedor = new MySqlCommand(queryProveedor, conexion, transaction);
                commandProveedor.Parameters.AddWithValue("@proveNom", comboBoxProveedores.SelectedItem.ToString());
                int proveId = Convert.ToInt32(commandProveedor.ExecuteScalar());

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
                string queryProducto = "INSERT INTO producto(pro_cod, pro_nom, pro_cad, pro_cos, pro_pre, pro_can, pro_img, prove_id, bo_id) VALUES (@proCod, @proNom, @proCad, @proCos, @proPre, @proCan, @proImg, @proveId, @boId)";
                MySqlCommand myCommandProducto = new MySqlCommand(queryProducto, conexion, transaction);
                myCommandProducto.Parameters.AddWithValue("@proCod", newProCod);
                myCommandProducto.Parameters.AddWithValue("@proNom", producto.Text);
                myCommandProducto.Parameters.AddWithValue("@proCad", fechaCaducidadFormateada);
                myCommandProducto.Parameters.AddWithValue("@proCos", costo.Value); // Guardar el costo original
                myCommandProducto.Parameters.AddWithValue("@proPre", precio.Value); // Guardar el precio calculado
                myCommandProducto.Parameters.AddWithValue("@proCan", can.Value);
                myCommandProducto.Parameters.AddWithValue("@proImg", imgBytes);
                myCommandProducto.Parameters.AddWithValue("@proveId", proveId);
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

                string query = "SELECT p.pro_cod, p.pro_nom, p.pro_cad, p.pro_cos, p.pro_pre, p.pro_can, p.bo_id, p.pro_img, pr.prove_nom " +
                               "FROM producto p " +
                               "JOIN proveedor pr ON p.prove_id = pr.prove_id " +
                               "WHERE p.pro_cod = @codigo";
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
        public void modificarProducto(TextBox cod, TextBox producto, DateTimePicker cad, NumericUpDown costo, NumericUpDown precio, NumericUpDown can, TextBox numbod, ComboBox comboBoxProveedores, PictureBox imgBox)
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

                // Obtener el ID del proveedor seleccionado
                string queryProveedor = "SELECT prove_id FROM proveedor WHERE prove_nom = @proveNom";
                MySqlCommand commandProveedor = new MySqlCommand(queryProveedor, conexion);
                commandProveedor.Parameters.AddWithValue("@proveNom", comboBoxProveedores.SelectedItem.ToString());
                int proveId = Convert.ToInt32(commandProveedor.ExecuteScalar());

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
                string queryActualizarProducto = "UPDATE producto SET pro_nom = @proNom, pro_cad = @proCad, pro_cos = @proCos, pro_pre = @proPre, pro_can = @proCan, pro_img = @proImg, prove_id = @proveId, bo_id = @boId WHERE pro_cod = @proCod";
                MySqlCommand myCommandActualizarProducto = new MySqlCommand(queryActualizarProducto, conexion);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCod", cod.Text);
                myCommandActualizarProducto.Parameters.AddWithValue("@proNom", producto.Text);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCad", fechaFormateada);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCos", costo.Value);
                myCommandActualizarProducto.Parameters.AddWithValue("@proPre", precio.Value);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCan", can.Value);
                myCommandActualizarProducto.Parameters.AddWithValue("@proImg", imgBytes);
                myCommandActualizarProducto.Parameters.AddWithValue("@proveId", proveId);
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
                string query = "SELECT p.pro_cod as 'Codigo', p.pro_nom as 'Nombre Producto', p.pro_cad as 'Caducidad', p.pro_cos as 'Costo', p.pro_pre as 'Precio', p.pro_can as 'Cantidad', pr.prove_nom as 'Proveedor', p.pro_img as 'Imagen' " +
                "FROM producto p " +
                "JOIN proveedor pr ON p.prove_id = pr.prove_id " +
                "WHERE p.pro_cod = @id";

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
            int columnIndex = tablaProducto.CurrentCell.ColumnIndex;
            try
            {
                if(columnIndex==0){
                    textboxproductoId.Text = tablaProducto.CurrentRow.Cells[0].Value.ToString();
                }
                // Asume que la columna del ID de bodega es la primera columna (índice 0)
               
                else if (columnIndex == 1)
                {
                    textboxproductoId.Text = tablaProducto.CurrentRow.Cells[1].Value.ToString();
                }
                else if (columnIndex == 6)
                {
                    textboxproductoId.Text = tablaProducto.CurrentRow.Cells[6].Value.ToString();
                }
                else
                {
                    // Puedes manejar el caso cuando no es ninguna de las columnas deseadas
                    MessageBox.Show("No se seleccionó una columna válida.");
                }
            }
            
            catch (Exception ex)
            {
                MessageBox.Show("No se logró seleccionar, error: " + ex.ToString());
            }
        }

        private void tablaproductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv.Columns[e.ColumnIndex].Name == "Cantidad")
            {
                if (e.Value != null && e.Value.ToString() == "0")
                {
                    e.CellStyle.BackColor = Color.Red;
                }

            }
        }
        public void BuscarProductoPorFiltros(DataGridView datagridviewProducto, TextBox textBoxFiltro, ComboBox comboBoxFiltro)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Construir la consulta base
                string query = "SELECT producto.pro_cod as 'Código Producto', producto.pro_nom as 'Nombre Producto', producto.pro_cad as 'Caducidad', " +
                               "producto.pro_cos as 'Costo', producto.pro_pre as 'Precio', producto.pro_can as 'Cantidad', " +
                               "proveedor.prove_nom as 'Proveedor', producto.pro_img as 'Imagen' " +
                               "FROM producto " +
                               "INNER JOIN proveedor ON producto.prove_id = proveedor.prove_id " +
                               "WHERE 1=1";

                // Determinar el filtro basado en la selección del ComboBox
                if (comboBoxFiltro.SelectedItem != null)
                {
                    string filtro = textBoxFiltro.Text.Trim();
                    if (comboBoxFiltro.SelectedItem.ToString() == "Nombre del producto" && !string.IsNullOrWhiteSpace(filtro))
                    {
                        query += " AND producto.pro_nom LIKE @filtro";
                    }
                    else if (comboBoxFiltro.SelectedItem.ToString() == "Código del producto" && !string.IsNullOrWhiteSpace(filtro))
                    {
                        query += " AND producto.pro_cod = @filtro";
                    }
                    else if (comboBoxFiltro.SelectedItem.ToString() == "Nombre del proveedor" && !string.IsNullOrWhiteSpace(filtro))
                    {
                        query += " AND proveedor.prove_nom LIKE @filtro";
                    }
                }

                MySqlCommand command = new MySqlCommand(query, conexion);

                // Asignar valores a los parámetros
                if (!string.IsNullOrWhiteSpace(textBoxFiltro.Text))
                {
                    string filtro = textBoxFiltro.Text.Trim();
                    if (comboBoxFiltro.SelectedItem.ToString() == "Nombre del producto" || comboBoxFiltro.SelectedItem.ToString() == "Nombre del proveedor")
                    {
                        command.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                    }
                    else if (comboBoxFiltro.SelectedItem.ToString() == "Código del producto")
                    {
                        command.Parameters.AddWithValue("@filtro", filtro);
                    }
                }

                // Adaptador para llenar los datos en un DataTable
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Limpiar el DataGridView antes de mostrar los resultados
                datagridviewProducto.DataSource = null;
                datagridviewProducto.Rows.Clear();

                // Asignar el DataTable con los resultados al DataGridView
                datagridviewProducto.DataSource = dt;

                // Ajustar el DataGridView para manejar la columna de imagen
                if (datagridviewProducto.Columns.Contains("Imagen"))
                {
                    datagridviewProducto.Columns["Imagen"].Visible = true;
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
                    datagridviewProducto.Columns.Add(imageColumn);
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
    }
}
