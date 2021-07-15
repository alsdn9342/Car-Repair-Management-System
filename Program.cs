using System;

namespace Assignment2_MinWooPark
{
    class Program
    {
        static void Main(string[] args)
        {
            DataAccess crud = new DataAccess();
            Boolean menu = true;

            do
            {
                int choice = DisplayMenu();

                    if (choice == 1)
                    {

                    bool vehicle = true;

                    while (vehicle)
                    {
                        int choiceOfVehicle = DisplayVehicleMenu();

                        switch (choiceOfVehicle)
                        {
                            case 1: // Get All vehicles
                                crud.GetAllVehicles();
                                break;


                            case 2: // Insert new vehicle
                                Console.Write("\nEnter make: ");
                                string make = Console.ReadLine();

                                Console.Write("Enter model: ");
                                string model = Console.ReadLine();

                                Console.Write("Enter year: ");
                                int year = int.Parse(Console.ReadLine());

                                Console.Write("Enter new or used: ");
                                string newOrUsed = Console.ReadLine();

                                crud.InsertVehicle(make, model, year, newOrUsed);
                                break;

                            case 3: // update Product
                                Console.Write("\nEnter ID: ");
                                int id = int.Parse(Console.ReadLine());

                                Console.Write("\nEnter make: ");
                                string makeUpdate = Console.ReadLine();

                                Console.Write("Enter model: ");
                                string modelUpdate = Console.ReadLine();

                                Console.Write("Enter year: ");
                                int yearUpdate = int.Parse(Console.ReadLine());

                                Console.Write("Enter new or used: ");
                                string newOrUsedUpdate = Console.ReadLine();


                                crud.UpdateVehicle(id, makeUpdate, modelUpdate, yearUpdate, newOrUsedUpdate);
                                break;


                            case 4: // Delete vehicle
                                Console.Write("\nEnter Product Id: ");
                                id = int.Parse(Console.ReadLine());

                                crud.DeleteVehicle(id);
                                break;

                            case 5:
                                vehicle = false;
                                break;

                            default:
                                Console.WriteLine("Invalid option! Try again!");
                                break;
                        }
                    }

                 
                    }
                    else if (choice == 2)
                    {
                    bool inventory = true;

                    while (inventory)
                    {
                        int choiceOfInventory = DisplayInventoryMenu();

                        switch (choiceOfInventory)
                        {
                              case 1: // Insert new vehicle
                                   Console.Write("\nEnter vehicle ID: ");
                                   int vehicleID = int.Parse(Console.ReadLine()); 

                                   Console.Write("Enter number on hand: ");
                                   string numbernhand = Console.ReadLine();

                                   Console.Write("Enter price: ");
                                   string price = Console.ReadLine();

                                   Console.Write("Enter cost: ");
                                   string cost = Console.ReadLine();

                                   crud.InsertInventory(vehicleID, numbernhand, price, cost);
                                   break;

                              case 2: // Get All vehicles
                                   crud.GetAllInventory();
                                   break;
                            
                              case 3: 
                                     Console.Write("\nEnter Product Id: ");
                                     int id = int.Parse(Console.ReadLine());

                                     Console.Write("Enter number on hand: ");
                                     string numbernhandToUpdate = Console.ReadLine();

                                      Console.Write("Enter price: ");
                                      int priceToUpdate = int.Parse(Console.ReadLine());

                                      Console.Write("Enter cost: ");
                                      int costToUpdate = int.Parse(Console.ReadLine());
                                      
                                      crud.UpdateInventory(id,numbernhandToUpdate,priceToUpdate,costToUpdate);
                                      break;
                              case 4:
                                      Console.Write("\nEnter Product Id: ");
                                      int idToDelete = int.Parse(Console.ReadLine());

                                      crud.DeleteInventory(idToDelete);
                                      break;


                              case 5:
                                     inventory = false;
                                     break;

                             default:
                                    Console.WriteLine("Invalid option! Try again!");
                                    break;
                        
                        }
                    }


                    }
                    else if (choice == 3)
                    {

                    bool repair = true;

                    while (repair)
                    {
                        int choiceOfRepair = DisplayRepairMenu();

                        switch (choiceOfRepair)
                        {
                            
                            case 1: // Insert new repair
                                Console.Write("\nEnter inventory ID: ");
                                int inventoryID = int.Parse(Console.ReadLine());

                                Console.Write("Enter what to repair: ");
                                string whatToRepair = Console.ReadLine();

                                crud.InsertRepair(inventoryID,whatToRepair);
                                break;
                          

                            case 2:
                                crud.GetAllRepair();
                                break;
                            
                               case 3:
                                   Console.Write("\nEnter Repair Id: ");
                                   int id = int.Parse(Console.ReadLine());

                                   Console.Write("Enter what to repair: ");
                                   string whatToRepairToUpdate = Console.ReadLine();

                                   crud.UpdateRepair(id, whatToRepairToUpdate);
                                   break;
                            
                            case 4:
                                   Console.Write("\nEnter Product Id: ");
                                   int idToDelete = int.Parse(Console.ReadLine());

                                   crud.DeleteRepair(idToDelete);
                                   break;

                            case 5:
                                repair = false;
                                break;

                            default:
                                Console.WriteLine("Invalid option! Try again!");
                                break;

                        }
                    }

                    }
                    else if (choice == 4)
                    {
                    menu = false;
                    }
                    

                
            } while (menu);
        }

        static int DisplayMenu()
        {
            Console.WriteLine("\n\n+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n");
            Console.WriteLine("\t1 - Press 1 to modify vehicles");
            Console.WriteLine("\t2 - Press 2 to modify inventory");
            Console.WriteLine("\t3 - Press 3 to modify repair");
            Console.WriteLine("\t4 - Press 4 to exit program");

            Console.Write("\nEnter your choice: ");
            return int.Parse(Console.ReadLine());
        }

        static int DisplayVehicleMenu()
        {
            Console.WriteLine("\n\n+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n");
            Console.WriteLine("\t1 - Press 1 to list all vehicles");
            Console.WriteLine("\t2 - Press 2 to add a new vehicle");
            Console.WriteLine("\t3 - Press 3 to update vehicle");
            Console.WriteLine("\t4 - Press 4 to delete program");
            Console.WriteLine("\t5 - Press 5 to return to main menu");

            Console.Write("\nEnter your choice: ");
            return int.Parse(Console.ReadLine());
        }

        static int DisplayInventoryMenu()
        {
            Console.WriteLine("\n\n+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n");
            Console.WriteLine("\t1 - Press 1 to insert new inventory");
            Console.WriteLine("\t2 - Press 2 to view inventory for vehicle");
            Console.WriteLine("\t3 - Press 3 to update inventory");
            Console.WriteLine("\t4 - Press 4 to delete inventory");
            Console.WriteLine("\t5 - Press 5 to return to main menu");

            Console.Write("\nEnter your choice: ");
            return int.Parse(Console.ReadLine());
        }

        static int DisplayRepairMenu()
        {
            Console.WriteLine("\n\n+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n");
            Console.WriteLine("\t1 - Press 1 to insert new repair");
            Console.WriteLine("\t2 - Press 2 to view all repair");
            Console.WriteLine("\t3 - Press 3 to update repair");
            Console.WriteLine("\t4 - Press 4 to delete repair");
            Console.WriteLine("\t5 - Press 5 to return to main menu");

            Console.Write("\nEnter your choice: ");
            return int.Parse(Console.ReadLine());
        }
    }
}
    

