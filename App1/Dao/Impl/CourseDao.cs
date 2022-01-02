using App1.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace App1.Dao.Impl
{
    public class CourseDao : CrudDao<string, Course>
    {
        private string Pathname;

        public CourseDao(string pathname)
        {
            Pathname = pathname;
        }

        public Course FindById(string id)
        {
            Course result = null;

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "SELECT * FROM COURSES WHERE COURSE_ID=@id;";
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string courseId = reader.GetString(0);

                        result = new Course.Builder().WithId(courseId).Build();
                    }
                }

                db.Close();
            }

            return result;
        }

        public List<Course> FindAll()
        {
            List<Course> result = new List<Course>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "SELECT * FROM COURSES;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string courseId = reader.GetString(0);

                        result.Add(new Course.Builder().WithId(courseId).Build());
                    }
                }

                db.Close();            
            }

            return result;
        }

        public void Save(Course course)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    var insertCmd = db.CreateCommand();

                    insertCmd.CommandText = $"insert into COURSES Select '{course.Id}' " +
                        $"Where not exists(select * from COURSES where COURSE_ID='{course.Id}');";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            }
        }

        public void Update(Course course)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    var insertCmd = db.CreateCommand();
                    var up = db.CreateCommand();

                    insertCmd.CommandText = $"UPDATE COURSES SET COURSE_ID ='{course.Id}' WHERE COURSE_ID ='{course.Id}';";
                    insertCmd.ExecuteNonQuery();

                    up.CommandText = $"UPDATE STUDENTS_COURSES SET COURSE_ID ='{course.Id}' WHERE COURSE_ID ='{course.Id}';";
                    up.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            } 
        }

        public void DeleteById(string id)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    var insertCmd = db.CreateCommand();
                    var del = db.CreateCommand();

                    del.CommandText = $"DELETE FROM STUDENTS_COURSES WHERE COURSE_ID ='{id}';";
                    del.ExecuteNonQuery();

                    insertCmd.CommandText = $"DELETE FROM COURSES WHERE COURSE_ID ='{id}';";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            }

        }

        public void SaveStudentCourse(string studentId, string courseId)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    var insertCmd = db.CreateCommand();

                    //insertCmd.CommandText = $"insert into STUDENTS_COURSES values ('{studentId}', '{courseId}');";
                    insertCmd.CommandText = $"insert into STUDENTS_COURSES Select '{studentId}', '{courseId}' " +
                       $"Where not exists(select * from STUDENTS_COURSES where STUDENT_ID='{studentId}' AND COURSE_ID='{courseId}');";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            }       
        }

        /*
         * This method removes data from the STUDENTS and STUDENTS_COURSES tables. It is used by ExcelParser class under the Pasrer folder
         * */
        public void EraseDatabase()
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String DropStudentsCourses = "DELETE FROM STUDENTS_COURSES;";
                SqliteCommand DropStudentsCoursesCommand = new SqliteCommand(DropStudentsCourses, db);
                DropStudentsCoursesCommand.ExecuteReader();

                /*String DropCourses = "DELETE FROM COURSES;";
                SqliteCommand DropCoursesCommand = new SqliteCommand(DropCourses, db);
                DropCoursesCommand.ExecuteReader();*/

                String DropStudents = "DELETE FROM STUDENTS;";
                SqliteCommand DropStudentsCommand = new SqliteCommand(DropStudents, db);
                DropStudentsCommand.ExecuteReader();
            }
        }

    }
}
