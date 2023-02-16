using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportTableSchema
{
    public partial class Form1 : Form
    {
        private string selectedItem;
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            IEnumerable<IFace> faces = GetFaces();
            selectedItem = faces.First().GetType().FullName;
            var i = 0;
            foreach(var face in faces)
            {
                var rootPath = System.Environment.CurrentDirectory;
                PictureBox picBox = new PictureBox();
                picBox.Size = new Size(120,120);
                picBox.BackgroundImageLayout = ImageLayout.Stretch;
                picBox.SizeMode = PictureBoxSizeMode.StretchImage;
                picBox.Image = new Bitmap(rootPath+ face.GetDbIco());
                picBox.Name = face.GetType().FullName;
                picBox.Top = 122 * i;
                picBox.Click += (sender, e) => {

                    var picBoxs = GetControlsOfType<PictureBox>(this);
                    foreach (var pb in picBoxs)
                    {
                        pb.BorderStyle = BorderStyle.None;
                    }
                    var picBox = ((PictureBox)sender);
                    selectedItem = picBox.Name;
                    picBox.BorderStyle = BorderStyle.Fixed3D;
                };
                if (i == 0)
                {
                    picBox.BorderStyle = BorderStyle.Fixed3D;
                }
                menuPanel.Controls.Add(picBox);
                i++;
            }
        }
        private IEnumerable<IFace> GetFaces()
        {
            List<IFace> faces = new List<IFace>();
            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IFace))))
                        .ToArray();
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集
            foreach (Type type in types)
            {
                string name = type.FullName;
                IFace face = (IFace)assembly.CreateInstance(name);
                faces.Add(face);
            }
            return faces;
        }
        private async void Ok_Click(object sender, EventArgs e)
        {
            Check();
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集
            IFace face = (IFace)assembly.CreateInstance(selectedItem);
            var ds = face.GetTableSchema(this.txtIP.Text, this.txtPort.Text, this.txtDbName.Text, this.txtAccount.Text, this.txtPassword.Text);
            ds.DataSetName = this.txtDbName.Text + "表结构";
            ProcessTableName(ds);
            var rootPath = System.Environment.CurrentDirectory+"\\download\\";
            if(!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            var filePath = rootPath + ds.DataSetName + DateTime.Now.ToString("yyyyMMddHHmmsss") + ".xls";
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (var inputStream = ExcelHelper.ExcelStream(ds))
                {
                    await inputStream.CopyToAsync(fs);
                }
                    
            }
            MessageBox.Show("导出成功");
        }
        private void ProcessTableName(DataSet ds)
        {
            if(ds?.Tables?.Count>0)
            {
                for(var i = 0;i<ds.Tables.Count;i++)
                {   
                    var table = ds.Tables[i];
                    //查询出的table前两个自定必须是表名称和表说明
                    table.TableName = table.Rows[0][0].ToString() + " " + table.Rows[0][1].ToString();
                }
            }
        }
        private void Check()
        {
            if(string.IsNullOrEmpty(this.txtIP.Text))
            {
                MessageBox.Show("IP未填");
            }
            if (string.IsNullOrEmpty(this.txtPort.Text))
            {
                MessageBox.Show("端口未填");
            }
            if (string.IsNullOrEmpty(this.txtDbName.Text))
            {
                MessageBox.Show("数据库名称未填");
            }
            if (string.IsNullOrEmpty(this.txtAccount.Text))
            {
                MessageBox.Show("账号未填");
            }
            if (string.IsNullOrEmpty(this.txtPassword.Text))
            {
                MessageBox.Show("密码未填");
            }
            this.txtIP.Text = this.txtIP.Text.Trim();
            this.txtPort.Text = this.txtPort.Text.Trim();
            this.txtDbName.Text = this.txtDbName.Text.Trim();
            this.txtAccount.Text = this.txtAccount.Text.Trim();
            this.txtPassword.Text = this.txtPassword.Text.Trim();
        }

        private static IEnumerable<T> GetControlsOfType<T>(Control root) where T : Control
        {
            var t = root as T;
            if (t != null)
                yield return t;

            if (root is ContainerControl || root is Control)
            {
                var container = root as Control;
                foreach (Control c in container.Controls)
                    foreach (var i in GetControlsOfType<T>(c))
                        yield return i;
            }
        }
    }
}
