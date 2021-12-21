using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace App1.TableCreators
{
    public class TableCreator
    {

       public TableCreator() 
        {

        }

        public void CreateTables(string pathname)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String createStudentsTableCmd = "CREATE TABLE if not exists STUDENTS " +
                "(STUDENT_ID VARCHAR(8) PRIMARY KEY, F_NAME VARCHAR(200), " +
                "L_NAME VARCHAR(200));";
                SqliteCommand createStudentTable = new SqliteCommand(createStudentsTableCmd, db);
                createStudentTable.ExecuteReader();

                String createCoursesTableCmd = "CREATE TABLE if not exists COURSES " +
                "(COURSE_ID VARCHAR(100) PRIMARY KEY);";
                SqliteCommand createCourseTable = new SqliteCommand(createCoursesTableCmd, db);
                createCourseTable.ExecuteReader();

                String createStudentsCoursesTableCmd = "CREATE TABLE if not exists STUDENTS_COURSES " +
                "(STUDENT_ID VARCHAR(8), COURSE_ID VARCHAR(100), FOREIGN KEY(STUDENT_ID) REFERENCES STUDENTS(STUDENT_ID), " +
                "FOREIGN KEY(COURSE_ID) REFERENCES COURSES(COURSE_ID), PRIMARY KEY(STUDENT_ID, COURSE_ID)); ";
                SqliteCommand createStudentsCoursesTable = new SqliteCommand(createStudentsCoursesTableCmd, db);
                createStudentsCoursesTable.ExecuteReader();
            }
        }

        public void EraseDatabase(string pathname)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String DropStudentsCourses = "DELETE FROM STUDENTS_COURSES;";
                SqliteCommand DropStudentsCoursesCommand = new SqliteCommand(DropStudentsCourses, db);
                DropStudentsCoursesCommand.ExecuteReader();

                String DropCourses = "DELETE FROM COURSES;";
                SqliteCommand DropCoursesCommand = new SqliteCommand(DropCourses, db);
                DropCoursesCommand.ExecuteReader();

                String DropStudents = "DELETE FROM STUDENTS;";
                SqliteCommand DropStudentsCommand = new SqliteCommand(DropStudents, db);
                DropStudentsCommand.ExecuteReader();
            }
        }

    }
}
