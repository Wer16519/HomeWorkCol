using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace t12
{
    public partial class MainWindow : Window
    {
        private string connectionString;
        private DataTable currentTable;
        private string currentTableName;
        private OleDbDataAdapter adapter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenDb_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Базы данных Access (*.mdb;*.accdb)|*.mdb;*.accdb"
            };

            if (ofd.ShowDialog() == true)
            {
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={ofd.FileName};";

                cbTables.Items.Clear();

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    DataTable schema = conn.GetSchema("Tables");
                    foreach (DataRow row in schema.Rows)
                    {
                        if (row["TABLE_TYPE"].ToString() == "TABLE")
                            cbTables.Items.Add(row["TABLE_NAME"].ToString());
                    }
                }
            }
        }

        private void CbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTables.SelectedItem == null) return;

            currentTableName = cbTables.SelectedItem.ToString();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                adapter = new OleDbDataAdapter($"SELECT * FROM [{currentTableName}]", conn);
                OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);

                currentTable = new DataTable();
                adapter.Fill(currentTable);

                dataGrid.ItemsSource = currentTable.DefaultView;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (adapter != null && currentTable != null)
            {
                try
                {
                    adapter.Update(currentTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }
            base.OnClosing(e);
        }
    }
}