using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWProjv1
{


    class StudentAssignment
    {
        public static int numOfDoubles { get; set; } //MacKay rooms
        private static int numUsedDoubles = 0;
        public static int numOfSingles { get; set; } //Dunn rooms
        private static int numUsedSingles = 0;
        public static double reqCoefficient { get; set; } //how easy it is to qualify as a potential roommate, 0 is super easy, -1 will be always

        private static bool macKayFull()
        {
            if (numUsedDoubles < numOfDoubles)
                return false;
            else
                return true;
        }

        private static bool dunnFull()
        {
            if (numUsedSingles < numOfSingles)
                return false;
            else
                return true;
        }

        private static bool roommateRequestMatch(ResApplicationForm a0, ResApplicationForm a1)
        {
            if (a0.studentID.Equals(a1.roommateID) && a0.roommateID.Equals(a1.studentID) &&
                a0.gender.Equals(a1.gender) && a0.preferBuilding.Equals(a1.preferBuilding) &&
                !a0.roommateID.Equals("") & !a1.roommateID.Equals(""))
                return true;
            else
                return false;
        }

        private static bool substanceRequire(ResApplicationForm a0, ResApplicationForm a1)
        {
            if ((!a0.smokes || a1.liveWithSmoke) && (!a0.drinks || a1.liveWithDrink) && (!a0.marijuana || a1.liveWithMarijuana))
                return true;
            else
                return false;
        }

        private static double areCompatible(ResApplicationForm a0, ResApplicationForm a1)
        {
            double howGood = 0;

            if (a0.schoolYear.Equals(a1.schoolYear))
                howGood += 2;
            if (a0.country.Equals(a1.country))
                howGood += 2;
            if (a0.socialLevel.Equals(a1.socialLevel))
                howGood += 2;
            if (a0.volumeLevel.Equals(a1.volumeLevel))
                howGood += 2;
            if (a0.bedtime.Equals(a1.bedtime))
                howGood += 1;
            if (a0.wakeUp.Equals(a1.wakeUp))
                howGood += 1;
            if (a0.overnightVisitors.Equals(a1.overnightVisitors))
                howGood += 1;
            if (a0.cleanliness.Equals(a1.cleanliness))
                howGood += 1;
            if (a0.studiesInRoom.Equals(a1.studiesInRoom))
                howGood += 1;
            foreach (String h0 in a0.hobbies)
                foreach (String h1 in a1.hobbies)
                    if (h0.Equals(h1))
                        howGood += 0.25;
            foreach (String s0 in a0.sports)
                foreach (String s1 in a1.sports)
                    if (s0.Equals(s1))
                        howGood += 0.25;
            foreach (String m0 in a0.music)
                foreach (String m1 in a1.music)
                    if (m0.Equals(m1))
                        howGood += 0.25;

            return howGood;
        }

        private static List<ResApplicationForm> createAndAssignPools(List<ResApplicationForm> pool) //returns a list of applications that have been accepted, and need to be removed
        {
            List<ResApplicationForm> accepted = new List<ResApplicationForm>();

            foreach (ResApplicationForm a0 in pool) //create pools of potentials
            {
                if (!a0.confirmed && !macKayFull())
                {
                    List<ResApplicationForm> potentials = new List<ResApplicationForm>();
                    List<Double> potentialCoefficients = new List<Double>();
                    double coefficient;
                    foreach (ResApplicationForm a1 in pool)
                        if (!a1.confirmed && !a0.applicationID.Equals(a1.applicationID) && !macKayFull())
                            if (substanceRequire(a0, a1) && substanceRequire(a1, a0)) //the two fulfill substance boolean
                                if ((coefficient = areCompatible(a0, a1)) > reqCoefficient) //the two fulfill coefficent requirement
                                {
                                    potentials.Add(a1);
                                    potentialCoefficients.Add(coefficient);
                                }
                    if (potentials.Count() == 0) //has no potential roommates, may be reconsidered after adding Dunn overflow
                    {

                    }
                    else
                    {
                        int highestIndex = 0;
                        for (int i = 1; i < potentials.Count; i++)
                        {
                            if (potentialCoefficients[i] > potentialCoefficients[highestIndex])
                                highestIndex = i;
                        }
                        a0.confirmed = true;
                        ResApplicationForm a1 = potentials[highestIndex];
                        a1.confirmed = true;
                        accepted.Add(a0);
                        accepted.Add(a1);
                        numUsedDoubles++;
                        //pair the two, assign to MacKay
                        String roomID0 = Server.getEmptyRoomID("Mackay");
                        String roomNum0 = Server.getRoomNum(roomID0);
                        String roomID1 = Server.getAdjoiningID(roomID0, roomNum0);
                        Server.assignRoom(a0.studentID, roomID0);
                        Server.assignRoom(a1.studentID, roomID1);
                    }
                }
            }
            foreach (ResApplicationForm a0 in accepted) //cleaning the pool
                pool.Remove(a0);
            return accepted;
        }

        public static void assign(List<ResApplicationForm> applications)
        {

            foreach (ResApplicationForm a0 in applications) //matches roommate requests, assigns and removes
                if (!a0.confirmed && !macKayFull())
                    foreach (ResApplicationForm a1 in applications)
                        if (!a1.confirmed && !a0.confirmed && !a0.applicationID.Equals(a1.applicationID))
                            if (a0.roommateRequest && a1.roommateRequest)
                                if (roommateRequestMatch(a0, a1))
                                {
                                    a0.confirmed = true;
                                    a1.confirmed = true;
                                    numUsedDoubles++;
                                    Console.WriteLine("Requested: " + a0.studentID + " and " + a1.studentID);
                                    //pair the two, assign to MacKay
                                    String roomID0 = Server.getEmptyRoomID("Mackay");
                                    String roomNum0 = Server.getRoomNum(roomID0);
                                    String roomID1 = Server.getAdjoiningID(roomID0, roomNum0);
                                    Server.assignRoom(a0.studentID, roomID0);
                                    Server.assignRoom(a1.studentID, roomID1);
                                }

            List<ResApplicationForm> malePool = new List<ResApplicationForm>(); //gender divided pools
            List<ResApplicationForm> femalePool = new List<ResApplicationForm>();
            List<ResApplicationForm> otherPool = new List<ResApplicationForm>();

            foreach (ResApplicationForm a0 in applications)
                if (a0.preferBuilding.Equals("Mackay") && !a0.confirmed)
                {
                    if (a0.gender.Equals("male"))
                        malePool.Add(a0);
                    else if (a0.gender.Equals("female"))
                        femalePool.Add(a0);
                    else
                        otherPool.Add(a0);
                }

            List<ResApplicationForm> fulfilled = new List<ResApplicationForm>();
            fulfilled.AddRange(createAndAssignPools(malePool)); //matching MacKay roommates
            fulfilled.AddRange(createAndAssignPools(femalePool));
            fulfilled.AddRange(createAndAssignPools(otherPool));
            foreach (ResApplicationForm a0 in fulfilled)
                applications.Remove(a0); //remove fulfilled MacKay applications from List

            List<ResApplicationForm> dunnPool = new List<ResApplicationForm>();
            List<ResApplicationForm> dunnFulfilled = new List<ResApplicationForm>();

            foreach (ResApplicationForm a0 in applications)
                if (!a0.confirmed && a0.preferBuilding.Equals("Dunn"))
                    dunnPool.Add(a0);


            foreach (ResApplicationForm a0 in dunnPool) //assigning Dunn applicants
                if (!dunnFull())
                {
                    a0.confirmed = true;
                    dunnFulfilled.Add(a0);
                    numUsedSingles++;
                    //assign to Dunn
                    String roomID0 = Server.getEmptyRoomID("Dunn");
                    String roomNum0 = Server.getRoomNum(roomID0);
                    Server.assignRoom(a0.studentID, roomID0);
                }

            foreach (ResApplicationForm a0 in dunnFulfilled) //remove fulfilled Dunn applications from List
            {
                dunnPool.Remove(a0);
                applications.Remove(a0);
            }

            if (dunnFull() && !macKayFull()) //move Dunn overflow and rerun gendered pools for roommates
            {
                foreach (ResApplicationForm a0 in dunnPool)
                    if (!a0.confirmed)
                    {
                        if (a0.gender.Equals("male"))
                            malePool.Add(a0);
                        else if (a0.gender.Equals("female"))
                            femalePool.Add(a0);
                        else
                            otherPool.Add(a0);
                    }

                List<ResApplicationForm> newFulfilled = new List<ResApplicationForm>();
                newFulfilled.AddRange(createAndAssignPools(malePool)); //matching MacKay roommates
                newFulfilled.AddRange(createAndAssignPools(femalePool));
                newFulfilled.AddRange(createAndAssignPools(otherPool));
                foreach (ResApplicationForm a0 in newFulfilled)
                    applications.Remove(a0); //remove newFulfilled MacKay applications from List

                //Console.WriteLine("The Dunn has been filled, and the MacKay might also have been filled.");
            }

            else if (!dunnFull() && macKayFull()) //move MacKay unmatchables and overflow to Dunn, as many as there is room
            {
                foreach (ResApplicationForm a0 in applications)

                    if (!dunnFull())
                    {
                        a0.confirmed = true;
                        numUsedSingles++;
                        //assign to Dunn
                        String roomID0 = Server.getEmptyRoomID("Dunn");
                        String roomNum0 = Server.getRoomNum(roomID0);
                        Server.assignRoom(a0.studentID, roomID0);
                    }
                applications.Clear();
                //Console.WriteLine("The MacKay has been filled, and the Dunn might also have been filled.");
            }

            else if (dunnFull() && macKayFull()) //do nothing
            {
                applications.Clear();
                //Console.WriteLine("Both the Dunn and the MacKay have been filled. There may be some students who were not accepted.");
            }

            else //move MacKay unmatchables to Dunn, as many as there is room
            {
                foreach (ResApplicationForm a0 in applications)
                    if (!dunnFull())
                    {
                        a0.confirmed = true;
                        numUsedSingles++;
                        //assign to Dunn
                        String roomID0 = Server.getEmptyRoomID("Dunn");
                        String roomNum0 = Server.getRoomNum(roomID0);
                        Server.assignRoom(a0.studentID, roomID0);
                        //Console.WriteLine("Cleanup to Dunn: " + a0.studentID);
                    }
                applications.Clear();
                //Console.WriteLine("All or most applicants have been placed in their desired residences.");
            }

            //Console.Read();
        }
    }
}

