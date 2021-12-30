using App1.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace App1.Dao.Impl
{
    public class StudentDao : CrudDao<string, Student>
    {
        private string Pathname;

        public StudentDao(string pathname)
        {
            Pathname = pathname;
        }

        public Student FindById(string id)
        {
            Student result = null;

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "SELECT * FROM STUDENTS WHERE STUDENT_ID=@id;";
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string studentId = reader.GetString(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);

                        result = new Student.Builder().WithId(studentId)
                            .WithFirstName(firstName).WithLastName(lastName).Build();
                    }
                }

                db.Close();

                List<Course> courses = FindCoursesForStudent(id);
                if (courses.Count != 0)
                {
                    result.SetCourses(courses);
                }
            }

            return result;
        }

        public List<Student> FindAll()
        {
            List<Student> result = new List<Student>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "SELECT * FROM STUDENTS;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string studentId = reader.GetString(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);

                        Student student = new Student.Builder().WithId(studentId)
                            .WithFirstName(firstName).WithLastName(lastName).Build();
                        student.SetCourses(FindCoursesForStudent(student.Id));
                        result.Add(student);
                    }
                }

                db.Close();
            }

            return result;
        }

        public void Save(Student student)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {

                    var insertCmd = db.CreateCommand();

                    insertCmd.CommandText = $"insert into STUDENTS Select '{student.Id}', '{student.FirstName}', '{student.LastName}' " +
                        $"Where not exists(select * from STUDENTS where STUDENT_ID='{student.Id}');";

                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();

                if (student.Courses != null)
                {
                    SaveStudentsCourses(student);

                }
            }
        }

        public void Update(Student student)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    var insertCmd = db.CreateCommand();

                    insertCmd.CommandText = $"UPDATE STUDENTS SET STUDENT_ID ='{student.Id}', F_NAME='{student.FirstName}', " +
                        $"L_NAME='{student.LastName}'" +
                        $"WHERE STUDENT_ID ='{student.Id}';";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();

                UpdateStudentsCourses(student);

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
                    var deleteFromStudentsCoursesTable = db.CreateCommand();
                    deleteFromStudentsCoursesTable.CommandText = $"DELETE FROM STUDENTS_COURSES WHERE STUDENT_ID ='{id}';";
                    deleteFromStudentsCoursesTable.ExecuteNonQuery();

                    var deleteFromNetworksTable = db.CreateCommand();
                    deleteFromNetworksTable.CommandText = $"DELETE FROM NETWORKS WHERE STUDENT_ID ='{id}';";
                    deleteFromNetworksTable.ExecuteNonQuery();

                    var deleteFromStudentsTable = db.CreateCommand();
                    deleteFromStudentsTable.CommandText = $"DELETE FROM STUDENTS WHERE STUDENT_ID ='{id}';";
                    deleteFromStudentsTable.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            }
        }

        /*
         * removing an existing student from course
         * */
        public void DeleteStudentFromCourse(string studentId, string courseId)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    var deleteFromStudentsCoursesTable = db.CreateCommand();
                    deleteFromStudentsCoursesTable.CommandText = $"DELETE FROM STUDENTS_COURSES WHERE STUDENT_ID ='{studentId}' AND COURSE_ID='{courseId}';";
                    deleteFromStudentsCoursesTable.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            }
        }

        private List<Course> FindCoursesForStudent(string studentId)
        {
            List<Course> result = new List<Course>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = $"SELECT COURSE_ID from STUDENTS_COURSES where STUDENT_ID='{studentId}';";
                //cmd.Parameters.AddWithValue("@id", studentId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string courseId = reader.GetString(0);

                        result.Add(new Course.Builder().WithId(courseId).Build());
                    }

                    db.Close();
                }

                return result;
            }
        }

        public void SaveStudentsCourses(Student student)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                var insertCmd = db.CreateCommand();

                for (int i = 0; i < student.Courses.Count; i++)
                {
                    insertCmd.CommandText = $"insert into STUDENTS_COURSES values ('{student.Id}', '{student.Courses[i].Id}');";
                    insertCmd.ExecuteNonQuery();
                }

                db.Close();
            }
        }

        /*
         * checking if STUDENTS table is empty. This method is used by ExcelParser class under the Parser folder
         * */
        public int CheckIfStudentATableIsEmpty() 
        {
            string result = null;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "select count(*) from STUDENTS;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetString(0);
                    }
                }

                db.Close();
            }

            return Int32.Parse(result);
        }

        private void UpdateStudentsCourses(Student student)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var insertCmd = db.CreateCommand();

                for (int i = 0; i < student.Courses.Count; i++)
                {
                    insertCmd.CommandText = $"UPDATE STUDENTS_COURSES SET COURSE_ID='{student.Courses[i].Id}' " +
                        $"where STUDENT_ID='{student.Id}';";
                    insertCmd.ExecuteNonQuery();
                }


                db.Close();
            }    
        }

        /*
         * This function checks if subnet mask exists in the database
         * */
        public int CheckIfStudentExists(string studentId)
        {
            string result = null;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = $"select count(*) from STUDENTS WHERE STUDENT_ID='{studentId}';";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetString(0);
                    }
                }

                db.Close();
            }

            return Int32.Parse(result);
        }

    }
}
