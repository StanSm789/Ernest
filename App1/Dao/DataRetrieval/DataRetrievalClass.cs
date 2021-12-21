using Microsoft.Data.Sqlite;
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

        public List<string> GetCoursesForStudent(string studentId)
        {
            List<string> result = new List<string>();

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
                        string courseId = reader.GetString(0);

                        result.Add(courseId);
                    }
                }

                db.Close();
            }

            return result;
        }

        public List<string> GetStudentsForCourse(string courseId)
        {
            List<string> result = new List<string>();

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
                        string student = "ID: " + reader.GetString(0) + ", Full Name: " + reader.GetString(1) + 
                            " " + reader.GetString(2); 

                        result.Add(student);
                    }
                }

                db.Close();
            }

            return result;
        }

        public List<string> FindIndividualStudent(string keyword)
        {
            List<string> result = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "SELECT STUDENT_ID, F_NAME, L_NAME FROM STUDENTS WHERE (STUDENT_ID LIKE '@keyword' OR F_NAME LIKE '@keyword' OR L_NAME LIKE '@keyword');";
                cmd.Parameters.AddWithValue("@keyword", keyword.Trim());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string student = "ID: " + reader.GetString(0) + ", Full Name: " + reader.GetString(1) +
                            " " + reader.GetString(2);

                        result.Add(student);
                    }
                }

                db.Close();
            }

            return result;
        }

    }

}
