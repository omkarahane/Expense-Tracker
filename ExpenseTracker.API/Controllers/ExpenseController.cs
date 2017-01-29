using ExpenseTracker.Repository;
//using ExpenseTracker.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ExpenseTracker.Repository.Factories;
using ExpenseTracker.DTO;
using Marvin.JsonPatch;

namespace ExpenseTracker.API.Controllers
{
    [RoutePrefix("api")]
    public class ExpenseController : ApiController
    {
        IExpenseTrackerRepository repository;
        ExpenseFactory expenseFactory;
        ExpenseGroupFactory expenseGroupFactory;

        public ExpenseController()
        {
            repository = new ExpenseTrackerEFRepository(new ExpenseTracker.Repository.Entities.ExpenseTrackerContext());
            expenseFactory = new ExpenseFactory();
            expenseGroupFactory = new ExpenseGroupFactory();
        }

        [HttpGet]
        [Route("expence/{expenceid}/expencegroup/{expencegroupid}")]
        [Route("expence/{expenceid}")]
        public IHttpActionResult Get(int expenceid, int? expencegroupid = null)
        {
            try
            {
                if (expencegroupid != null)
                {
                    var expenceGroup = repository.GetExpenseGroup(expencegroupid.Value);
                    if (expenceGroup == null)
                        return NotFound();

                    var grpExpense = expenceGroup.Expenses.Where(e => e.Id == expenceid).FirstOrDefault();

                    if(grpExpense == null)
                        return NotFound();

                    var returnObj = new
                    {
                        Id = grpExpense.Id,
                        Amount = grpExpense.Amount,
                        Date=grpExpense.Date,
                        Description = grpExpense.Description,
                        ExpenseGroupId=grpExpense.ExpenseGroupId,
                    };
                    return Ok(returnObj); 
                }

                var expense = repository.GetExpense(expenceid);

                if (expense == null)
                    return NotFound();

                return Ok(expense);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
              var result=  repository.GetExpense(id);

                if (result == null)
                    return NotFound();
                else
                    return Ok(expenseFactory.CreateExpense(result));
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                return Ok(repository.GetExpenses().ToList().Select(e=> expenseFactory.CreateExpense(e)));
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult CreateExpense(Expense expense)
        {
            try
            {
                if (expense == null)
                    return BadRequest();

                var result=repository.InsertExpense(expenseFactory.CreateExpense(expense));

                if (result.Status == RepositoryActionStatus.Created)
                    return Created(Request.RequestUri + "/" + expense.Id, expenseFactory.CreateExpense(result.Entity));

                return BadRequest();

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPut]
        public IHttpActionResult UpdateExpense(Expense expense)
        {
            try
            {
                var etu = repository.GetExpense(expense.Id);

                if (etu == null)
                    return NotFound();

                var result= repository.UpdateExpense(expenseFactory.CreateExpense(expense));

                if (result.Status == RepositoryActionStatus.Updated)
                    return Ok(expenseFactory.CreateExpense(result.Entity));

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPatch]
        public IHttpActionResult PartiallyUpdateExpense(int id, JsonPatchDocument<Expense> expensePatchDoc)
        {
            try
            {
                var expenceEntity = repository.GetExpense(id);

                if (expenceEntity == null)
                    return NotFound();

                Expense expenceDto = expenseFactory.CreateExpense(expenceEntity);

                expensePatchDoc.ApplyTo(expenceDto);

                var result = repository.UpdateExpense(expenseFactory.CreateExpense(expenceDto));

                if (result.Status == RepositoryActionStatus.Updated)
                    return Ok(expenseFactory.CreateExpense(result.Entity));
                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [HttpDelete]
        public IHttpActionResult DeleteExpense(int id)
        {
            try
            {
                var etd = repository.GetExpense(id);
                if (etd == null)
                    return NotFound();

                var result=repository.DeleteExpense(id);

                if (result.Status == RepositoryActionStatus.Deleted)
                    return StatusCode(HttpStatusCode.NoContent);

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


    }
}
