using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows.Forms;

namespace dethi02
{
    public partial class frmSanpham : Form
    {
        public frmSanpham()
        {
            InitializeComponent();
        }

        private void frmSanpham_Load(object sender, EventArgs e)
        {
            dgvSanPham.AutoGenerateColumns = false;
            SetupDataGridView(); // Set up DataGridView
            LoadLoaiSP(); // Load product types into combobox
            LoadSanPham(); // Load product list into DataGridView
        }

        private void SetupDataGridView()
        {
            dgvSanPham.AutoGenerateColumns = false;

            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaSP",
                HeaderText = "Mã Sản Phẩm",
                Name = "MaSP" // Ensure the Name is set
            });

            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TenSP",
                HeaderText = "Tên Sản Phẩm",
                Name = "TenSP" // Ensure the Name is set
            });

            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ngaynhap",
                HeaderText = "Ngày Nhập",
                Name = "Ngaynhap" // Ensure the Name is set
            });

            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaLoai",
                HeaderText = "Loại Sản Phẩm",
                Name = "MaLoai" // Ensure the Name is set
            });
        }


        public void LoadLoaiSP()
        {
            using (var context = new Models.Model1())
            {
                cboLoaiSP.DataSource = context.LoaiSP.ToList();
                cboLoaiSP.DisplayMember = "TenLoai";
                cboLoaiSP.ValueMember = "MaLoai";
            }
        }

        public void LoadSanPham()
        {
            using (var context = new Models.Model1())
            {
                dgvSanPham.DataSource = context.SanPham.ToList();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateSanPham())
                return;

            try
            {
                using (var context = new Models.Model1())
                {
                    var existingSanPham = context.SanPham.FirstOrDefault(sp => sp.MaSP == txtMaSP.Text);
                    if (existingSanPham != null)
                    {
                        MessageBox.Show("Mã sản phẩm đã tồn tại. Vui lòng nhập mã khác.");
                        return;
                    }

                    var sanPham = new Models.SanPham
                    {
                        MaSP = txtMaSP.Text,
                        TenSP = txtTenSP.Text,
                        Ngaynhap = dtNgaynhap.Value,
                        MaLoai = cboLoaiSP.SelectedValue.ToString()
                    };

                    context.SanPham.Add(sanPham);
                    context.SaveChanges();
                }

                LoadSanPham();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có hàng nào được chọn không
            if (dgvSanPham.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvSanPham.SelectedRows[0]; // Lấy hàng được chọn
                try
                {
                    // Gán giá trị từ các ô của hàng đã chọn vào các ô tương ứng trong form
                    txtMaSP.Text = selectedRow.Cells["MaSP"].Value.ToString(); // Mã sản phẩm
                    txtTenSP.Text = selectedRow.Cells["TenSP"].Value.ToString(); // Tên sản phẩm
                    dtNgaynhap.Value = Convert.ToDateTime(selectedRow.Cells["Ngaynhap"].Value); // Ngày nhập
                    cboLoaiSP.SelectedValue = selectedRow.Cells["MaLoai"].Value.ToString(); // Loại sản phẩm
                }
                catch (ArgumentException ex)
                {
                    // Hiển thị thông báo lỗi nếu có lỗi xảy ra khi truy cập vào các ô trong DataGridView
                    MessageBox.Show($"Lỗi khi truy cập cột DataGridView: {ex.Message}");
                }
            }
            else
            {
                // Thông báo nếu không có sản phẩm nào được chọn
                MessageBox.Show("Vui lòng chọn một sản phẩm để chỉnh sửa.");
            }
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.SelectedRows.Count > 0)
            {
                string maSP = dgvSanPham.SelectedRows[0].Cells["MaSP"].Value.ToString();
                try
                {
                    using (var context = new Models.Model1())
                    {
                        var sanPham = context.SanPham.Find(maSP);
                        if (sanPham != null)
                        {
                            context.SanPham.Remove(sanPham);
                            context.SaveChanges();
                        }
                    }
                    LoadSanPham();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string tenSP = txtTim.Text.Trim();

            if (!string.IsNullOrEmpty(tenSP))
            {
                using (var context = new Models.Model1())
                {
                    var sanPhams = context.SanPham.Where(sp => sp.TenSP.Contains(tenSP)).ToList();
                    dgvSanPham.DataSource = sanPhams;

                    if (!sanPhams.Any())
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm cần tìm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!ValidateSanPham())
                return;

            try
            {
                using (var context = new Models.Model1())
                {
                    string maSP = txtMaSP.Text.Trim();
                    var sanPham = context.SanPham.FirstOrDefault(sp => sp.MaSP == maSP);

                    if (sanPham == null)
                    {
                        sanPham = new Models.SanPham
                        {
                            MaSP = maSP,
                            TenSP = txtTenSP.Text.Trim(),
                            Ngaynhap = dtNgaynhap.Value,
                            MaLoai = cboLoaiSP.SelectedValue.ToString()
                        };
                        context.SanPham.Add(sanPham);
                    }
                    else
                    {
                        sanPham.TenSP = txtTenSP.Text.Trim();
                        sanPham.Ngaynhap = dtNgaynhap.Value;
                        sanPham.MaLoai = cboLoaiSP.SelectedValue.ToString();
                    }

                    context.SaveChanges();
                }

                LoadSanPham();
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        private void btnKLuu_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn hủy các thay đổi không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ClearForm();
            }
        }

        public void ClearForm()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            dtNgaynhap.Value = DateTime.Now;
            cboLoaiSP.SelectedIndex = -1;
        }

        public bool ValidateSanPham()
        {
            if (string.IsNullOrEmpty(txtMaSP.Text) || txtMaSP.Text.Length > 10)
            {
                MessageBox.Show("Mã sản phẩm không được để trống hoặc vượt quá 10 ký tự.");
                return false;
            }

            if (string.IsNullOrEmpty(txtTenSP.Text) || txtTenSP.Text.Length > 50)
            {
                MessageBox.Show("Tên sản phẩm không được để trống hoặc vượt quá 50 ký tự.");
                return false;
            }

            return true;
        }

        private void dgvSanPham_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvSanPham.Rows[e.RowIndex];

                txtMaSP.Text = selectedRow.Cells["MaSP"].Value.ToString();
                txtTenSP.Text = selectedRow.Cells["TenSP"].Value.ToString();
                dtNgaynhap.Value = Convert.ToDateTime(selectedRow.Cells["Ngaynhap"].Value);
                cboLoaiSP.SelectedValue = selectedRow.Cells["MaLoai"].Value.ToString();
            }
        }

        private void ShowErrorMessage(Exception ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
            MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}\nChi tiết: {innerException}");
        }
    }
}
