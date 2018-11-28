using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SWProjv1
{
    class Server
    {
        static SqlConnection sql;
        public static SqlCommand command;

        public static bool Init()
        {
            try
            {
                sql = new SqlConnection("Data Source =ZACHMAC\\SQLEXPRESS; Initial Catalog = SEProjectDB ; Integrated Security = SSPI");
                sql.Open();
                command = sql.CreateCommand();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static void setCommand(String type, String searchTerm)
        {
            if (type.Equals("Room"))
                command.CommandText = "SELECT * FROM " + type;
            else if (type.Equals("Student"))
                command.CommandText = "SELECT * FROM Student, User_T WHERE Student.UserID = User_T.UserID";
        }

        //NEW STUFF//

        private static void setCommandApplication(String type, String parameter) //parameter is schoolYear or applicationID
        {
            if (type.Equals("Application"))
                command.CommandText = "SELECT * FROM Application WHERE schoolYear = " + parameter;
            else if (type.Equals("Sport"))
                command.CommandText = "SELECT * FROM Sport WHERE applicationID = " + parameter;
            else if (type.Equals("musicType"))
                command.CommandText = "SELECT * FROM musicType WHERE applicationID = " + parameter;
            else if (type.Equals("Hobby"))
                command.CommandText = "SELECT * FROM Hobby WHERE applicationID = " + parameter;
        }

                    

        public static List<ResApplicationForm> runQueryApplication(int schoolYear) //collects the most recent year of applications
        {
            String schoolString = schoolYear.ToString();
            setCommandApplication("Application", schoolString);

            List<ResApplicationForm> applications = new List<ResApplicationForm>();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ResApplicationForm a0 = new ResApplicationForm();
                String applicationID = reader.GetString(0).Trim();
                a0.applicationID = applicationID;
                a0.studentID = reader.GetString(1).Trim();
                a0.firstName = reader.GetString(2).Trim();
                a0.lastName = reader.GetString(3).Trim();
                a0.otherName = reader.GetString(4).Trim();
                a0.schoolYear = schoolYear;
                a0.gender = reader.GetString(6).Trim();
                a0.email = reader.GetString(7).Trim();
                a0.streetAddress = reader.GetString(8).Trim();
                a0.city = reader.GetString(9).Trim();
                a0.region = reader.GetString(10).Trim();
                a0.country = reader.GetString(11).Trim();
                a0.postalCode = reader.GetString(12).Trim();
                a0.phoneCountryCode = reader.GetString(13).Trim();
                a0.phoneAreaCode = reader.GetString(14).Trim();
                a0.phoneNumber = reader.GetString(15).Trim();
                a0.preferBuilding = reader.GetString(16).Trim();
                a0.smokes = reader.GetBoolean(17);
                a0.liveWithSmoke = reader.GetBoolean(18);
                a0.drinks = reader.GetBoolean(19);
                a0.liveWithDrink = reader.GetBoolean(20);
                a0.marijuana = reader.GetBoolean(21);
                a0.liveWithMarijuana = reader.GetBoolean(22);
                a0.socialLevel = reader.GetString(23).Trim();
                a0.bedtime = reader.GetString(24).Trim();
                a0.wakeUp = reader.GetString(25).Trim();
                a0.volumeLevel = reader.GetString(26).Trim();
                a0.overnightVisitors = reader.GetBoolean(27);
                a0.cleanliness = reader.GetString(28).Trim();
                a0.studiesInRoom = reader.GetBoolean(29);
                a0.roommateRequest = reader.GetBoolean(30);
                a0.roommateName = reader.GetString(31).Trim();
                a0.roommateID = reader.GetString(32).Trim();
                a0.mealPlan = reader.GetString(33).Trim();

                applications.Add(a0);
            }
            reader.Close();

            foreach (ResApplicationForm a0 in applications)
            {
                setCommandApplication("Sport", a0.applicationID); //gets Sports table
                reader = command.ExecuteReader();
                List<String> sports = new List<String>();
                while (reader.Read())
                    sports.Add(reader.GetString(1));
                a0.sports = sports.ToArray();
                reader.Close();

                setCommandApplication("musicType", a0.applicationID); //gets Music table
                reader = command.ExecuteReader();
                List<String> music = new List<String>();
                while (reader.Read())
                    music.Add(reader.GetString(1));
                a0.music = music.ToArray();
                reader.Close();

                setCommandApplication("Hobby", a0.applicationID); //gets Hobbies table
                reader = command.ExecuteReader();
                List<String> hobbies = new List<String>();
                while (reader.Read())
                    hobbies.Add(reader.GetString(1));
                a0.hobbies = hobbies.ToArray();
                reader.Close();
            }

            return applications;
        }

        public static String getEmptyRoomID(String dorm)
        {
            String emptyRooms = "select Room.roomID from Room, RoomHistory where Room.roomID = RoomHistory.roomID" +
                " AND RoomHistory.dateLeft IS NOT NULL AND Room.building = '" + dorm + "'";
            command.CommandText = emptyRooms;

            int times = 0;
            String roomID = "";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read() && times == 0)
            {
                roomID = reader.GetString(0).Trim();
                times++;
            }
            reader.Close();

            return roomID;
        }

        public static String getRoomNum(String dorm)
        {
            String roomID = getEmptyRoomID(dorm);
            String emptyRooms = "select roomNum from Room where roomID = '" + roomID + "'";
            command.CommandText = emptyRooms;

            int times = 0;
            String roomNum = "";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read() && times == 0)
            {
                roomNum = reader.GetString(0).Trim();
                times++;
            }
            reader.Close();

            return roomNum;
        }

        public static String getAdjoiningID(String roomID, String roomNum)
        {
            String adjoining = "select roomID from Room where roomID != '" + roomID + "' AND roomNum = '" + roomNum + "'";
            command.CommandText = adjoining;

            int times = 0;
            String adRoomID = "";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read() && times == 0)
            {
                adRoomID = reader.GetString(0).Trim();
                times++;
            }
            reader.Close();

            return adRoomID;
        }

        public static void assignRoom(String studentID, String roomID)
        {
            DateTime dateTime = DateTime.Now;
            String format = "yyyy-MM-dd HH:mm:ss";
            
            System.Data.SqlTypes.SqlDateTime d = new System.Data.SqlTypes.SqlDateTime(2018, 11, 27);
            String insertStmt = "INSERT [dbo].[RoomHistory] ([roomID], [studentID], [dateEntered], [dateLeft]) " +
                "VALUES (N'" + roomID + "', N'" + studentID + "', "+ dateTime.ToString(format)+", NULL)";
            command.CommandText = insertStmt;
            command.ExecuteNonQuery();
        }
                //END OF NEW STUFF//
        
        public static List<ListBoxItem> runQuery(String type)
        {
            List<ListBoxItem> results = new List<ListBoxItem>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                switch (type)
                {
                    case "SWProjv1.Room":
                        String roomSide;
                        try
                        {
                            roomSide = reader.GetString(1).Trim();
                        }
                        catch (Exception excep)
                        {
                            roomSide = "";
                        }
                        Room room = new Room(
                            reader.GetString(0).Trim(),
                            roomSide,
                            reader.GetString(2).Trim(),
                            reader.GetString(3).Trim(),
                            reader.GetString(4).Trim(),
                            reader.GetString(5).Trim()
                        );
                        room.setListBoxItem();
                        results.Add(room.listboxitem);
                        break;
                    case "SWProjv1.Student":
                        Student student = new Student(
                                
                            );
                        break;
                }
            }
            reader.Close();
            return results;
        }
    }
}
