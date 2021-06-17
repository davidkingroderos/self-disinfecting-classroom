using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Self_Disinfecting_Classroom
{
    // Class That Defines All Classrooms
    public class Room
    {
        private bool isDisinfecting;

        public bool IsDisinfecting
        {
            get
            {
                if (this.LoggedUsers.Count <= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string RoomNumber { get; set; }

        public List<Card> LoggedUsers { get; set; }

        public Room(string roomNumber)
        {
            Console.WriteLine($"Turning On Room {roomNumber}...");
            this.RoomNumber = roomNumber;
            this.LoggedUsers = new List<Card>();
            this.isDisinfecting = false;
        }

        public void Disinfect()
        {
            Console.WriteLine($"Room {this.RoomNumber} is being disinfected.");
            for (int i = 0; i < LoggedUsers.Count; i++)
            {
                LoggedUsers[i].DisinfectLogout(this);
            }
            this.LoggedUsers.Clear();
            this.isDisinfecting = true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            
            sb.AppendLine($"Room Number: {this.RoomNumber}");
            sb.Append($"Logged Users: ");

            Console.ResetColor();
            foreach (var loggedUser in this.LoggedUsers)
            {
                sb.Append($"{loggedUser.FullName} || ");
            }
            sb.AppendLine();

            string isDisinfectingString = this.IsDisinfecting ? "Yes" : "No";
            sb.AppendLine($"Is Disinfecting: {isDisinfectingString}");
            sb.AppendLine($"Is Disinfecting: {isDisinfectingString}");

            return sb.ToString();
        }
    }
}
