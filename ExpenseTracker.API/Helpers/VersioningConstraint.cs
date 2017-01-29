using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Routing;

namespace ExpenseTracker.API.Helpers
{
    public class VersioningConstraint : IHttpRouteConstraint
    {
        public const string VersionHeaderName = "api-version";
        int DedaultVersion = 1;

        public int AllowedVersion
        {
            get;
            private set;
        }
        public VersioningConstraint(int allowedVersion)
        {
            AllowedVersion = allowedVersion;
        }


        bool IHttpRouteConstraint.Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            try
            {

                int? version = GetversionFromCustomheader(request);
                if (version == null)
                    version = GetVersionFromContentHeader(request);

                return ((version ?? DedaultVersion) == AllowedVersion);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        int? GetVersionFromContentHeader(HttpRequestMessage message)
        {
            try
            {
                var mediaType = message.Headers.Accept.Select(x => x.MediaType);

                string matchingMediaType = null;

                Regex regex = new Regex(@"application/vnd.expenseTracker.V[\d]+\+json");

                foreach (var media in mediaType)
                {
                    if (regex.IsMatch(media))
                        matchingMediaType = media;
                }

                if (matchingMediaType == null)
                    return null;

                // extract the version number
                Match m = regex.Match(matchingMediaType);
                string versionAsString = m.Groups[1].Value;

                // ... and return
                int version;
                if (versionAsString != null && Int32.TryParse(versionAsString, out version))
                {
                    return version;
                }

                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        int? GetversionFromCustomheader(HttpRequestMessage message)
        {
            try
            {
                IEnumerable<string> versionHeaderList;
                message.Headers.TryGetValues(VersionHeaderName, out versionHeaderList);
                string version = "";

                if (versionHeaderList != null && versionHeaderList.Count() == 1)
                    version = versionHeaderList.FirstOrDefault();

                int versionresult;
                if (!string.IsNullOrEmpty(version) && int.TryParse(version, out versionresult))
                    return versionresult;
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}