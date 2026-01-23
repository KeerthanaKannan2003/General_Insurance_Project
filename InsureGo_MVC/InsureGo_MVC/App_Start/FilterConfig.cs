<<<<<<< HEAD
﻿using System.Web;
using System.Web.Mvc;
=======
﻿using System.Web.Mvc;
>>>>>>> 9c8b818 (initial commit)

namespace InsureGo_MVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
