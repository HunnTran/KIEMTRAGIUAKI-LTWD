using DE02.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DE02
{
    public partial class frmSanpham : Form
    {
        public frmSanpham()
        {
            InitializeComponent();
        }
        Model1 context = new Model1();
        private void frmSanpham_Load(object sender, EventArgs e)
        {
            List<SANPHAM> sanphams = context.SANPHAMs.ToList();
            List<LOAISP> loaisanphams = context.LOAISPs.ToList();

            FillSanPham(sanphams);
            FillLoaiSanPham(loaisanphams);
        }

        private void FillSanPham(List<SANPHAM> sanphams)
        {
            dgv_sanpham.Rows.Clear(); 
            foreach (var sanpham in sanphams)
            {
                string tenLoai = context.LOAISPs
            .Where(loai => loai.MALOAI == sanpham.MALOAI)
            .Select(loai => loai.TENLOAI)
            .FirstOrDefault();

                int RowNew = dgv_sanpham.Rows.Add();
                dgv_sanpham.Rows[RowNew].Cells[0].Value = sanpham.MASP;
                dgv_sanpham.Rows[RowNew].Cells[1].Value = sanpham.TENSP;
                dgv_sanpham.Rows[RowNew].Cells[2].Value = sanpham.NGAYNHAP.ToString("dd/MM/yyyy");
                dgv_sanpham.Rows[RowNew].Cells[3].Value = tenLoai;
            }
        }

        private void FillLoaiSanPham(List<LOAISP> loaisanphams)
        {
            cmb_01.DataSource = loaisanphams;
            cmb_01.DisplayMember = "TENLOAI";
            cmb_01.ValueMember = "MALOAI";
        }

        private List<SANPHAM> tempSanPhams = new List<SANPHAM>();   
        private void btn_them_Click(object sender, EventArgs e)
        {
                try
                {
                    if (CheckInputData())
                    {
                        
                        SANPHAM newSanPham = new SANPHAM
                        {
                            MASP = txt_masp.Text.Trim(),
                            TENSP = txt_tensp.Text,
                            NGAYNHAP = dtp_01.Value,
                            MALOAI = cmb_01.SelectedValue.ToString()
                        };

                        
                        tempSanPhams.Add(newSanPham);

                        
                        FillSanPham(tempSanPhams);

                        
                        LoadForm();

                        MessageBox.Show("Đã thêm sản phẩm vào danh sách tạm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
        }

        private void LoadForm()
        {
            txt_masp.Clear();
            txt_tensp.Clear();
            dtp_01.Value = DateTime.Now;
            cmb_01.SelectedIndex = 0;

        }

        private void RefreshData()
        {
            List<SANPHAM> sanphams = context.SANPHAMs.ToList();
            FillSanPham(sanphams);
            LoadForm();
        }


        private bool CheckInputData()
        {
            if (string.IsNullOrEmpty(txt_masp.Text) || string.IsNullOrEmpty(txt_tensp.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_sanpham.SelectedRows.Count > 0)
                {
                    string masp = txt_masp.Text;

                    
                    var sanpham = context.SANPHAMs.SingleOrDefault(sp => sp.MASP == masp);
                    if (sanpham != null)
                    {
                        sanpham.TENSP = txt_tensp.Text;
                        sanpham.NGAYNHAP = dtp_01.Value;
                        sanpham.MALOAI = cmb_01.SelectedValue.ToString();

                        context.SANPHAMs.AddOrUpdate(sanpham); 
                        context.SaveChanges();

                        MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        RefreshData();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_sanpham.SelectedRows.Count > 0)
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string masp = dgv_sanpham.SelectedRows[0].Cells[0].Value.ToString();

                       
                        var sanpham = context.SANPHAMs.SingleOrDefault(sp => sp.MASP == masp);
                        if (sanpham != null)
                        {
                            context.SANPHAMs.Remove(sanpham);
                            context.SaveChanges();

                            MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            RefreshData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            try
            {
                string keyword = richtxt_search.Text.Trim();
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    RefreshData();
                    return;
                }

                var filteredSanPhams = context.SANPHAMs.Where(sp => sp.TENSP.Contains(keyword)).ToList();

                if (filteredSanPhams.Count > 0)
                {
                    FillSanPham(filteredSanPhams);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm hiện có!","Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgv_sanpham.Rows.Clear();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_luu_Click(object sender, EventArgs e)
        {
            try
            {
                if (tempSanPhams.Count > 0)
                {
                    foreach (var sanpham in tempSanPhams)
                    {
                        context.SANPHAMs.Add(sanpham);
                    }

                    context.SaveChanges();
                    tempSanPhams.Clear();
                    FillSanPham(context.SANPHAMs.ToList());

                    MessageBox.Show("Dữ liệu đã được lưu vào cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không có sản phẩm nào để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_khongluu_Click(object sender, EventArgs e)
        {
            if (tempSanPhams.Count > 0)
            {
                DialogResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn hủy các sản phẩm vừa thêm?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    tempSanPhams.Clear();
                    FillSanPham(context.SANPHAMs.ToList());

                    MessageBox.Show("Các thay đổi đã được hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Không có sản phẩm nào để hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Thoát_Click(object sender, EventArgs e)
        {
            if (tempSanPhams.Count > 0)
            {                
                DialogResult result = MessageBox.Show(
                    "Bạn có sản phẩm chưa lưu vào cơ sở dữ liệu. Bạn có chắc chắn muốn thoát mà không lưu?",
                    "Cảnh báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {              
                DialogResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn thoát?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    this.Close();
                }
            }
        }
    }
}
