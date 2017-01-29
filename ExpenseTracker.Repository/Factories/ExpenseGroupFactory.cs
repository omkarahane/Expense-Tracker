using ExpenseTracker.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Helpers;

namespace ExpenseTracker.Repository.Factories
{
    public class ExpenseGroupFactory
    {
        ExpenseFactory expenseFactory = new ExpenseFactory();

        public ExpenseGroupFactory()
        {

        }

        public ExpenseGroup CreateExpenseGroup(DTO.ExpenseGroup expenseGroup)
        {
            return new ExpenseGroup()
            {
                Description = expenseGroup.Description,
                ExpenseGroupStatusId = expenseGroup.ExpenseGroupStatusId,
                Id = expenseGroup.Id,
                Title = expenseGroup.Title,
                UserId = expenseGroup.UserId,
                Expenses = expenseGroup.Expenses == null ? new List<Expense>() : expenseGroup.Expenses.Select(e => expenseFactory.CreateExpense(e)).ToList()
            };
        }


        public DTO.ExpenseGroup CreateExpenseGroup(ExpenseGroup expenseGroup)
        {
            return new DTO.ExpenseGroup()
            {
                Description = expenseGroup.Description,
                ExpenseGroupStatusId = expenseGroup.ExpenseGroupStatusId,
                Id = expenseGroup.Id,
                Title = expenseGroup.Title,
                UserId = expenseGroup.UserId,
                Expenses = expenseGroup.Expenses.Select(e => expenseFactory.CreateExpense(e)).ToList()
            };
        }

        public object CreateDatashapedObject(ExpenseGroup expenseGroup, List<string> parameters)
        {
            try
            {


                //Expense expences = expenseGroup.Expenses;
                if (expenseGroup == null)
                    throw new ArgumentNullException();

                if (!parameters.Any(x => !string.IsNullOrEmpty(x)))
                    return expenseGroup;

                ExpandoObject expandoObject = new ExpandoObject();

                foreach (string field in parameters)
                {

                    if (!field.Equals("expences"))
                    {
                        var value = expenseGroup.GetType()
                                        .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                                        .GetValue(expenseGroup);

                        ((IDictionary<string, object>)expandoObject).Add(field, value);
                    }
                    else
                        break;
                }

                if (parameters.Any(x => x.Equals("expences")))
                {
                    int expenceId = parameters.IndexOf("expences");

                    object expences = new object();

                    if ((parameters.Count - 1) == expenceId)
                    {
                        int count = 1;
                        foreach (var expense in expenseGroup.Expenses)
                        {
                            expences = expenseFactory.CreateDataShapedObject(expense, null);
                            ((IDictionary<string, object>)expandoObject).Add("expences"+count++, expences);
                        }
                    }
                    else
                    {
                        List<object> expencesList = new List<object>();
                        List<string> paras = parameters.Skip(expenceId + 1).Take(parameters.Count - (expenceId + 1)).ToList();
                        foreach (var expense in expenseGroup.Expenses)
                        {
                            expencesList.Add(expenseFactory.CreateDataShapedObject(expense,paras));
                        }

                        ((IDictionary<string, object>)expandoObject).Add("expences", expencesList);

                    }

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