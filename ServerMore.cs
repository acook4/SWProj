namespace Software_Engineering
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;


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
                                                ///NEW STUFF///
                                                
            private static void setCommandApplication(String type, String parameter) //parameter is schoolYear or applicationID
            {
                if (type.Equals("Application"))
                    command.CommandText = "SELECT * FROM Application WHERE schoolYear = " + parameter;
                else if (type.Equals("Sport"))
                    command.CommandText = "SELECT * FROM Sport, WHERE applicationID = " + parameter;
                else if (type.Equals("musicType"))
                    command.CommandText = "SELECT * FROM musicType, WHERE applicationID = " + parameter;
                else if (type.Equals("Hobby"))
                    command.CommandText = "SELECT * FROM Hobby, WHERE applicationID = " + parameter;
            }
                                   
            public static List<Application> runQueryApplication (int schoolYear) //collects the most recent year of applications
            {
                String schoolString = schoolYear.ToString();
                setCommandApplication("Application", schoolString);

                List<Application> applications = new List<Application>();
                SqlDataReader reader = command.ExecuteReader();
                SqlDataReader subtables;
                while(reader.Read())
                {
                    Application a0 = new Application();
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

                    setCommandApplication("Sport", applicationID); //gets Sports table
                    subtables = command.ExecuteReader();
                    List<String> sports = new List<String>();
                    while(subtables.Read())
                        sports.Add(subtables.GetString(1));
                    a0.sports = sports.ToArray();
                    subtables.Close();

                    setCommandApplication("musicType", applicationID); //gets Music table
                    subtables = command.ExecuteReader();
                    List<String> music = new List<String>();
                    while (subtables.Read())
                        music.Add(subtables.GetString(1));
                    a0.music = music.ToArray();
                    subtables.Close();

                    setCommandApplication("Hobby", applicationID); //gets Hobbies table
                    subtables = command.ExecuteReader();
                    List<String> hobbies = new List<String>();
                    while (subtables.Read())
                        hobbies.Add(subtables.GetString(1));
                    a0.hobbies = hobbies.ToArray();
                    subtables.Close();

                    applications.Add(a0);
                }
                reader.Close();

                return applications;
            }

                                             ///END OF NEW STUFF/// 


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

}
