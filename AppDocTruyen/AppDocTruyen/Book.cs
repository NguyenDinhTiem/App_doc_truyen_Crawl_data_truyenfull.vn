using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDocTruyen
{
    public class Book 
    {
        public string bookName;
        public string bookURL;
        public int sTT;
        public string thongtintruyen;
        public string danhsachchuong;
        public string DanhSachChuong { get => danhsachchuong; set => danhsachchuong = value; }

        public string tenchuong;
        public string TenChuong { get => tenchuong; set => tenchuong = value; }
        public string noidungchuong;
        public int sTTchuong;

        public string BookName { get => bookName; set => bookName = value; }
        public string BookURL { get => bookURL; set => bookURL = value; }
        public int STT { get => sTT; set => sTT = value; }
        public string ThongTinTruyen { get => thongtintruyen; set => thongtintruyen = value; }
  
        public string NoiDungChuong { get => noidungchuong; set => noidungchuong = value; }
        public int STTChuong { get => sTTchuong; set => sTTchuong = value; }
      


    }
}
