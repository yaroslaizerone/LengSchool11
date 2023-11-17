using LengSchool11.Entity;

namespace LengSchool11.Classes
{
    internal class Helper
    {
        public static bool flag = false;
        public static SchoolEntities context;

        public static SchoolEntities GetContext()
        {
            if (context == null)
                context = new SchoolEntities();
            return context;
        }
    }
}
