using App1.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace App1.Dao.Impl
{
    public class NetworkDao : CrudDao<int, Network>
    {
        private string Pathname;

        public NetworkDao(string pathname)
        {
            Pathname = pathname;
        }

        public Network FindById(int id)
        {
            Network result = null;

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "SELECT * FROM NETWORKS WHERE ID=@id;";
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int networkId = Int32.Parse(reader.GetString(0));
                        string studentId = reader.GetString(1);
                        string subnetMask = reader.GetString(2);

                        result = new Network.Builder().WithId(networkId)
                            .WithStudentId(studentId).WithSubnetMask(subnetMask).Build();
                    }
                }

                db.Close();
            }

            return result;
        }

        public List<Network> FindAll()
        {
            List<Network> result = new List<Network>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "SELECT * FROM NETWORKS;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int networkId = Int32.Parse(reader.GetString(0));
                        string studentId = reader.GetString(1);
                        string subnetMask = reader.GetString(2);

                        Network network = new Network.Builder().WithId(networkId)
                            .WithStudentId(studentId).WithSubnetMask(subnetMask).Build();
                        result.Add(network);
                    }
                }

                db.Close();
            }

            return result;
        }

        public void Save(Network network)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {

                    var insertCmd = db.CreateCommand();

                    insertCmd.CommandText = $"insert into NETWORKS (STUDENT_ID, SUBNET_MASK) values ('{network.StudentId}', '{network.SubnetMask}');";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            }
        }

        public void Update(Network network)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    var insertCmd = db.CreateCommand();

                    insertCmd.CommandText = $"UPDATE NETWORKS SET STUDENT_ID ='{network.StudentId}', SUBNET_MASK='{network.SubnetMask}'" +
                        $"WHERE ID ='{network.Id}';";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            }
        }

        public void DeleteById(int id)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    var del = db.CreateCommand();
                    del.CommandText = $"DELETE FROM NETWORKS WHERE ID ='{id}';";
                    del.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            }
        }

        /*
         * This function is used to delete data from Networks table by STUDENT_ID. This method is used in ExcelParser class under Parser folder
         * */
        public void DeleteByStudentId(string id)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, Pathname);
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    var del = db.CreateCommand();
                    del.CommandText = $"DELETE FROM NETWORKS WHERE STUDENT_ID ='{id}';";
                    del.ExecuteNonQuery();

                    transaction.Commit();
                }

                db.Close();
            }
        }

    }
}
