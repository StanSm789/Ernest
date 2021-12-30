using App1.Dao.DataRetrieval;
using App1.Dao.Impl;
using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App1.Parser
{
    public class ExcelParser
    {
        private StudentDao StudentDao;
        private CourseDao CourseDao;
        private NetworkDao NetworkDao;
        private DataRetrievalClass DataRetrieval;

        public ExcelParser(StudentDao studentDao, CourseDao courseDao, 
            NetworkDao networkDao, DataRetrievalClass dataRetrieval)
        {
            StudentDao = studentDao;
            CourseDao = courseDao;
            NetworkDao = networkDao;
            DataRetrieval = dataRetrieval;
        }

        /*
         * The function reads data from Excel file and saves every cell into a temporary object (Obj class)
         * */
        public List<Obj> ReadFromExcel(string inputString)
        {
            List<string[]> lines = SplitString(inputString);
            List<Obj> _data = new List<Obj>();

            foreach (string[] line in lines)
            {
                Obj obj = new Obj();
                obj.StudentId = line[0];
                obj.FirstName = line[1];
                obj.LastName = line[2];
                obj.ChildCourse = line[3];
                _data.Add(obj);
            }

            _data.RemoveAt(0);

            return _data;
        }

        /*
         * The function writes data into the database
         * */
        public void WriteToDatabase(List<Obj> inputStudents)
        {
            if (StudentDao.CheckIfStudentATableIsEmpty() == 0) // checking if students table does not exist and insert new data
            {
                for (int i = 0; i < inputStudents.Count; i++)
                {
                    CourseDao.Save(new Course.Builder().WithId(inputStudents[i].ChildCourse).Build());

                    Student student = new Student.Builder().WithId(inputStudents[i].StudentId)
                            .WithFirstName(inputStudents[i].FirstName).WithLastName(inputStudents[i].LastName).Build();
                    StudentDao.Save(student);

                    CourseDao.SaveStudentCourse(inputStudents[i].StudentId, inputStudents[i].ChildCourse);
                }
            } else // if database already has data, update the database with new entries and remove students who dropped out
            {
                HashSet<string> distinctCoursesFromInputFile = GetDistinctCoursesFromInputFile(inputStudents); // getting distinct courses from the input file

                foreach (string courseId in distinctCoursesFromInputFile)
                {
                    CourseDao.DeleteById(courseId); // deleting courses by course id 
                    CourseDao.Save(new Course.Builder().WithId(courseId).Build()); // saving the course again
                }

                foreach (Obj student in inputStudents)
                {

                    if (StudentDao.CheckIfStudentExists(student.StudentId) == 0) // if student does not exist add them into the database
                    {
                        Student st = new Student.Builder().WithId(student.StudentId)
                            .WithFirstName(student.FirstName).WithLastName(student.LastName).Build();
                        StudentDao.Save(st);
                    }

                    CourseDao.SaveStudentCourse(student.StudentId, student.ChildCourse); // saving student to course
                }
            }
        }

        /*
         * The function splits a row from Excel file into a list of strings
         * */
        private List<string[]> SplitString(string inputString)
        {
            List<string[]> result = new List<string[]>();

            string[] lines = inputString.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                result.Add(line.Split(','));
            }

            return result;
        }

        /*
         * The function gets all existing students from the database
         * */
        private List<Obj> GetAllExistingStudents() 
        {
            List<Student> existingStudents = StudentDao.FindAll();
            List<Obj> result = new List<Obj>();

            foreach(Student student in existingStudents)
            {
               for (int i = 0; i < student.Courses.Count; i++)
                {
                    Obj obj = new Obj();
                    obj.StudentId = student.Id;
                    obj.FirstName = student.FirstName;
                    obj.LastName = student.LastName;
                    obj.ChildCourse = student.Courses[i].Id;
                    result.Add(obj);
                }
            }

            return result;
        }

        /*
         * The function gets all distinct courses (distinction by campus) from the input data
         * */
        private HashSet<string> GetDistinctCoursesFromInputFile(List<Obj> students)
        {
            HashSet<string> result = new HashSet<string>();

            foreach (Obj student in students)
            {
                result.Add(student.ChildCourse);
            }

            return result;
        }
    }

         /*
         * This class is used to store data from each row of Excel file
         * */
    public class Obj
    {
        public string StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ChildCourse { get; set; }
    }

}
