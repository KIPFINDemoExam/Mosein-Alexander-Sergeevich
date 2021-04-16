using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarService
{
    public static class DB
    {
        public static DirectoryInfo PhotoDirectory = new DirectoryInfo(@"..\..\Услуги автосервиса");
        public static Entities db = new Entities();
        public static Employees CurrentEmployee = new Employees();
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            var newcollection = new ObservableCollection<T>();
            collection.ToList().ForEach(it => newcollection.Add(it));
            return newcollection;
        }
    }
}
