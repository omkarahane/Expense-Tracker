using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;

namespace ExpenseTracker.API.Helpers
{
    public class VersionRoute : RouteFactoryAttribute
    {
        public int AllowedVersion{ get; private set; }

        public VersionRoute(string template,int allowedversion) : base(template)
        {
            AllowedVersion = allowedversion;
        }

        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints =new  HttpRouteValueDictionary();
                constraints.Add("version",new VersioningConstraint(AllowedVersion));
                return constraints;
            }
        }

    }
}