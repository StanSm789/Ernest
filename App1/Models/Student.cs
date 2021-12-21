using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class Student
    {
        private Student() { }
        public string Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public List<Course> Courses { get; private set; }

        public void SetCourses(List<Course> courses)
        {
            Courses = courses;
        }

        //Nested builder
        public class Builder
        {
            private readonly Student student = new Student();

            public Builder WithId(string Id)
            {
                student.Id = Id;
                return this;
            }

            public Builder WithFirstName(string firstName)
            {
                student.FirstName = firstName;
                return this;
            }

            public Builder WithLastName(string lastName)
            {
                student.LastName = lastName;
                return this;
            }

            public Builder WithCourses(List<Course> courses)
            {
                student.Courses = courses;
                return this;
            }

            public Student Build() => student;
        }

        public override string ToString()
        {
            return "Student ID: " + Id + 
                " Student name: " + FirstName + " " + LastName;
        }

    }
}
