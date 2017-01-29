using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace ExpenseTracker.API.Helpers
{
    public static class IQueryableExtension
    {

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> source, string sortignParameters)
        {
            try
            {
                if (source == null)
                    throw new ArgumentNullException();

                if (string.IsNullOrEmpty(sortignParameters))
                    return source;

                List<string> parameters = sortignParameters.Split(',').ToList();

                string consolodatedParameters = string.Empty; 

                foreach (string parameter in parameters)
                {
                    if (parameter.StartsWith("-"))
                    {
                        consolodatedParameters += parameter.Remove(0, 1) + " descending,";
                    }
                    else
                    {
                        consolodatedParameters += parameter+",";
                    }

                }

                consolodatedParameters = consolodatedParameters.Remove((consolodatedParameters.Length - 1), 1);

                return source.OrderBy(consolodatedParameters);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}