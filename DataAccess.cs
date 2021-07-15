using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Data.SqlClient;


namespace Assignment2_MinWooPark
{  
    class DataAccess
    {
        private SqlConnection _conn;
        private SqlDataAdapter _adapter;
        private SqlCommandBuilder _cmdBuilder;
        private DataSet _dataSet;
        private DataTable _tblVehicle; 
        private DataTable _tblInventory;
        private DataTable _tblRepair;

        // constructor
        public DataAccess()
        {
            string cs = GetConnectionString("CarRepairManagementSystemMdf");   // change the connection string name to match yours
            string query = "Select ID, make, model, year, newOrUsed from Vehicle; Select ID, vehicleID, numberOnhand, price, cost from Inventory; Select ID, inventoryID,whatTorepair from Repair";
     
            _conn = new SqlConnection(cs);
            _adapter = new SqlDataAdapter(query, _conn);
            _cmdBuilder = new SqlCommandBuilder(_adapter);
        
            FillDataSet();
        }

        // method to read the connection string from the JSON file
        public string GetConnectionString(string connectionStringName)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("config.json");
            IConfiguration config = configurationBuilder.Build();

            return config["ConnectionStrings:" + connectionStringName];
        }

        // method to refresh the dataset
        private void FillDataSet()
        {
            _conn.Close();
            // reset the dataset
            _dataSet = new DataSet();

            _adapter.Fill(_dataSet);

            _tblVehicle = _dataSet.Tables[0];
            _tblInventory = _dataSet.Tables[1];
            _tblRepair = _dataSet.Tables[2];

            // define primary key for vehicle
            DataColumn[] pk = new DataColumn[1];
            pk[0] = _tblVehicle.Columns["ID"];
            _tblVehicle.PrimaryKey = pk;

            // define primary key for inventory
            DataColumn[] pk1 = new DataColumn[1];
            pk1[0] = _tblInventory.Columns["ID"];
            _tblInventory.PrimaryKey = pk1;

            //define primary key for repair
            DataColumn[] pk2 = new DataColumn[1];
            pk2[0] = _tblRepair.Columns["ID"];
            _tblRepair.PrimaryKey = pk2;

            DataColumn parentInVehicle;
            DataColumn childrenInInventory;
            parentInVehicle = _dataSet.Tables[0].Columns["ID"];
            childrenInInventory = _dataSet.Tables[1].Columns["vehicleID"];

            ForeignKeyConstraint inventoryFK = new ForeignKeyConstraint(parentInVehicle,childrenInInventory);
            inventoryFK.DeleteRule = Rule.Cascade;
            // Cannot delete a customer value that has associated existing orders.  
            _dataSet.Tables[1].Constraints.Add(inventoryFK);
            _dataSet.EnforceConstraints = true;

            DataColumn parentInInventory;
            DataColumn childrenInRepair;
            parentInInventory = _dataSet.Tables[1].Columns["ID"];
            childrenInRepair = _dataSet.Tables[2].Columns["inventoryID"];
            ForeignKeyConstraint repairFK = new ForeignKeyConstraint(parentInInventory, childrenInRepair);
            repairFK.DeleteRule = Rule.Cascade;
            _dataSet.Tables[2].Constraints.Add(repairFK);
            _dataSet.EnforceConstraints = true;
        }

        public void GetAllVehicles()
        {
            // display products
            Console.WriteLine("\n\n+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n");
            Console.WriteLine($"{"ID",5} {"make",10} {"model",10} {"year",10} {"newOrUsed",10}");
            Console.WriteLine();
            foreach (DataRow row in _tblVehicle.Rows)
            {
                Console.WriteLine($"{row["ID"],5} {row["make"],10} {row["model"],10} {row["year"],10} {row["newOrUsed"],10}");
            }
        }

        public void GetAllInventory()
        {
            // display products
            Console.WriteLine("\n\n+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n");
            Console.WriteLine($"{"ID",5} {"vehicleID",10} {"numberOnhand",10} {"price",10} {"cost",10}");
            Console.WriteLine();
            foreach (DataRow row in _tblInventory.Rows)
            {
                Console.WriteLine($"{row["ID"],5} {row["vehicleID"],10} {row["numberOnhand"],10} {row["price"],10} {row["cost"],10}");
            }
        }

        public void GetAllRepair()
        {
            // display products
            Console.WriteLine("\n\n+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n");
            Console.WriteLine($"{"ID",10} {"inventoryID",10} {"whatTorepair",10}" );
            Console.WriteLine();
            foreach (DataRow row in _tblRepair.Rows)
            {
                Console.WriteLine($"{row["ID"],10} {row["inventoryID"],10} {row["whatTorepair"],10}");
            }
        }

        public void InsertVehicle(string make, string model, int year, string newOrUsed)
        {
            DataRow newRow = _tblVehicle.NewRow();  // create a new row
            newRow["ID"] = 0;                 // add a dummy value to ProductID, 
                                                     // since it'll be overwritten by the SQL Server 
                                                   // because ProductID is set as 
                                                     // Identity (Auto-increment) in the database
            newRow["make"] = make;
            newRow["model"] = model;
            newRow["year"] = year;
            newRow["newOrUsed"] = newOrUsed;
            _tblVehicle.Rows.Add(newRow);           // add the new row to the Rows collection

            // read the INSERT query from the SqlCommandBuilder object
            _adapter.InsertCommand = _cmdBuilder.GetInsertCommand();
            _adapter.Update(_tblVehicle);  // save the changes to database

            FillDataSet();  // refresh dataset
        }

        public void InsertInventory(int vehicleId, string numberOnhand,string price, string cost)
        {
            DataRow vehicleRow = _tblVehicle.Rows.Find(vehicleId);
     
            //check vehicle id exists or not
            if (vehicleRow == null)
            {
                Console.WriteLine("you insert invalid vehicle Id. Try again.");
            }
            else
            {
                bool checkVehicleId = true;
                
                // check if vehicle id was used or not.
                 foreach (DataRow row in _tblInventory.Rows)
               {
                    int vehicleID = int.Parse($"{row["vehicleID"],10}") ;

                    if (vehicleID == vehicleId)
                   {
                        checkVehicleId = false;
                   }
               }

                if(checkVehicleId == true)
                {
                   
                    string cs = GetConnectionString("CarRepairManagementSystemMdf");
                    string query = "Insert into Inventory (vehicleID, numberOnhand, price, cost) values (@vehicleId, @numberOnhand, @price, @cost)";

                    
                    using (SqlConnection conn = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand(query, _conn);
                        cmd.Parameters.AddWithValue("vehicleID", vehicleId);
                        cmd.Parameters.AddWithValue("numberOnhand", numberOnhand);
                        cmd.Parameters.AddWithValue("price", price);
                        cmd.Parameters.AddWithValue("cost", cost);
                        _conn.Open();

                        int result = cmd.ExecuteNonQuery();

                        if(result ==1)
                        {
                            Console.WriteLine("It successfuly is inserted");
                        }
                        else
                        {
                            Console.WriteLine("Not Inserted");
                        }
                    }

                    FillDataSet();  // refresh dataset
                } else
                {
                    Console.WriteLine(vehicleId + " already was used. Try differnet vehicle ID ");
                }
            }
        }

        public void InsertRepair(int inventoryId, string whatToRepair)
        {
            DataRow inventoryRow = _tblInventory.Rows.Find(inventoryId);

            //check vehicle id exists or not
            if (inventoryRow == null)
            {
                Console.WriteLine("you insert invalid vehicle Id. Try again.");
            }
            else
            {
                bool checkInventoryId = true;

                // check if vehicle id was used or not.
                foreach (DataRow row in _tblRepair.Rows)
                {
                    int inventoryID = int.Parse($"{row["inventoryID"],10}");

                    if (inventoryID == inventoryId)
                    {
                        checkInventoryId = false;
                    }
                }

                if (checkInventoryId == true)
                {

                    string cs = GetConnectionString("CarRepairManagementSystemMdf");
                    string query = "Insert into Repair (inventoryID, whatTorepair) values (@inventoryId, @whatToRepair)";


                    using (SqlConnection conn = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand(query, _conn);
                        cmd.Parameters.AddWithValue("inventoryID", inventoryId);
                        cmd.Parameters.AddWithValue("whatTorepair", whatToRepair);
                        
                        _conn.Open();

                        int result = cmd.ExecuteNonQuery();

                        if (result == 1)
                        {
                            Console.WriteLine("It successfuly is inserted");
                        }
                        else
                        {
                            Console.WriteLine("Not Inserted");
                        }
                    }

                    FillDataSet();  // refresh dataset
                }
                else
                {
                    Console.WriteLine("Inventory ID " + inventoryId + " is already was used. Try differnet inventory ID ");
                }
            }
        }

        public void UpdateVehicle(int id, string make, string model, int year, string newOrUsed)
        {
            DataRow row = _tblVehicle.Rows.Find(id);

            if (row != null)
            {
                row["make"] = make;
                row["model"] = model;
                row["year"] = year;
                row["newOrUsed"] = newOrUsed;

                _adapter.UpdateCommand = _cmdBuilder.GetUpdateCommand();
                _adapter.Update(_tblVehicle);

                FillDataSet();
            }
            else
                Console.WriteLine("\nInvalid vehicle ID. Please try again.");
        }

        public void UpdateInventory(int id, string numberOnhand, int price, int cost)
        {
            DataRow row = _tblInventory.Rows.Find(id);

            if (row != null)
            {

                string cs = GetConnectionString("CarRepairManagementSystemMdf");
                string query = "Update Inventory SET numberOnhand = @numberOnhand, price = @price, cost= @cost where ID = @id";


                using (SqlConnection conn = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand(query, _conn);

                    cmd.Parameters.AddWithValue("ID", id);
                    cmd.Parameters.AddWithValue("numberOnhand", numberOnhand);
                    cmd.Parameters.AddWithValue("price", price);
                    cmd.Parameters.AddWithValue("cost", cost);
                    _conn.Open();

                    int result = cmd.ExecuteNonQuery();

                    if (result == 1)
                    {
                        Console.WriteLine("It successfuly is modified");
                    }
                    else
                    {
                        Console.WriteLine("Not Modified");
                    }
                }

                FillDataSet();
            }
            else
                Console.WriteLine("\nInvalid Inventory ID. Please try again.");
        }

        public void UpdateRepair(int id, string whatToRepair)
        {
            DataRow row = _tblRepair.Rows.Find(id);

            if (row != null)
            {

                string cs = GetConnectionString("CarRepairManagementSystemMdf");
                string query = "Update Repair SET whatTorepair = @whatToRepair where ID = @id";


                using (SqlConnection conn = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand(query, _conn);

                    cmd.Parameters.AddWithValue("ID", id);
                    cmd.Parameters.AddWithValue("whatTorepair", whatToRepair);
                   
                    _conn.Open();

                    int result = cmd.ExecuteNonQuery();

                    if (result == 1)
                    {
                        Console.WriteLine("It successfuly is modified");
                    }
                    else
                    {
                        Console.WriteLine("Not Modified");
                    }
                }

                FillDataSet();
            }
            else
                Console.WriteLine("\nInvalid Repair ID. Please try again.");
        }

        public void DeleteVehicle(int id)
        {
            DataRow row = _tblVehicle.Rows.Find(id);

            if (row != null)
            {
                row.Delete();

                _adapter.DeleteCommand = _cmdBuilder.GetDeleteCommand();
                _adapter.Update(_tblVehicle);

                FillDataSet();
            }
            else
                Console.WriteLine("\nInvalid Vehicle ID. Please try again.");
        }

        public void DeleteInventory(int id)
        {
            DataRow row = _tblInventory.Rows.Find(id);

            if (row != null)
            {
                string cs = GetConnectionString("CarRepairManagementSystemMdf");
                string query = "Delete from Inventory where ID = @id";

                using (SqlConnection conn = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand(query, _conn);

                    cmd.Parameters.AddWithValue("ID", id);
                    _conn.Open();

                    int result = cmd.ExecuteNonQuery();

                    if (result == 1)
                    {
                        Console.WriteLine("It successfuly is deleted");
                    }
                    else
                    {
                        Console.WriteLine("Not Deleted");
                    }
                }

                FillDataSet();
            }
            else
                Console.WriteLine("\nInvalid Inventory ID. Please try again.");
        }

        public void DeleteRepair(int id)
        {
            DataRow row = _tblRepair.Rows.Find(id);

            if (row != null)
            {
                string cs = GetConnectionString("CarRepairManagementSystemMdf");
                string query = "Delete from Repair where ID = @id";

                using (SqlConnection conn = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand(query, _conn);

                    cmd.Parameters.AddWithValue("ID", id);
                    _conn.Open();

                    int result = cmd.ExecuteNonQuery();

                    if (result == 1)
                    {
                        Console.WriteLine("It successfuly is deleted");
                    }
                    else
                    {
                        Console.WriteLine("Not Deleted");
                    }
                }

                FillDataSet();
            }
            else
                Console.WriteLine("\nInvalid Repair ID. Please try again.");
        }

    }
}
