using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ZedgraphWpfSimple
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new int Run()
        {
            int res;
            try
            {
                res = base.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return res;
        }
    }
}
