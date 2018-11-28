using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWProjv1
{
    class ResApplicationForm
    {
        public String applicationID { get; set; }
        public String studentID { get; set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
        public String otherName { get; set; }
        public String gender { get; set; }
        public String preferBuilding { get; set; }
        public String email { get; set; }
        public String streetAddress { get; set; }
        public String city { get; set; }
        public String region { get; set; }
        public String country { get; set; }
        public String postalCode { get; set; }
        public String phoneCountryCode { get; set; }
        public String phoneAreaCode { get; set; }
        public String phoneNumber { get; set; }
        public String socialLevel { get; set; }
        public String bedtime { get; set; }
        public String wakeUp { get; set; }
        public String volumeLevel { get; set; }
        public String cleanliness { get; set; }
        public String roommateName { get; set; }
        public String roommateID { get; set; }
        public String mealPlan { get; set; }
        public bool smokes { get; set; }
        public bool liveWithSmoke { get; set; }
        public bool drinks { get; set; }
        public bool liveWithDrink { get; set; }
        public bool marijuana { get; set; }
        public bool liveWithMarijuana { get; set; }
        public bool overnightVisitors { get; set; }
        public bool studiesInRoom { get; set; }
        public bool roommateRequest { get; set; }
        public String[] sports { get; set; }
        public String[] hobbies { get; set; }
        public String[] music { get; set; }
        public int schoolYear { get; set; }

        public bool confirmed { get; set; }

    }
}
