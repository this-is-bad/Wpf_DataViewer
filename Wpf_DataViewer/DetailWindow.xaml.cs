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
        Character _character = new Character();

        //BitmapImage _bi3 = new BitmapImage();

        public DetailWindow(Character character)
        {
            InitializeComponent();
            _character = character;

            //_bi3.BeginInit();
            //_bi3.UriSource = new Uri(@"Images/" + _person.ImageFileName, UriKind.Relative);
            //_bi3.EndInit();
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("pack://application:,,,/AssemblyName;component/Images/" + _character.ImageFileName);

            //
            // format and display character information
            //
            lbl_FullName.Content = _character.FullName();
            lbl_FirstName.Content = "First Name: " + _character.FirstName;
            lbl_LastName.Content = "Last Name: " + _character.LastName;
            lbl_Id.Content = "ID: " + _character.Id.ToString();
            lbl_Age.Content = "Age: " + _character.Age.ToString();
            lbl_Gender.Content = "Gender: " + _character.Gender.ToString();
            txtbx_Description.Text = _character.Description;

            img_Character.Source = new BitmapImage(uri);

            this.Content = _character.FullName() + " Details";
        }
    }
}
