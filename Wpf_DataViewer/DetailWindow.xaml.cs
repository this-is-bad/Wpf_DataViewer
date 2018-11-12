using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wpf_DataViewer
{
    
    /// <summary>
    /// Interaction logic for DetailWindow.xaml
    /// </summary>
    public partial class DetailWindow : Window
    {
        Character _person = new Character();

        public DetailWindow(Character person)
        {
            InitializeComponent();
            _person = person;
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string packUri = "pack://application:,,,/AssemblyName;Images/" + _person.ImageFileName;
            //
            // format and display character information
            //
            lbl_FullName.Content = _person.FullName();
            lbl_FirstName.Content = "First Name: " + _person.FirstName;
            lbl_LastName.Content = "Last Name: " + _person.LastName;
            lbl_Id.Content = "ID: " + _person.Id.ToString();
            lbl_Age.Content = "Age: " + _person.Age.ToString();
            lbl_Gender.Content = "Gender:" + _person.Gender.ToString();
            txtbx_Description.Text = _person.Description;
            //Uri uri = new Uri(@"/Wpf_DataViewer;Images/" + _person.ImageFileName, UriKind.Relative);
            //img_Character.Source = new BitmapImage(uri);
            img_Character.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            this.Content = _person.FullName() + " Details";
        }
    }
}
