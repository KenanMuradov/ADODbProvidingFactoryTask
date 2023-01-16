using Microsoft.Extensions.Configuration;
using System;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using static System.Reflection.Metadata.BlobBuilder;

namespace ADODbProvidingFactoryTask;

#nullable disable

public partial class MainWindow : Window
{
    DbConnection connection = null;
    DbDataAdapter adapter = null;
    DbProviderFactory providerFactory = null;
    IConfigurationRoot configuration = null;
    DataSet dataSet = null;
    string providerName = string.Empty;


    public MainWindow()
    {
        InitializeComponent();
        Configuration();
    }

    private void Configuration()
    {
        DbProviderFactories.RegisterFactory("System.Data.SqlClient", typeof(SqlClientFactory));
        providerName = "System.Data.SqlClient";

        providerFactory = DbProviderFactories.GetFactory(providerName);

        configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        dataSet = new DataSet();
        connection = providerFactory.CreateConnection();
        connection.ConnectionString = configuration.GetConnectionString(providerName);
        adapter = providerFactory.CreateDataAdapter();

        // Table ve column mapping aid tam duzgun numune fikirlese bilmedim ona gore bunlari yazib example kimi commente atdim

        // adapter.TableMappings.Add("Table", "Books");
        // adapter.TableMappings["Table"].ColumnMappings.Add("Id","Identificator");
        // 
        // adapter.TableMappings.Add("Table1", "Authors");
        // adapter.TableMappings["Table1"].ColumnMappings.Add("Id", "Identificator");
        // 
        // adapter.TableMappings.Add("Table2", "T_Cards");
        // adapter.TableMappings["Table2"].ColumnMappings.Add("Id", "Identificator");
    }

    private void btnExecute_Click(object sender, RoutedEventArgs e)
    {
        var command = providerFactory.CreateCommand();

        command.CommandText = txtCommand.Text;
        command.Connection = connection;

        adapter.SelectCommand = command;

        if (Tabs.Items.Count > 1)
        {
            for (int i = Tabs.Items.Count - 1; i > 0; i--)
                Tabs.Items.RemoveAt(i);
        }

        dataSet.Tables.Clear(); 

        try
        {
            

            adapter.Fill(dataSet);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        CreatingTabs();
    }

    private void CreatingTabs()
    {
        foreach (DataTable table in dataSet.Tables)
        {
            var tab = new TabItem();
            tab.Header = table.TableName;

            var dataGrid = new DataGrid();


            dataGrid.IsReadOnly = true;

            dataGrid.ItemsSource = table.AsDataView();

            tab.Content = dataGrid;

            Tabs.Items.Add(tab);
        }
    }
}
