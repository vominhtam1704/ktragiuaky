using Lab06.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Lab06
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadLoaiSach();
            LoadSach();
        }

        private void LoadLoaiSach()
        {
            using (var context = new Thuvien())
            {
                var loaiSachList = context.LoaiSaches.ToList();
                cmbTheloai.DataSource = loaiSachList;
                cmbTheloai.DisplayMember = "TenLoai";
                cmbTheloai.ValueMember = "MaLoai";
            }
        }

        private void LoadSach()
        {
            using (var context = new Thuvien())
            {
                var sachList = context.Saches.Include("LoaiSach").ToList();
                dgvQuanLySach.DataSource = sachList.Select(s => new
                {
                    s.MaSach,
                    s.TenSach,
                    s.NamXB,
                    s.MaLoai,
                    TenLoai = s.LoaiSach.TenLoai
                }).ToList();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMasach.Text) || string.IsNullOrEmpty(txtTensach.Text) || string.IsNullOrEmpty(txtNXB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sách!");
                return;
            }

            if (txtMasach.Text.Length != 6)
            {
                MessageBox.Show("Mã sách phải có 6 kí tự!");
                return;                                                                                                                                                                                                
            }

            using (var context = new Thuvien())
            {
                var sach = new Sach
                {
                    MaSach = txtMasach.Text,
                    TenSach = txtTensach.Text,
                    NamXB = int.Parse(txtNXB.Text),
                    MaLoai = (int)cmbTheloai.SelectedValue
                };
                context.Saches.Add(sach);
                context.SaveChanges();
                MessageBox.Show("Thêm mới thành công!");
                LoadSach();
                ResetFields();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMasach.Text))
            {
                MessageBox.Show("Vui lòng chọn sách để xóa!");
                return;
            }

            using (var context = new Thuvien())
            {
                var sach = context.Saches.Find(txtMasach.Text);
                if (sach != null)
                {
                    var result = MessageBox.Show("Bạn có muốn xóa không?", "Xác nhận", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        context.Saches.Remove(sach);
                        context.SaveChanges();
                        MessageBox.Show("Xóa thành công!");
                        LoadSach();
                        ResetFields();
                    }
                }
                else
                {
                    MessageBox.Show("Sách cần xóa không tồn tại!");
                }
            }
        }

       

        private void ResetFields()
        {
            txtMasach.Clear();
            txtTensach.Clear();
            txtNXB.Clear();
            cmbTheloai.SelectedIndex = -1;
        }

        private void dgvQuanLySach_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có nhấp vào một ô hợp lệ không
            if (e.RowIndex >= 0)
            {
                // Lấy dòng được chọn
                var row = dgvQuanLySach.Rows[e.RowIndex];

                // Cập nhật các trường nhập liệu với dữ liệu từ hàng được chọn
                txtMasach.Text = row.Cells["MaSach"].Value.ToString();
                txtTensach.Text = row.Cells["TenSach"].Value.ToString();
                txtNXB.Text = row.Cells["NamXB"].Value.ToString();
                cmbTheloai.SelectedValue = row.Cells["MaLoai"].Value; // Assuming "MaLoai" is available
            }
        }

        private void TimKiemSach(string keyword)
        {
            using (var context = new Thuvien())
            {
                var sachList = context.Saches.Include("LoaiSach").ToList();

                var ketQua = sachList.Where(s =>
                    s.MaSach.Contains(keyword) ||
                    s.TenSach.Contains(keyword) ||
                    s.NamXB.ToString().Contains(keyword))
                    .Select(s => new
                    {
                        s.MaSach,
                        s.TenSach,
                        s.NamXB,
                        s.MaLoai,
                        TenLoai = s.LoaiSach.TenLoai
                    }).ToList();

                // Cập nhật DataGridView với kết quả tìm kiếm
                dgvQuanLySach.DataSource = ketQua;
            }
        }

        private void thốngKêSáchTheoNămToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ThongKeSach thongKeSach = new ThongKeSach();    
            thongKeSach.Show();
        }

        private void bntSua_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMasach.Text) || string.IsNullOrEmpty(txtTensach.Text) || string.IsNullOrEmpty(txtNXB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sách!");
                return;
            }

            if (txtMasach.Text.Length != 6)
            {
                MessageBox.Show("Mã sách phải có 6 ký tự!");
                return;
            }

            int namXB;
            if (!int.TryParse(txtNXB.Text, out namXB))
            {
                MessageBox.Show("Năm xuất bản không hợp lệ! Vui lòng nhập một số nguyên.");
                return;
            }

            using (var context = new Thuvien())
            {
                // Tìm sách theo mã sách
                var sach = context.Saches.FirstOrDefault(s => s.MaSach == txtMasach.Text);
                if (sach == null)
                {
                    MessageBox.Show("Sách không tồn tại!");
                    return;
                }


                sach.TenSach = txtTensach.Text;
                sach.NamXB = namXB;
                sach.MaLoai = (int)cmbTheloai.SelectedValue;

                context.SaveChanges();
                MessageBox.Show("Cập nhật thành công!");
                LoadSach();
                ResetFields();
            }

        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            TimKiemSach(txtTimKiem.Text);
        }
    }
}

