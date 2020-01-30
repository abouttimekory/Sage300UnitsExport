using Sage.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage300UnitsExport
{
    //GAC32 Needed References
    //Sage.Data.CRE.Interfaces
    //Sage.LS1.LibraryManagement
    //Sage.LS1.Messaging
    //Sage.LS1.Messaging.Interfaces

    //C:\Program Files (x86)\Timberline Office\Shared
    //Sage.AOF
    //Sage.Data.AOF
    //Sage.Data.CRE
    //Sage.Data.CRE.Connect
    //Sage.Data.CRE.Interfaces
    //Sage.STO.TransactionService
    //Sage.STO.TransactionServiceJCAddin
    class UnitsExportDriver
    {
        static void Main(string[] args)
        {
                //"C:\\Program Files (x86)\\Timberline Office\\Shared"
            Console.WriteLine("v1.17 - Sage300 v18x");
            DateTime myDateTime = new DateTime();
            Console.WriteLine((int)DayOfWeek.Sunday);
            Console.WriteLine("DT:" + myDateTime.ToString());
            Console.WriteLine(myDateTime.CompareTo(DateTime.MinValue));
            LibraryManager.AddSupplementaryResolveDirectory(Path.Combine(LibraryManager.SharedLibrariesLocation, Properties.Settings.Default.path));
            new UnitsExportManager().handleUnitsExport();
            Console.ReadLine();
        }
    }
}
