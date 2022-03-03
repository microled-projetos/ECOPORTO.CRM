using System;
using System.ServiceProcess;

namespace Ecoporto.CRM.Service
{
    static class Program
    {
        static void Main()
        {
//            ServiceBase[] ServicesToRun;

  //          ServicesToRun = new ServiceBase[]
    //        {
      //         new Workflow()
        //    };

          //  ServiceBase.Run(ServicesToRun);

            Workflow workflow = new Workflow();
            workflow.Iniciar();
            Console.WriteLine("");
        }
    }
}
