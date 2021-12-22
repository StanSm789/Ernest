using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class Network
    {
        private Network() { }

        public int Id { get; private set; }
        public string StudentId { get; private set;}
        public string SubnetMask { get; private set; }



        //Nested builder
        public class Builder
        {
            private readonly Network network = new Network();

            public Builder WithId(int Id)
            {
                network.Id = Id;
                return this;
            }

            public Builder WithStudentId(string StudentId)
            {
                network.StudentId = StudentId;
                return this;
            }

            public Builder WithSubnetMask(string SubnetMask)
            {
                network.SubnetMask = SubnetMask;
                return this;
            }

            public Network Build() => network;
        }

        public override string ToString()
        {
            return "Network ID: " + Id + ", Student ID: " + StudentId + ", SubnetMask: " + SubnetMask;
        }
    }
}
