using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.Commons.Helper
{
    public static class CollectionHelper
    {
        public static void FindCollection(ObservableCollection<ActItems> sourceCollection, string findStringName, FunctionPointer delFunc)
        {
            var element = sourceCollection.FirstOrDefault(x => x.Name == findStringName);
            element.Func = delFunc;
        }

    }
}
