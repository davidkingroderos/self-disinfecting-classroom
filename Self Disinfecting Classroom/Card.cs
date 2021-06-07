using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Self_Disinfecting_Classroom
{
    // Class That Stores The Data From Database
    public class Card
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string RFIDNumber { get; set; }
        public int ID { get; set; }

        public bool IsMale { get; set; }
        public string LoggedRoom { get; set; }

        public Card(int id, string firstName, string lastName, string rfidNumber, bool isMale)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.FullName = firstName + " " + lastName;
            this.RFIDNumber = rfidNumber;
            this.ID = id;
            this.IsMale = isMale;
            this.LoggedRoom = "Outside";

            Console.WriteLine($"{this.FullName} Detected On The Database...");
        }

        public void Login(Room room)
        {
            if (room.LoggedUsers.Contains(this))
            {
                this.Logout(room);
            }
            else
            {
                if (this.LoggedRoom != "Outside")
                {
                    var logoutRoom = Program.rooms.Find((x) => x.RoomNumber == this.LoggedRoom);
                    Logout(logoutRoom);
                }

                room.LoggedUsers.Add(this);
                this.LoggedRoom = room.RoomNumber;
            }
        }

        public void Logout(Room room)
        {
            this.LoggedRoom = "Outside";
            room.LoggedUsers.Remove(this);
        }

        public void DisinfectLogout(Room room)
        {
            this.LoggedRoom = "Outside";
        }

        public void ManualLogin(Room room)
        {
            if (room.LoggedUsers.Contains(this))
            {
                Console.WriteLine("This user is already logged in this room");
            }
            else
            {
                // Logout on other room first before logging in again
                if (this.LoggedRoom != "Outside")
                {
                    var logoutRoom = Program.rooms.Find((x) => x.RoomNumber == this.LoggedRoom);
                    Logout(logoutRoom);
                }

                Console.WriteLine($"{this.FullName} has log in to room {room.RoomNumber}");
                room.LoggedUsers.Add(this);
                this.LoggedRoom = room.RoomNumber;
            }
        }

        public void ManualLogout(Room room)
        {
            Console.WriteLine($"{this.FullName} has log out in to room {room.RoomNumber}");
            this.LoggedRoom = "Outside";
            room.LoggedUsers.Remove(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine($"Full Name: {this.FullName}");
            sb.AppendLine($"RFID Tag: {this.RFIDNumber}");
            sb.AppendLine($"ID: {this.ID}");
            sb.AppendLine($"Logged Room: {this.LoggedRoom}");

            return sb.ToString();
        }
    }
}
