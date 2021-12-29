using App1.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace App1.Dao.DataRetrieval
{
    public class DataRetrievalClass
    {
        private string Pathname;

        public DataRetrievalClass(string pathname)
        {
            Pathname = pathname;
        }

        public List<Course> GetCoursesForStudent(string studentId)
        {
            List<Course> result = new List<Course>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "select COURSE_ID from STUDENTS_COURSES where STUDENT_ID=@id;";
                cmd.Parameters.AddWithValue("@id", studentId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Course course = new Course.Builder().WithId(reader.GetString(0)).Build();

                        result.Add(course);
                    }
                }

                db.Close();
            }

            return result;
        }

        public List<Student> GetStudentsForCourse(string courseId)
        {
            List<Student> result = new List<Student>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "select S.STUDENT_ID, S.F_NAME, S.L_NAME from STUDENTS AS S, STUDENTS_COURSES AS SC where S.STUDENT_ID=SC.STUDENT_ID and SC.COURSE_ID=@id;";
                cmd.Parameters.AddWithValue("@id", courseId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Student.Builder().WithId(reader.GetString(0))
                            .WithFirstName(reader.GetString(1)).WithLastName(reader.GetString(2)).Build());
                    }
                }

                db.Close();
            }

            return result;
        }

        /*
         * The function finds student and their courses by a keyword: sNumber, fName, lName
         * */
        public List<Student> FindIndividualStudent(string keyword)
        {
            List<Student> result = new List<Student>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "SELECT * FROM STUDENTS WHERE STUDENT_ID LIKE @keyword OR F_NAME LIKE @keyword OR L_NAME LIKE @keyword;";
                cmd.Parameters.AddWithValue("@keyword", keyword.Trim());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {


                        Student student = new Student.Builder().WithId(reader.GetString(0))
                            .WithFirstName(reader.GetString(1)).WithLastName(reader.GetString(2))
                            .WithCourses(GetCoursesForStudent(reader.GetString(0))).Build();

                        result.Add(student);
                    }
                }

                db.Close();
            }

            return result;
        }

        /*
         * The method is used to find subnet masks by student id
         * */
        public List<string> GetSubnetMasksByStudentId(string studentId)
        {
            List<string> result = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "select SUBNET_MASK from NETWORKS where STUDENT_ID=@id;";
                cmd.Parameters.AddWithValue("@id", studentId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string courseId = reader.GetString(0);

                        result.Add(courseId);
                    }
                }

                db.Close();
            }

            return result;
        }

    }

}
