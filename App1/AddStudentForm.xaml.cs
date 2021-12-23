using App1.Dao.DataRetrieval;
using App1.Dao.Impl;
using App1.Models;
using App1.TableCreators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
    public sealed partial class AddStudentForm : Page
    {
        public static string DatabaseName = "students.db";
        public StudentDao studentDao = new StudentDao(DatabaseName);
        public CourseDao courseDao = new CourseDao(DatabaseName);
        public NetworkDao networkDao = new NetworkDao(DatabaseName);
        public DataRetrievalClass dataRetrievalClass = new DataRetrievalClass(DatabaseName);
        public List<string> CoursesList = new List<string>();
        public string combo; // selection for combo box; "used in add course to studend"

        public AddStudentForm()
        {
            this.InitializeComponent();

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
         * adding new student to the db
         * */
        void SaveStudentButton_Click(object sender, RoutedEventArgs e)
        {
            string newStudentNumber = this.sNumber.Text.Trim();
            string fName = this.FirstName.Text.Trim();
            string lName = this.LastName.Text.Trim();

            if (!IsValidStudentNumber(newStudentNumber))
            {
                this.ErrorMessage.Text = "Please insert valid sNumber";
            } 
            else if(!IsValidName(fName) || !IsValidName(lName)) 
            {
                this.ErrorMessage.Text = "Please insert correct Name";
            }

            else if (studentDao.FindById(newStudentNumber) != null)
            {
                this.ErrorMessage.Text = "Student with this sNumber already exists";
            }
            else
            {
                Student student =
                new Student.Builder().WithId(newStudentNumber).WithFirstName(fName).WithLastName(lName).Build();
                studentDao.Save(student);
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        /*
         * adding new course to the db
         * */
        void SaveNewCourse_Click(object sender, RoutedEventArgs e)
        {
            string courseId = this.courseDetails.Text.Trim();

            if (courseDao.FindById(courseId) != null)
            {
                this.ErrorMessageForAddingCourse.Text = "Course already exists";
            } 
            else
            {
                courseDao.Save(new Course.Builder().WithId(courseId).Build());
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        /*
         * saving a course to an existing student
         * */
        void SaveCourse_To_Student_Click(object sender, RoutedEventArgs e)
        {
            string studentNumber = this.student_Number.Text.Trim();
            string course = combo;

            if (studentDao.FindById(studentNumber) == null)
            {
                this.ErrorMessageForAddingCourse_To_Student.Text = "Student with this sNumber does not exists";
            } 
            else
            {
                courseDao.SaveStudentCourse(studentNumber, course);
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        void Save_Network_Click(object sender, RoutedEventArgs e)
        {
            string sNumber = this.studentNumber.Text.Trim();
            string network = this.network.Text.Trim();

            if (studentDao.FindById(sNumber) == null)
            {
                this.ErrorMessageNetwork.Text = "Student with this sNumber does not exists";
            }
            else
            {
                try
                {
                    networkDao.Save(new Network.Builder().WithStudentId(sNumber).WithSubnetMask(network).Build());
                    this.Frame.Navigate(typeof(MainPage));
                } catch
                {
                    this.ErrorMessageNetwork.Text = "This subnet mask already belongst to another student";

                }
            }
        }

        private bool IsValidName(string name)
        {
            string strRegex = @"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$";

            Regex re = new Regex(strRegex);

            if (re.IsMatch(name))
                return (true);
            else
                return (false);
        }

        private bool IsValidStudentNumber(string sNumber)
        {
            string strRegex = @"[s]\d{7}";

            Regex re = new Regex(strRegex);

            if (re.IsMatch(sNumber))
                return (true);
            else
                return (false);
        }

        /*
         * combo box selection
         * */
        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           ComboBox comboBox = sender as ComboBox;

           combo = comboBox.SelectedValue.ToString(); 
        }
    }
}

