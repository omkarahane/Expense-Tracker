using ExpenseTracker.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Factories
{
    public class ExpenseFactory
    {

        public ExpenseFactory()
        {

        }

        public DTO.Expense CreateExpense(Expense expense)
        {
            return new DTO.Expense()
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }



        public Expense CreateExpense(DTO.Expense expense)
        {
            return new Expense()
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }

        public object CreateDataShapedObject(Expense expense, List<string> parameters)
        {
            try
            {
                if (parameters == null || !parameters.Any())
                    return expense;

                ExpandoObject expandoObject = new ExpandoObject();

                foreach (string parameter in parameters)
                {
                    ((IDictionary<string, object>)expandoObject).Add(parameter, expense.GetType()
                                                                                .GetProperty(parameter, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                                                                .GetValue(expense));
                }

                return expandoObject;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        
         
    }
}
