using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ExpenseTracker.API.Controllers
{
    public class ExenceGroupStatusController : ApiController
    {

        IExpenseTrackerRepository repository;
        ExpenseMasterDataFactory factory;

        public ExenceGroupStatusController()
        {
            repository = new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext());
            factory = new ExpenseMasterDataFactory();
        }


        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                return Ok(repository.GetExpenseGroupStatusses().ToList().Select(e=> factory.CreateExpenseGroupStatus(e)));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var egs = repository.GetExpenseGroupStatus(id);

                if (egs == null)
                    return NotFound();

                return Ok(factory.CreateExpenseGroupStatus(egs));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}