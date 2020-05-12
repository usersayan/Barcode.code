using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;

namespace Barcode
{
    public partial class Barcode : Form
    {
        public Barcode()
        {
            InitializeComponent();
        }
        string ss = ConfigurationManager.ConnectionStrings["kkk"].ConnectionString;

   
        public void GridCounterStock()
        {
            SqlConnection con = new SqlConnection(ss);
            con.Open();
            SqlCommand cmd = new SqlCommand("select Qty from counter_stock ", con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                SqlConnection con1 = new SqlConnection(ss);
                con1.Open();
                SqlCommand cmd1 = new SqlCommand("delete from counter_stock where Qty<=0 ", con1);
                cmd1.ExecuteNonQuery();
                con1.Close();

                SqlConnection con2 = new SqlConnection(ss);
                con2.Open();
                SqlDataAdapter da2 = new SqlDataAdapter("exec counterbarcode ", con2);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2, "counter_stock");
                dataGridView1.DataSource = ds2.Tables["counter_stock"];
                con2.Close();
            }
            else
            {
                SqlConnection con3 = new SqlConnection(ss);
                con3.Open();
                SqlDataAdapter da3 = new SqlDataAdapter(" exec counterbarcode", con3);
                DataSet ds3 = new DataSet();
                da3.Fill(ds3, "counter_stock");
                dataGridView1.DataSource = ds3.Tables["counter_stock"];
                con3.Close();
            }
        }

        private void Barcode_Load(object sender, EventArgs e)
        {
            SqlConnection con31 = new SqlConnection(ss);
            con31.Open();
            SqlDataAdapter da31 = new SqlDataAdapter("select distinct(product) from counter_stock ", con31);
            DataSet ds31 = new DataSet();
            da31.Fill(ds31, "counter_stock");
            comboBox1.DataSource = ds31.Tables["counter_stock"];
            comboBox1.DisplayMember = "product";
            con31.Close();

            SqlConnection con12 = new SqlConnection(ss);
            con12.Open();
            SqlDataAdapter da12 = new SqlDataAdapter("select distinct(counter) from counter_stock", con12);
            DataSet ds12 = new DataSet();
            da12.Fill(ds12, "counter_stock");
            comboBox2.DataSource = ds12.Tables["counter_stock"];
            comboBox2.DisplayMember = "counter";
            con12.Close();

            SqlConnection con3 = new SqlConnection(ss);
            con3.Open();
            SqlDataAdapter da3 = new SqlDataAdapter("select distinct(Types) from counter_stock", con3);
            DataSet ds3 = new DataSet();
            da3.Fill(ds3, "counter_stock");
            comboBox14.DataSource = ds3.Tables["counter_stock"];
            comboBox14.DisplayMember = "Types";
            con3.Close();



            GridCounterStock();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection con10 = new SqlConnection(ss);
            con10.Open();
            SqlDataAdapter da10 = new SqlDataAdapter("select distinct(Brand) from counter_stock where Product='" + comboBox1.Text + "'", con10);
            DataSet ds10 = new DataSet();
            da10.Fill(ds10, "counter_stock");
            comboBox8.DataSource = ds10.Tables["counter_stock"];
            comboBox8.DisplayMember = "Brand";
            con10.Close();
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection con1 = new SqlConnection(ss);
            con1.Open();
            SqlDataAdapter da1 = new SqlDataAdapter("select product,brand,types,description,barcode,qty,saleprice from counter_stock  where product like '" + comboBox1.Text + "%'  and Brand like '" + comboBox8.Text + "%' order by Brand  ", con1);
            DataSet ds1 = new DataSet();
            da1.Fill(ds1, "counter_stock");
            dataGridView1.DataSource = ds1.Tables["counter_stock"];
            con1.Close();
        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ss);
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("select product,brand,types,description,barcode,qty,saleprice from  counter_stock where Types like '" + comboBox14.Text + "%' order by Brand  ", con);
            DataSet ds = new DataSet();
            da.Fill(ds, "counter_stock");
            dataGridView1.DataSource = ds.Tables["counter_stock"];
            con.Close();
        }

        public static string prod, brand, paper;
        private void button3_Click(object sender, EventArgs e)
        {
              SqlConnection con11 = new SqlConnection(ss);
                con11.Open();
                SqlCommand cmd11 = new SqlCommand("truncate table barcode_counterstock ", con11);
                cmd11.ExecuteNonQuery();
                con11.Close();


                if (dataGridView1.RowCount > 0)
                {
                    int inserted = 0;



                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        bool isSelected = Convert.ToBoolean(row.Cells["chk"].Value);
                        if (isSelected)
                        {
                            using (SqlConnection con = new SqlConnection(ss))
                            {
                                using (SqlCommand cmd = new SqlCommand("insert into barcode_counterstock values(@counter,@product,@brand,@types,@description,@barcode,@qty,@saleprice)", con))
                                {

                                    cmd.Parameters.AddWithValue("@counter", comboBox2.Text);
                                    cmd.Parameters.AddWithValue("@product", row.Cells[1].Value);
                                    cmd.Parameters.AddWithValue("@brand", row.Cells[2].Value);
                                    cmd.Parameters.AddWithValue("@types", row.Cells[3].Value);
                                    cmd.Parameters.AddWithValue("@description", row.Cells[4].Value);
                                    cmd.Parameters.AddWithValue("@barcode", row.Cells[5].Value);
                                    cmd.Parameters.AddWithValue("@qty", row.Cells[6].Value);
                                    //cmd.Parameters.AddWithValue("@Unit", row.Cells[7].Value);
                                    cmd.Parameters.AddWithValue("@saleprice", row.Cells[7].Value);
                                   


                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();

                                }
                            }
                            inserted++;
                        }

                    }

                    if (inserted > 0)
                    {
                        MessageBox.Show(string.Format("{0} Barcode Printing.", inserted), "Message");

                        prod = comboBox1.Text;
                        brand = comboBox8.Text;

                        Print_Barcode pb = new Print_Barcode();
                        pb.Show();


                    }
                    else
                    {
                        MessageBox.Show("Please select Checkbox to Print Barcode");
                    }
                }
                else
                {
                    MessageBox.Show("No records found");
                }
            
          
        }

   

    }
}
