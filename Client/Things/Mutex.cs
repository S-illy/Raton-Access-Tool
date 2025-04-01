using System.Threading;

namespace Client.Things
{
    internal class MutexControl
    {
        public static Mutex meow;
        public static bool CreateMutex()
        {
            meow = new Mutex(false, Config.Mutex, out bool createdNew);
            return createdNew;
        }
        public static void CloseMutex()
        {
            if (meow != null)
            {
                meow.Close();
                meow = null;
            }
        }
    }
}
