using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Threading;

namespace AppLogViewer
{
    public partial class Form1 : Form, IDisposable
    {
        string connectionString = @"Data Source=hsphmltsql1;Initial Catalog=HSPMelt;Persist Security Info=True;User ID=HSPMelt_rw_datauser;Password=HSPMelt_rw_datauser";
        string executableName = null;
        string query = null;
        DispatcherTimer StartPolling = new DispatcherTimer();

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void GetData()
        {
            query = $"SELECT TOP 1000 [DateTimeStamp], [ExecutableName], [ModuleName], [MessageText] " +
                    $"FROM ApplicationLog " +
                    $"order by DateTimeStamp desc";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);

                DataTable data = new DataTable();
                adapter.Fill(data);

                dataGridView1.DataSource = data;
            }

            query = "SELECT DISTINCT ExecutableName " +
                    "FROM [HSPMelt].[dbo].[ApplicationLog] " +
                    "ORDER BY ExecutableName asc ";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader DR = cmd.ExecuteReader();

                while (DR.Read())
                {

                    comboBox1.Items.Add(DR[0]);

                }
            }

        }

        private void btnGetLogs_Click(object sender, EventArgs e)
        {
            query = $"SELECT TOP 1000 [DateTimeStamp], [ExecutableName], [ModuleName], [MessageText] " +
                    $"FROM ApplicationLog " +
                    $"Where ExecutableName = '{executableName}'" +
                    $"order by DateTimeStamp desc";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);

                DataTable data = new DataTable();
                adapter.Fill(data);

                dataGridView1.DataSource = data;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            executableName = comboBox1.SelectedItem.ToString();
        }

        private void btnGetAll_Click(object sender, EventArgs e)
        {
            query = $"SELECT TOP 1000 [DateTimeStamp], [ExecutableName], [ModuleName], [MessageText] " +
                    $"FROM ApplicationLog " +
                    $"order by DateTimeStamp desc";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);

                DataTable data = new DataTable();
                adapter.Fill(data);

                dataGridView1.DataSource = data;
            }
        }

        private void btnStartPolling_Click(object sender, EventArgs e)
        {
            StartPolling.Interval = System.TimeSpan.FromMilliseconds(5000);
            StartPolling.Tick += new System.EventHandler(StartPolling_Tick);
            StartPolling.Start();
        }

        private void StartPolling_Tick(object sender, EventArgs e)
        {
            StartPolling.Stop();
            GetData();
            StartPolling.Start();
        }

        private void btnStopPolling_Click(object sender, EventArgs e)
        {
            StartPolling.Stop();
        }
    }
}
