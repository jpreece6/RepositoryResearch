using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEngine.Contexts;

namespace RepoConsole.Presenter
{
    public abstract class Presenter
    {
        protected SessionContext SessionContext;

        protected virtual bool OpenSession()
        {
            Console.WriteLine("\nConnecting....");

            try
            {
                SessionContext.OpenContextSession();
                Console.Clear();

                if (SessionContext.IsLocal())
                {
                    Console.WriteLine("Could not connect to server!\n- Now using local DB for this operation!\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to server!");
                return false;
            }

            return true;
        }
    }
}
