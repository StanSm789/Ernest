using App1.Dao.DataRetrieval;
using App1.Dao.Impl;
using App1.Models;
using App1.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Delete : Page
    {

        public static string DatabaseName = "students.db";
        public StudentDao studentDao = new StudentDao(DatabaseName);
        public CourseDao courseDao = new CourseDao(DatabaseName);
        public NetworkDao networkDao = new NetworkDao(DatabaseName);
        public DataRetrievalClass dataRetrievalClass = new DataRetrievalClass(DatabaseName);
        public List<string> CoursesList = new List<string>();
        public string combo; // selection for combo box, used to display all distinct courses from the database

        public Delete()
        {
            this.InitializeComponent();
            dataGrid.ItemsSource = dataRetrievalClass.GetStudentsWithoutCourses(); // data grid for students without courses
            dataGridOne.ItemsSource = GetStudentsOnViewWithCourses(studentDao.FindAll()); // get all students with at least one enrolment  
            dataGridTwo.ItemsSource = GetStudentsOnViewWithIpAddresses(studentDao.FindAll()); // get all students with at least one IP address

            List<Course> courses = courseDao.FindAll();
            foreach (Course course in courses)
            {
                CoursesList.Add(course.Id); // combo box selection
            }
        }

        /*
         * going back to the main page
         * */
        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        /*
         * deleting an existing student from the db
         * */
        void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            string studentNumber = this.sNumber.Text.Trim();

            if (studentDao.FindById(studentNumber) == null)
            {
                this.ErrorMessageForDeletingStudent.Text = "Student with this sNumber does not exist";
            }
            else
            {
                studentDao.DeleteById(studentNumber);
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        /*
         * deleting an existing course from the db
         * */
        void DeleteCourseButton_Click(object sender, RoutedEventArgs e)
        {
            string courseNumber = combo;
            courseDao.DeleteById(courseNumber);
            this.Frame.Navigate(typeof(MainPage));
        }

        /*
         * removing a student from course
         * */
        void RemoveStudentFromCourse_Click(object sender, RoutedEventArgs e)
        {
            string studentNumber = this.student_Number.Text.Trim();
            string course = combo;

            if (studentDao.FindById(studentNumber) == null)
            {
                this.ErrorMessageForRemovingStudentFromCourse.Text = "Student with this sNumber does not exists";
            }
            else
            {
                studentDao.DeleteStudentFromCourse(studentNumber, course);
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        void DeleteNetwork_Click(object sender, RoutedEventArgs e)
        {
            string studentNumber = this.studentNumber.Text.Trim();
            string subnetMask = this.subnetMask.Text.Trim();

            if (studentDao.FindById(studentNumber) == null)
            {
                this.ErrorMessageNetwork.Text = "Student with this sNumber does not exist";
            }
            else if (networkDao.CheckIfSubnetMaskExists(subnetMask) == 0)
            {
                this.ErrorMessageNetwork.Text = "This subnet mask does not exist";
            }
            else
            {
                networkDao.DeleteBySubnetMaskAndStudentID(subnetMask, studentNumber);
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        /*
         * combo box selection for deleiting student from course
         * */
        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            combo = comboBox.SelectedValue.ToString();
        }

        /*
         * combo box selection for deleting existing course
         * */
        private void ComboxBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            combo = comboBox.SelectedValue.ToString();
        }

        /*
         * This function gets information about students who have at least one enrolment
         * */
        private List<StudentView> GetStudentsOnViewWithCourses(List<Student> students)
        {
            List<StudentView> result = new List<StudentView>();

            foreach (Student student in students)
            {
                if (student != null && student.Courses.Count != 0)
                {
                    List<string> subnetMasks = dataRetrievalClass.GetSubnetMasksByStudentId(student.Id);

                    StudentView studentView = new StudentView();
                    studentView.StudentId = student.Id;
                    studentView.FirstName = student.FirstName;
                    studentView.LastName = student.LastName;
                    studentView.Courses = string.Join(", ", student.Courses);
                    studentView.IPv4Addresses = string.Join(", ", subnetMasks);

                    result.Add(studentView);
                }
            }

            return result;
        }

        /*
         * This function gets information about students who have at least one IP address
         * */
        private List<StudentView> GetStudentsOnViewWithIpAddresses(List<Student> students)
        {
            List<StudentView> result = new List<StudentView>();

            foreach (Student student in students)
            {
                if (student != null)
                {
                    List<string> subnetMasks = dataRetrievalClass.GetSubnetMasksByStudentId(student.Id);

                    if (subnetMasks.Count != 0)
                    {
                        StudentView studentView = new StudentView();
                        studentView.StudentId = student.Id;
                        studentView.FirstName = student.FirstName;
                        studentView.LastName = student.LastName;
                        studentView.Courses = string.Join(", ", student.Courses);
                        studentView.IPv4Addresses = string.Join(", ", subnetMasks);

                        result.Add(studentView);
                    }
                    
                }
            }

            return result;
        }

    }
}
