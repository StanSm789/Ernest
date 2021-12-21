using App1.Dao.Impl;
using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Parser
{
    public class ExcelParser
    {
        private StudentDao StudentDao;
        private CourseDao CourseDao;

        public ExcelParser(StudentDao studentDao, CourseDao courseDao)
        {
            StudentDao = studentDao;
            CourseDao = courseDao;
        }

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

        public void WriteToDatabase(List<Obj> _data)
        {
            for (int i = 0; i < _data.Count; i++)
            {
                CourseDao.InsertDistinctCourse(_data[i].ChildCourse);

                Student student = new Student.Builder().WithId(_data[i].StudentId)
                        .WithFirstName(_data[i].FirstName).WithLastName(_data[i].LastName).Build();
                StudentDao.Save(student);

                CourseDao.SaveStudentCourse(_data[i].StudentId, _data[i].ChildCourse);
            }
        }

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
    }

    public class Obj
    {
        public string StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ChildCourse { get; set; }
    }
}
