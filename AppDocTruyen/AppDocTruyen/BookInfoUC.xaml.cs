using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    /// Interaction logic for BookInfoUC.xaml
    /// </summary>
    public partial class BookInfoUC : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Book> listchuong;
        public ObservableCollection<Book> Listchuong { get => listchuong; set => listchuong = value; }
        //private Chuong chuonginfo;

        private Book bookinfo;

        public string link;
       

        public ObservableCollection<Book> ListChuong { get; private set; }

        public Book Bookinfo
        {
            get { return bookinfo; }
            set
            {
                bookinfo = value;
                OnPropertyChanged("Bookinfo");
                this.DataContext = Bookinfo;
                tbxthongtintruyen.Text = Bookinfo.ThongTinTruyen;
                link = Bookinfo.BookURL;
                ListChuong = new ObservableCollection<Book>();
                lbsndchuong.ItemsSource = ListChuong;
                Crawlchuong();


            }
         
        }
        //public String htmlchuongx;
        //public int n = 1;
        void Crawlchuong()
        {
            //HttpRequest http = new HttpRequest();
            //do
            //{
            //    string htmlchuong = http.Get(link +"trang-" +n+"/#list-chapter").ToString();
            //    string doanchualink = @" < div class=""row""><div class=""col-xs-12 col-sm-6 col-md-6"">(.*?)</ul></div></div>";
            //    var doanchuachuong = Regex.Matches(htmlchuong, doanchualink, RegexOptions.Singleline);

            //    string danhsachchuong = doanchuachuong[0].ToString();

            //    var listlink = Regex.Matches(danhsachchuong, @"<li>(.*?)</li>", RegexOptions.Singleline);
            //    for (int i = 0; i < listlink.Count; i++)
            //    {
            //        var link = Regex.Matches(listlink[i].ToString(), @"<a href=""(.*?)""|title=""(.*?)""", RegexOptions.Singleline);
            //        string linkchuongstring = link[0].ToString();
            //        string linkchuong = linkchuongstring.Substring(linkchuongstring.IndexOf("<a href=\""), linkchuongstring.Length - 1).Replace("<a href=\"", "");

            //        string stringten = link[1].ToString();
            //        string tenchuong = stringten.Substring(stringten.IndexOf("title=\""), stringten.Length - 1).Replace("title=\"", "");

            //        HttpRequest http2 = new HttpRequest();
            //        string htmlBook = http2.Get(linkchuong).ToString();
            //        var truyen = Regex.Matches(htmlBook, @"<div class=""visible-md visible-lg (.*?)</div><div class=""text-center", RegexOptions.Singleline);
            //        string temp = "Chưa có thông tin truyện!";
            //        if (truyen.Count > 0)
            //        {
            //            temp = truyen[0].ToString();
            //            string tempToCut = temp.Substring(0, temp.IndexOf('>') + 1);
            //            temp = temp.Replace(tempToCut, "").Replace("<br>", "").Replace("</p>", "").Replace("</div>", "").Replace("<b>", "").Replace("</b>", "");
            //        }
            //        ListChuong.Add(new Book() { DanhSachChuong = linkchuong, TenChuong = tenchuong, NoiDungChuong = temp, STTChuong = i + 1 });
            //    }
            //    htmlchuongx = http.Get(link + @"trang-" + (n+1).ToString()+ @"/#list-chapter").ToString();
            //    n++;
            //} while (htmlchuongx.Length > 0);
                HttpRequest http = new HttpRequest();
            //string htmlchuong = http.Get(link + "trang-" + n + "/#list-chapter").ToString();
                 string htmlchuong = http.Get(link).ToString();
                 string doanchualink = @"<div class=""row""><div class=""col-xs-12 col-sm-6 col-md-6"">(.*?)</ul></div></div>";
                var doanchuachuong = Regex.Matches(htmlchuong, doanchualink, RegexOptions.Singleline);

                string danhsachchuong = doanchuachuong[0].ToString();

                var listlink = Regex.Matches(danhsachchuong, @"<li>(.*?)</li>", RegexOptions.Singleline);
                for (int i = 0; i < listlink.Count; i++)
                {
                    var link = Regex.Matches(listlink[i].ToString(), @"<a href=""(.*?)""|title=""(.*?)""", RegexOptions.Singleline);
                    string linkchuongstring = link[0].ToString();
                    string linkchuong = linkchuongstring.Substring(linkchuongstring.IndexOf("<a href=\""), linkchuongstring.Length - 1).Replace("<a href=\"", "");

                    string stringten = link[1].ToString();
                    string tenchuong = stringten.Substring(stringten.IndexOf("title=\""), stringten.Length - 1).Replace("title=\"", "");

                    HttpRequest http2 = new HttpRequest();
                    string htmlBook = http2.Get(linkchuong).ToString();
                    var truyen = Regex.Matches(htmlBook, @"<div class=""visible-md visible-lg (.*?)</div><hr class=""chapter-end"" id=""chapter-end-bot"">", RegexOptions.Singleline);//</div><div class=""text-center
                    string temp = "Chưa có thông tin truyện!";
                    if (truyen.Count > 0)
                    {
                        temp = truyen[0].ToString();
                        string tempToCut = temp.Substring(0, temp.IndexOf('>') + 1);
                        temp = temp.Replace(tempToCut, "").Replace("<br>", "").Replace("</p>", "").Replace("</div>", "").Replace("<b>", "").Replace("</b>", "").Replace("<p>", "").Replace("<i>", "").Replace("</i>", "").Replace("</br>", "");
                    }
                    ListChuong.Add(new Book() { DanhSachChuong = linkchuong, TenChuong = tenchuong, NoiDungChuong = temp, STTChuong = i + 1 });
                }
               // htmlchuongx = http.Get(link + @"trang-" + (n + 1).ToString() + @"/#list-chapter").ToString();
               // n++;

        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }

        public BookInfoUC()
        {
            InitializeComponent();

            this.DataContext = Bookinfo;
            ucndchuong.BackToMain += Ucndchuong_BackToMain;
        }

        private void Ucndchuong_BackToMain(object sender, EventArgs e)
        {
            gridndchuong.Visibility = Visibility.Visible;
            ucndchuong.Visibility = Visibility.Hidden;
        }

        private event EventHandler backToMain;
        public event EventHandler BackToMain
        {
            add { backToMain += value; }
            remove { backToMain -= value; }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (backToMain != null)
                backToMain(this, new EventArgs());

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Book book = (sender as Button).DataContext as Book;

            gridndchuong.Visibility = Visibility.Hidden;
            ucndchuong.Visibility = Visibility.Visible;
           ucndchuong.Bookinfo = book;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("@R&d Product by Nguyễn Đình Tiềm fb:/nguyendinhtiem1999");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("SkyBooks APP 2019 Version 31.7.2020");
        }
    }
}
