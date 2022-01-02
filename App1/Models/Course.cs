using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class Course
    {
        private Course() { }
        public string Id { get; private set; }

        //Nested builder
        public class Builder
        {
            private readonly Course course = new Course();

            public Builder WithId(string Id)
            {
                course.Id = Id;
                return this;
            }

            public Course Build() => course;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
