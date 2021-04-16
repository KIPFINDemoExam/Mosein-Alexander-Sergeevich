using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarService
{
    public static class DB
    {
        public static DirectoryInfo PhotoDirectory = new DirectoryInfo(@"..\..\Услуги автосервиса");
        public static CarEntities db = new CarEntities();
    }
}
