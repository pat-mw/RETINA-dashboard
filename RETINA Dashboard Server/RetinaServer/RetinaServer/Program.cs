using System;
using System.Threading;

namespace RetinaServer
{
    class Program
    {
        public static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title ="Retina Dashboard Server";

            isRunning = true;

            // create new thread to run game loop on
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(5, 26950);            
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread has started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
