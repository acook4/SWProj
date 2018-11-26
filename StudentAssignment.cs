using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Software_Engineering
{
    
    
    class StudentAssignment
    {
        public static int numOfDoubles { get; set; } //MacKay rooms
        private static int numUsedDoubles = 0;
        public static int numOfSingles { get; set; } //Dunn rooms
        private static int numUsedSingles = 0;
        public static double reqCoefficient { get; set; } //how easy it is to qualify as a potential roommate, 0 is super easy, -1 will be always

        private static bool macKayFull ()
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

        private static bool roommateRequestMatch(Application a0, Application a1)
        {
            if (a0.studentID.Equals(a1.roommateID) && a0.roommateID.Equals(a1.studentID) &&
                a0.gender.Equals(a1.gender) && a0.preferBuilding.Equals(a1.preferBuilding) &&
                !a0.roommateID.Equals("") & !a1.roommateID.Equals(""))
                return true;
            else
                return false;
        }

        private static bool substanceRequire(Application a0, Application a1)
        {
            if ((!a0.smokes || a1.liveWithSmoke) && (!a0.drinks || a1.liveWithDrink) && (!a0.marijuana || a1.liveWithMarijuana))
                return true;
            else
                return false;
        }

        private static double areCompatible(Application a0, Application a1)
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

        private static List<Application> createAndAssignPools (List<Application>pool) //returns a list of applications that have been accepted, and need to be removed
        {
            List<Application> accepted = new List<Application>();

            foreach (Application a0 in pool) //create pools of potentials
            {
                if (!a0.confirmed && !macKayFull())
                {
                    List<Application> potentials = new List<Application>();
                    List<Double> potentialCoefficients = new List<Double>();
                    double coefficient;
                    foreach (Application a1 in pool)
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
                        potentials[highestIndex].confirmed = true;
                        accepted.Add(a0);
                        accepted.Add(potentials[highestIndex]);
                        numUsedDoubles++;
                        //pair the two, assign to MacKay
                        Console.WriteLine("MacKay: " + a0.studentID + " " + potentials[highestIndex].studentID);
                    }
                }
            }
            foreach (Application a0 in accepted) //cleaning the pool
                pool.Remove(a0);
            return accepted;
        }

        public static void assign (List<Application> applications)
        {

            foreach (Application a0 in applications) //matches roommate requests, assigns and removes
                if (!a0.confirmed && !macKayFull())
                    foreach (Application a1 in applications)
                        if (!a1.confirmed && !a0.confirmed && !a0.applicationID.Equals(a1.applicationID))
                            if (a0.roommateRequest && a1.roommateRequest)
                                if (roommateRequestMatch(a0, a1))
                                {
                                    a0.confirmed = true;
                                    a1.confirmed = true;
                                    numUsedDoubles++;
                                    Console.WriteLine("Requested: " + a0.studentID + " and " + a1.studentID);
                                    //pair the two, assign to MacKay
                                }

            List<Application> malePool = new List<Application>(); //gender divided pools
            List<Application> femalePool = new List<Application>();
            List<Application> otherPool = new List<Application>();

            foreach (Application a0 in applications)
                if (a0.preferBuilding.Equals("MacKay") && !a0.confirmed)
                {
                    if (a0.gender.Equals("male"))
                        malePool.Add(a0);
                    else if (a0.gender.Equals("female"))
                        femalePool.Add(a0);
                    else
                        otherPool.Add(a0);
                }

            List<Application> fulfilled = new List<Application>();
            fulfilled.AddRange(createAndAssignPools(malePool)); //matching MacKay roommates
            fulfilled.AddRange(createAndAssignPools(femalePool));
            fulfilled.AddRange(createAndAssignPools(otherPool));
            foreach (Application a0 in fulfilled)
                applications.Remove(a0); //remove fulfilled MacKay applications from List

            List<Application> dunnPool = new List<Application>();
            List<Application> dunnFulfilled = new List<Application>();

            foreach (Application a0 in applications)
                if (!a0.confirmed && a0.preferBuilding.Equals("Dunn"))
                    dunnPool.Add(a0);


            foreach (Application a0 in dunnPool) //assigning Dunn applicants
                if (!dunnFull())
                {
                    a0.confirmed = true;
                    dunnFulfilled.Add(a0);
                    numUsedSingles++;
                    //assign to Dunn
                    Console.WriteLine("Dunn: " + a0.studentID);
                }

            foreach (Application a0 in dunnFulfilled) //remove fulfilled Dunn applications from List
            {
                dunnPool.Remove(a0);
                applications.Remove(a0);
            }

            if (dunnFull() && !macKayFull()) //move Dunn overflow and rerun gendered pools for roommates
            {
                foreach (Application a0 in dunnPool)
                    if (!a0.confirmed)
                    {
                        if (a0.gender.Equals("male"))
                            malePool.Add(a0);
                        else if (a0.gender.Equals("female"))
                            femalePool.Add(a0);
                        else
                            otherPool.Add(a0);
                    }

                List<Application> newFulfilled = new List<Application>();
                newFulfilled.AddRange(createAndAssignPools(malePool)); //matching MacKay roommates
                newFulfilled.AddRange(createAndAssignPools(femalePool));
                newFulfilled.AddRange(createAndAssignPools(otherPool));
                foreach (Application a0 in newFulfilled)
                    applications.Remove(a0); //remove newFulfilled MacKay applications from List

                Console.WriteLine("The Dunn has been filled, and the MacKay might also have been filled.");
            }

            else if (!dunnFull() && macKayFull()) //move MacKay unmatchables and overflow to Dunn, as many as there is room
            {
                foreach (Application a0 in applications)

                    if (!dunnFull())
                    {
                        a0.confirmed = true;
                        numUsedSingles++;
                        //assign to Dunn
                        Console.WriteLine("Cleanup to Dunn: " + a0.studentID);
                    }
                applications.Clear();
                Console.WriteLine("The MacKay has been filled, and the Dunn might also have been filled.");
            }

            else if (dunnFull() && macKayFull()) //do nothing
            {
                applications.Clear();
                Console.WriteLine("Both the Dunn and the MacKay have been filled. There may be some students who were not accepted.");
            }

            else //move MacKay unmatchables to Dunn, as many as there is room
            {
                foreach (Application a0 in applications) 
                    if (!dunnFull())
                    {
                        a0.confirmed = true;
                        numUsedSingles++;
                        //assign to Dunn
                        Console.WriteLine("Cleanup to Dunn: " + a0.studentID);
                    }
                applications.Clear();
                Console.WriteLine("All or most applicants have been placed in their desired residences.");
            }

            numUsedDoubles = 0;
            numUsedSingles = 0;

            Console.Read();
        }
    }
}
