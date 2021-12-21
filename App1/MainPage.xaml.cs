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
        public DataRetrievalClass dataRetrievalClass = new DataRetrievalClass(DatabaseName);
        public TableCreator tableCreator = new TableCreator();

        public MainPage()
        {
            this.InitializeComponent();
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
                //textBlock.Text = await FileIO.ReadTextAsync(file);
                //this.textBlock.Text = file.Path;
                string excelFile = await FileIO.ReadTextAsync(file);
                ExcelParser excelParser = new ExcelParser(studentDao, courseDao);
                excelParser.WriteToDatabase(excelParser.ReadFromExcel(excelFile));

          
                textBlock.Text = "";

                List<Student> students = studentDao.FindAll();




        //    foreach (Student student in students)
        //    {
        //        textBlock.Text = textBlock.Text + "\n" + student.ToString();
        //    }
        //
        //    }
        //    else
        //    {
        //        textBlock.Text = "Operation cancelled.";
        //    }
        //
        }

        private void EraseDatabase_Tapped(object sender, TappedRoutedEventArgs e)
        {
            tableCreator.EraseDatabase(DatabaseName);
            textBlock.Text = "All data has been successfully removed from the database.";
        }

        private void Find_Courses(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "";
            string studentId = Get_Courses_ForStudent_Box.Text.ToString();
            List<string> courses = dataRetrievalClass.GetCoursesForStudent(studentId);

            foreach(string course in courses)
            {
                textBlock.Text = textBlock.Text + "\n" + course;
            }
        }

        private void Find_Students(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "";
            string courseId = Get_Students_For_Course_Box.Text.ToString();
            List<string> students = dataRetrievalClass.GetStudentsForCourse(courseId);

            foreach (string student in students)
            {
                textBlock.Text = textBlock.Text + "\n" + student;
            }
        }

        private void Find_Individual_Student(object sender, RoutedEventArgs e)
        {
            /*textBlock.Text = "";
            string keyword = Find_Student_By_Keyword.Text.ToString();
            List<string> students = dataRetrievalClass.FindIndividualStudent(keyword);
            foreach (string student in students)
            {
                textBlock.Text = textBlock.Text + "\n" + student;
            }*/
        }

    }
}
