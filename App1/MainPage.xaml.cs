using App1.Dao.DataRetrieval;
using App1.Dao.Impl;
using App1.Models;
using App1.Parser;
using App1.TableCreators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static string DatabaseName = "students.db";
        public StudentDao studentDao = new StudentDao(DatabaseName);
        public CourseDao courseDao = new CourseDao(DatabaseName);
        public NetworkDao networkDao = new NetworkDao(DatabaseName);
        public DataRetrievalClass dataRetrievalClass = new DataRetrievalClass(DatabaseName);
        public TableCreator tableCreator = new TableCreator();
        public List<string> CoursesList = new List<string>(); // List for storing courses that appear in the database
        public string combo; // selection for combo box; "used in add course to studend"

        public MainPage()
        {
            this.InitializeComponent();
            dataGrid.ItemsSource = studentDao.FindAll(); // fitch student table everytime when open the App

            List<Course> courses = courseDao.FindAll();
            foreach (Course course in courses)
            {
                CoursesList.Add(course.Id); // combo box selection
            }
        }

        private async void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

            picker.FileTypeFilter.Add(".csv");
            picker.FileTypeFilter.Add(".xls");
            picker.FileTypeFilter.Add(".xlsx");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                string excelFile = await FileIO.ReadTextAsync(file);
                ExcelParser excelParser = new ExcelParser(studentDao, courseDao, networkDao);
                excelParser.WriteToDatabase(excelParser.ReadFromExcel(excelFile));

                List<Student> students = studentDao.FindAll();
                dataGrid.ItemsSource = students;
            }

            List<Course> courses = courseDao.FindAll();
            foreach (Course course in courses)
            {
                CoursesList.Add(course.Id); // combo box selection
            }
        }

        private async void EraseDatabase_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "Delete all data from the database permanently?",
                Content = "If you erase databse, you won't be able to recover it. Do you want to erase it?",
                PrimaryButtonText = "Erase",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteFileDialog.ShowAsync();

            // Delete the file if the user clicked the primary button.
            /// Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                tableCreator.EraseDatabase(DatabaseName);
                CoursesList.Clear();
                dataGrid.ItemsSource = new List<string> { "All data from the database has been deleted." };
            }
            else
            {
                // The user clicked the CLoseButton, pressed ESC, Gamepad B, or the system back button.
                // Do nothing.
            }
        }

        private void Find_Courses(object sender, RoutedEventArgs e)
        {
                string studentId = Get_Courses_ForStudent_Box.Text.ToString();
                List<string> courses = dataRetrievalClass.GetCoursesForStudent(studentId);

                dataGrid.ItemsSource = courses;
        }

        private void Find_Students(object sender, RoutedEventArgs e)
        {
            string courseId = combo;
            List<string> students = dataRetrievalClass.GetStudentsForCourse(courseId);

                dataGrid.ItemsSource = students;
        }

        private void Find_Individual_Student(object sender, RoutedEventArgs e)
        {
            string keyword = Find_Student_By_Keyword_Box.Text.ToString();
            List<string> students = dataRetrievalClass.FindIndividualStudent(keyword);
            dataGrid.ItemsSource = students;
        }

        private void Find_Networks_By_Student_Id(object sender, RoutedEventArgs e)
        {
            string studentId = Find_Networks_By_Student_Id_Box.Text.ToString();
            List<string> subnetMasks = dataRetrievalClass.GetSubnetMasksByStudentId(studentId);

            dataGrid.ItemsSource = subnetMasks;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddStudentForm));
        }

        private void DeleteExistingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Delete));
        }

        /*
         * combo box selection
         * */
        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            combo = comboBox.SelectedValue.ToString();
        }

        /*
         * README flyout
         * */
        private async void Readme_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = "README",
                Content = "Some text with instructions" +
                "Some text with instructions" +
                "Some text with instructions" +
                "Some text with instructions" +
                "Some text with instructions" +
                "Some text with instructions" +
                "Some text with instructions" +
                "Some text with instructions" +
                "Some text with instructions" +
                "Some text with instructions",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noWifiDialog.ShowAsync();
        }

    }
}
