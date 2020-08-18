using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xNet;

namespace AppDocTruyen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool isChecktruyenhot;
        private bool isChecktruyenmoi;
        private bool isChecktruyenfull;
        /*observabel tuong tu list nhung dung no se hien thi du lieu luon len form khi get duoc du lieu*/
        private ObservableCollection<Book> listtruyenhot;
        private ObservableCollection<Book> listtruyenmoi;
        private ObservableCollection<Book> listtruyenfull;
     



        public bool IsCheckTruyenHot { get => isChecktruyenhot; set { isChecktruyenhot = value; lsbTopBooks.ItemsSource = ListTruyenHot; isChecktruyenmoi = false; isChecktruyenfull = false; OnPropertyChanged("IsCheckTruyenHot"); OnPropertyChanged("IsCheckTruyenMoi"); OnPropertyChanged("IsCheckTruyenFull"); } }
        public bool IsCheckTruyenMoi { get => isChecktruyenmoi; set { isChecktruyenmoi = value; lsbTopBooks.ItemsSource = ListTruyenMoi; isChecktruyenhot = false; isChecktruyenfull = false; OnPropertyChanged("IsCheckTruyenHot"); OnPropertyChanged("IsCheckTruyenMoi"); OnPropertyChanged("IsCheckTruyenFull"); } }
        public bool IsCheckTruyenFull { get => isChecktruyenfull; set { isChecktruyenfull = value; lsbTopBooks.ItemsSource = ListTruyenFull; isChecktruyenhot = false; isChecktruyenmoi = false; OnPropertyChanged("IsCheckTruyenHot"); OnPropertyChanged("IsCheckTruyenMoi"); OnPropertyChanged("IsCheckTruyenFull"); } }

        public ObservableCollection<Book> ListTruyenHot { get => listtruyenhot; set => listtruyenhot = value; }
        public ObservableCollection<Book> ListTruyenMoi { get => listtruyenmoi; set => listtruyenmoi = value; }
        public ObservableCollection<Book> ListTruyenFull { get => listtruyenfull; set => listtruyenfull = value; }




        public MainWindow()
        {
            InitializeComponent();
            ucBookInfo.BackToMain += UcBookInfo_BackToMain;

            this.DataContext = this;
            
            
            ListTruyenMoi = new ObservableCollection<Book>();
            ListTruyenHot = new ObservableCollection<Book>();
            ListTruyenFull = new ObservableCollection<Book>();
            
     
            CrawlBXH();
            IsCheckTruyenHot = true;
        }
        void CrawlBXH()
        {
            HttpRequest http = new HttpRequest();
            string htmlBXH = http.Get(@"http://truyenfull.vn/").ToString();

            string bxhPattern = @"(tainer"" id=)(.*?)</div></div></div><div(class=""con || id=""footer"")";
            var listBXH = Regex.Matches(htmlBXH, bxhPattern, RegexOptions.Singleline);

            string listtruyenhot = listBXH[0].ToString();
            Addhotbook(ListTruyenHot,listtruyenhot);

            string listtruyenmoi = listBXH[1].ToString();
            Addbookupdate(ListTruyenMoi, listtruyenmoi);

            string listtruyenfull = listBXH[2].ToString();
            Addbookfull(ListTruyenFull, listtruyenfull);



        }
        /*Vi cac form co ma html khong giong nhau nen tao 3 ham*/
        /*Ham crawl data cho form truyen hot*/
        void Addhotbook(ObservableCollection<Book> listbook, string html)
        {
            var listBookHTML = Regex.Matches(html, @"<div class=""item top(.*?)</a>", RegexOptions.Singleline);
            for (int i = 0; i < listBookHTML.Count; i++)
            {
                var booknameandauthor = Regex.Matches(listBookHTML[i].ToString(), @"<a href=""(.*?)""|alt=""(.*?)""", RegexOptions.Singleline);

                string booknameString = booknameandauthor[1].ToString();
                string bookname = booknameString.Substring(booknameString.IndexOf("alt=\""), booknameString.Length - 1).Replace("alt=\"", "");
                string urlstring = booknameandauthor[0].ToString();
                string urlname = urlstring.Substring(urlstring.IndexOf("<a href=\""), urlstring.Length - 1).Replace("<a href=\"", "");

                /*Crawl link trang ke tiep*/
                HttpRequest http = new HttpRequest();
                string htmlBook = http.Get(urlname).ToString();
                var gioithieu = Regex.Matches(htmlBook, @"<div class=""desc-text(.*?)</div>", RegexOptions.Singleline);
                string temp = "Chưa có thông tin truyện!";
                if (gioithieu.Count > 0)
                {
                    temp = gioithieu[0].ToString();
                    string tempToCut = temp.Substring(0, temp.IndexOf('>') + 1);
                    temp = temp.Replace(tempToCut, "").Replace("<br>", "").Replace("</br>", "").Replace("</b>", "").Replace("</p>", "").Replace("</div>", "").Replace("<b>", "").Replace("<p>", "").Replace("<span>", "").Replace("</span>", "");
                }
                ListTruyenHot.Add(new Book() { BookName = bookname, BookURL = urlname, STT = i + 1 , ThongTinTruyen = temp });


            }
        }

        /*Ham crawl data cho form truyen moi*/
        void Addbookupdate(ObservableCollection<Book> listbook, string html)
        {   /*regex cac doan ma chu noi dung gong link truyen va ten truyen*/
            var listBookHTML = Regex.Matches(html, @"<div class=""col-xs-9 col-sm-6 col-md-5 col-title"">(.*?)</div>", RegexOptions.Singleline);
            /*Su dung vong lap vi khi regex buoc tren se cho ra ket qu kieu nhu mot danh sach giong mang, duyet vong for cho tung cum regex duoc*/           
            for (int i = 0; i < listBookHTML.Count; i++)
            {
                /*Tiep tuc boc tiep den noi dung link va ten truyen mot cach chinh xac hon*/
                var booknameandauthor = Regex.Matches(listBookHTML[i].ToString(), @"<a href=""(.*?)""|title=""(.*?)""", RegexOptions.Singleline);
                /*sau khi regex duoc hai gia tri laf link dang href= vaf tile= ten truyen, buoc duoi se lam cho data sach hon, chir gom link vaf te truyen ma khong chua bat cu ma code html nao*/
                string booknameString = booknameandauthor[1].ToString();
                string bookname = booknameString.Substring(booknameString.IndexOf("title=\""), booknameString.Length - 1).Replace("title=\"", "");
                
                string urlstring = booknameandauthor[0].ToString();
                string urlname = urlstring.Substring(urlstring.IndexOf("<a href=\""), urlstring.Length - 1).Replace("<a href=\"", "");

                /*Crawl link trang ke tiep*/
                HttpRequest http = new HttpRequest();
                string htmlBook = http.Get(urlname).ToString();
                var truyen = Regex.Matches(htmlBook, @"<div class=""visible-md visible-lg (.*?)</div><div class=""text-center", RegexOptions.Singleline);
                string temp = "Chưa có thông tin truyện!";
                if (truyen.Count > 0)
                {
                    temp = truyen[0].ToString();
                    string tempToCut = temp.Substring(0, temp.IndexOf('>') + 1);
                    temp = temp.Replace(tempToCut, "").Replace("<br>", "").Replace("</br>", "").Replace("</div>", "").Replace("<b>", "").Replace("</b>", "").Replace("<p>", "").Replace("</p>", "");
                }


                ListTruyenMoi.Add(new Book() { BookName = bookname, BookURL = urlname, STT = i + 1 , ThongTinTruyen = temp });

            }
        }
        /*Ham regex cho form truyen full*/
        /* Ve co ban cac ham nay gan giong nhau*/
        void Addbookfull (ObservableCollection<Book> listbook, string html)
        {   /*regex cac doan ma chu noi dung gong link truyen va ten truyen*/
            var listBookHTML = Regex.Matches(html, @"<div class=""col-xs-4 col-sm-3 col-md-2"">(.*?)</div>", RegexOptions.Singleline);
            /*Su dung vong lap vi khi regex buoc tren se cho ra ket qu kieu nhu mot danh sach giong mang, duyet vong for cho tung cum regex duoc*/
            for (int i = 0; i < listBookHTML.Count; i++)
            {
                /*Tiep tuc boc tiep den noi dung link va ten truyen mot cach chinh xac hon*/
                var booknameandauthor = Regex.Matches(listBookHTML[i].ToString(), @"<a href=""(.*?)""|title=""(.*?)""", RegexOptions.Singleline);
                /*sau khi regex duoc hai gia tri laf link dang href= vaf tile= ten truyen, buoc duoi se lam cho data sach hon, chir gom link vaf te truyen ma khong chua bat cu ma code html nao*/
                string booknameString = booknameandauthor[1].ToString();
                string bookname = booknameString.Substring(booknameString.IndexOf("title=\""), booknameString.Length - 1).Replace("title=\"", "");

                string urlstring = booknameandauthor[0].ToString();
                string urlname = urlstring.Substring(urlstring.IndexOf("<a href=\""), urlstring.Length - 1).Replace("<a href=\"", "");

                /*Crawl link trang ke tiep*/
                HttpRequest http = new HttpRequest();
   
                    String htmlBook = http.Get(urlname).ToString();
                    var gioithieu = Regex.Matches(htmlBook, @"<div class=""desc-text(.*?)</div>", RegexOptions.Singleline);
                    string temp = "Chưa có thông tin truyện!";
                    if (gioithieu.Count > 0)
                    {
                        temp = gioithieu[0].ToString();
                        string tempToCut = temp.Substring(0, temp.IndexOf('>') + 1);
                        temp = temp.Replace(tempToCut, "").Replace("<br>", "").Replace("</br>", "").Replace("<b>", "").Replace("</b>", "").Replace("</div>", "");
                    }
                  
                //listchuong(ListChuong, urlname);

                ListTruyenFull.Add(new Book() { BookName = bookname, BookURL = urlname, STT = i + 1, ThongTinTruyen = temp });
            }
        }

   
        private void UcBookInfo_BackToMain(object sender, EventArgs e)
        {
            gridTop10.Visibility = Visibility.Visible;
            ucBookInfo.Visibility = Visibility.Hidden;
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Book book = (sender as Button).DataContext as Book;

            gridTop10.Visibility = Visibility.Hidden;
            ucBookInfo.Visibility = Visibility.Visible;
            ucBookInfo.Bookinfo = book;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("@R&d Product by Nguyễn Đình Tiềm fb:/nguyendinhtiem1999");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("SkyBooks APP 2019 Version 31.7.2020");
        }
    }
}
