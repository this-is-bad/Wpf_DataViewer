using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf_DataViewer.DAL;

namespace Wpf_DataViewer
{
    /// <summary>
    /// Interaction logic for ListWindow.xaml
    /// </summary>
    public partial class ListWindow : Window
    {
        private List<Character> _characters;

        public ListWindow()
        {
            InitializeComponent();
        }

        private string _sortNameDir;

        public enum FilterGenderType { Male, Female }

        #region EVENTS

        private void ListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lbl_SearchMessage.Content = "";

            cmb_Filter.ItemsSource = Enum.GetValues(typeof(FilterGenderType));
            //
            // bind the data source to the datagridview
            //
            try
            {
                ReadMongoDBAndBindToDataGrid();
            }
            catch (FileNotFoundException)
            {

                MessageBox.Show("Unable to locate data file.\nExiting the application.");
                this.Close();
            }
        }

        private void Btn_Help_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.ShowDialog();
        }

        private void Btn_Detail_Click(object sender, RoutedEventArgs e)
        {
            ////
            //// send the selected record to the DetailForm and launch the DetailForm
            ////
            if (dataGridView_Characters.GetType().ToString() != "")
            {
                Character character = new Character();
                character = (Character)dataGridView_Characters.SelectedItem;

                DetailWindow detailWindow = new DetailWindow(character);
                detailWindow.ShowDialog();
            }
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //
                // make sure the grid is replicated to the data source
                //
                IDataService dataService = new MongoDBDataService();
                dataService.WriteAll(_characters);

            }
            catch (Exception)
            {

                throw;
            }
            this.Close();
        }

        private void Btn_Sort_Click(object sender, RoutedEventArgs e)
        {
            SortSearchFilter(true);
        }

        private void Txt_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            SortSearchFilter(false);
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //     display the number of matching records on the form

            if (dataGridView_Characters.Items.Count == 0) 
            {
                lbl_SearchMessage.Content = "No results found";
            }
            else
            {
                int i = dataGridView_Characters.Items.Count;
                lbl_SearchMessage.Content = String.Format("{0} record{1} found", (i.ToString()), (i == 1 ? "" : "s"));
            }
        }

        private void Cmb_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortSearchFilter(false);
        }

        private void Btn_ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txt_Search.Text = "";
        }

        private void Btn_ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            cmb_Filter.SelectedItem = null;
            SortSearchFilter(false);
        }

        #endregion EVENTS

        #region METHODS

        /// <summary>
        /// Connect to the database, retrieve a list of Characters, and bind the list to the data grid
        /// </summary>
        private void ReadMongoDBAndBindToDataGrid()
        {
            //
            // read data file
            //
            IDataService dataService = new MongoDBDataService();

            //
            // insert data into the table, only needed when the table is empty
            //
            //dataService.WriteAll(GenerateListOfCharacters());

            _characters = dataService.ReadAll().ToList<Character>();

            //
            // bind list to DataGridView control
            //
            var bindingList = new BindingList<Character>(_characters);
            dataGridView_Characters.ItemsSource = bindingList;
            HideGridColumns();
        }

        /// <summary>
        /// Hide columns in the grid
        /// </summary>
        private void HideGridColumns()
        {
            //
            // configure DataGridView control
            //
            this.dataGridView_Characters.Columns[0].Visibility = Visibility.Hidden;
            this.dataGridView_Characters.Columns[5].Visibility = Visibility.Hidden;
            this.dataGridView_Characters.Columns[6].Visibility = Visibility.Hidden;
        }
        
        /// <summary>
        /// Sort, search, and filter grid
        /// </summary>
        /// <param name="toggleSort"></param>
        private void SortSearchFilter(bool toggleSort)
        {
            if (_characters.Any())
            {
                List<Character> resultList = SortList(FilterList(SearchList(_characters)), toggleSort);
                dataGridView_Characters.ItemsSource = resultList;
                HideGridColumns();
            }
        }

        /// <summary>
        /// Filters a list of Characters and returns a list of characters that match the filter criteria
        /// </summary>
        /// <param name="characterList"></param>
        /// <returns>List of Character</returns>
        private List<Character> FilterList(List<Character> characterList)
        {
            string searchTerm = txt_Search.Text;
            List<Character> filteredList;
            filteredList = characterList.Where(c => (cmb_Filter.SelectedItem == null || cmb_Filter.SelectedItem.ToString() == "" || c.Gender.ToString() == cmb_Filter.SelectedItem.ToString())).ToList();

            return filteredList;
        }

        /// <summary>
        /// Searches a list of Characters and returns a list of characters that match the search criteria
        /// </summary>
        /// <param name="characterList"></param>
        /// <returns>List of Character</returns>
        private List<Character> SearchList(List<Character> characterList)
        {
            string searchTerm = txt_Search.Text.ToString();

            List<Character> searchedList = characterList.Where(c => c.LastName.ToUpper().Contains(searchTerm.ToUpper())
                                                || c.FirstName.ToUpper().Contains(searchTerm.ToUpper())).ToList();

            return searchedList;
        }

        /// <summary>
        /// Sorts a list of Characters, alternately in ascending and descending order
        /// </summary>
        /// <param name="characterList"></param>
        /// <returns>List of Character</returns>
        private List<Character> SortList(List<Character> characterList, bool toggleSort)
        {
            List<Character> sortedList;

            _sortNameDir = (_sortNameDir == null || _sortNameDir == "" ? "A": _sortNameDir);

            //
            // sort ascending when _sortNameDir = "A", and descending otherwise
            //
            if (_sortNameDir == "A")
            {
                sortedList = characterList.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ThenBy(c => c.Gender).ToList();
            }
            else
            {
                sortedList = characterList.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName).ThenByDescending(c => c.Gender).ToList();
            }

            if (toggleSort)
            {
                if (_sortNameDir == "A")
                {
                    _sortNameDir = "D";
                    btn_Sort.Content = "Sort Desc";
                }
                else
                {
                    _sortNameDir = "A";
                    btn_Sort.Content = "Sort";
                }
            }

            return sortedList;
        }

        /// <summary>
        /// Initializes a list of Character objects
        /// </summary>
        /// <returns>List of Character</returns>
        private static List<Character> GenerateListOfCharacters()
        {
            List<Character> characters = new List<Character>()
            {
                new Character()
                {
                    Id = 1,
                    LastName = "Flintstone",
                    FirstName = "Fred",
                    Age = 28,
                    Gender = Character.GenderType.Male,
                    ImageFileName = "fred_flintstone.jpg",
                    Description = "Fred is the main character of the series. He's an accident-prone bronto-crane operator at the Slate Rock and Gravel Company and the head of the Flintstone household. He is quick to anger (usually over trivial matters), but is nonetheless a very loving husband and father. He's also good at bowling and is a member of the fictional \"Loyal Order of Water Buffaloes\" (Lodge No. 26), a men-only club paralleling real-life fraternities such as the Loyal Order of Moose."
                },
                new Character()
                {
                    Id = 2,
                    LastName = "Flintstone",
                    FirstName = "Wilma",
                    Age = 27,
                    Gender = Character.GenderType.Female,
                    ImageFileName = "wilma_flintstone.jpg",
                    Description = "Wilma is Fred's wife. She is more intelligent and level-headed than her husband, though she often has a habit of spending money recklessly (with Betty and her catchphrase being \"Da-da-da duh da-da CHARGE IT!!\"). She's often a foil to Fred's poor behavior."
                },
                new Character()
                {
                    Id = 3,
                    LastName = "Flintstone",
                    FirstName = "Pebbles",
                    Age = 1,
                    Gender = Character.GenderType.Female,
                    ImageFileName = "pebbles_flintstone.jpg",
                    Description = "Pebbles is the Flintstones' infant daughter, who is born near the end of the third season."
                },
                new Character()
                {
                    Id = 4,
                    LastName = "Rubble",
                    FirstName = "Barney",
                    Age = 28,
                    Gender = Character.GenderType.Male,
                    ImageFileName = "barney_rubble.jpg",
                    Description = "Barney is the secondary main character and Fred's best friend and next-door neighbor. His occupation is, for the most part of the series, unknown, though later series depict him working in the same quarry as Fred. He shares many of Fred's interests such as bowling and golf, and is also a member of the \"Loyal Order of Water Buffaloes\". Though Fred and Barney frequently get into feuds with one another (usually due to Fred's short temper), their deep fraternal bond remains evident."
                },
                new Character()
                {
                    Id = 5,
                    LastName = "Rubble",
                    FirstName = "Betty",
                    Age = 26,
                    Gender = Character.GenderType.Female,
                    ImageFileName = "betty_rubble.gif",
                    Description = "Betty is Barney's wife and Wilma's best friend. Like Wilma, she, too, has a habit of carelessly spending money."
                },
                new Character()
                {
                    Id = 6,
                    LastName = "Rubble",
                    FirstName = "Bamm-Bamm",
                    Age = 2,
                    Gender = Character.GenderType.Male,
                    ImageFileName = "bamm_bamm_rubble.gif",
                    Description = "Bamm-Bamm is the Rubbles' abnormally strong adopted son, whom they adopt during the fourth season."
                },
                new Character()
                {
                    Id = 7,
                    LastName = "",
                    FirstName = "Dino",
                    Age = 7,
                    Gender = Character.GenderType.Female,
                    ImageFileName = "dino.jpg",
                    Description = "Dino, a prosauropod dinosaur, is the Flintstones' pet that barks and generally acts like a dog. A running gag in the series involves Dino knocking down Fred out of excitement and licking him repeatedly. Though this irritates Fred a lot, he generally likes Dino very much."
                }
            };

            return characters;
        }

        #endregion METHODS
    }
}
