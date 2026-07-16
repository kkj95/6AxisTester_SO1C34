using FZ4P.Commons.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    //초기 생성 타임에 컴포지션에 의한 Lazyinit이 필요한상황에 init로직을 태우기 위함.
    public class BootLoadinit
    {
        public BootLoadinit()
        {
            STATIC.Rcp.ConditionExt.ActionList.AddRange(STATIC.Process.ItemList.Select(x => x.Name));
            DataIOHelper.SerializeToXMLViewerFile<Condition, ConditionExtra>(STATIC.Rcp.Condition, STATIC.Rcp.ConditionExt, STATIC.ViewRecipeDir + STATIC.Rcp.Current.ConditionName);
        }
    }
}
