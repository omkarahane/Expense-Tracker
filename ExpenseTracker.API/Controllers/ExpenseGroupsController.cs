using ExpenseTracker.DTO;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Marvin.JsonPatch;
using ExpenseTracker.API.Helpers;
using System.Web.Http.Routing;
using System.Web;

namespace ExpenseTracker.API.Controllers
{
    public class ExpenseGroupsController : ApiController
    {
        IExpenseTrackerRepository expenceRepository;
        ExpenseGroupFactory egFactory = new ExpenseGroupFactory();
        const int maxPageSize = 30;

        public ExpenseGroupsController()
        {
            expenceRepository = new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext());
        }

        public ExpenseGroupsController(IExpenseTrackerRepository expenceRepository)
        {
            this.expenceRepository = expenceRepository;
        }

        [VersionRoute("api/GetExpenceGroup",1,Name ="GetExpenseGroup")]
        public IHttpActionResult GetExpenceGroup(string sortParameters="id",string status="",string userId=""
            ,string fields ="",int pageNumber=1, int pageSize=10)
        {
            try
            {
                if (pageSize > maxPageSize)
                    pageSize = maxPageSize;

                int? statusId = -1;

                List<string> fieldList = fields.Split(',').ToList();
                bool areExpencesRequsted = fieldList.Any(x => x.Equals("expences"));
                

                if (!string.IsNullOrEmpty(status))
                statusId = expenceRepository.GetExpenseGroupStatusses().Where(x=>x.Description.Equals(status)).FirstOrDefault().Id;

                var result=(areExpencesRequsted? expenceRepository.GetExpenseGroupsWithExpenses() : expenceRepository.GetExpenseGroups())
                    .ApplySorting(sortParameters)
                    .Where(x => statusId.Value == -1 || statusId == null || x.ExpenseGroupStatusId == statusId)
                    .Where(x => string.IsNullOrEmpty(userId) || x.UserId.Equals(userId))
                    .ToList();

                int totalCount = result.Count;
                int totalPages = (int)Math.Ceiling((double)totalCount/pageSize);


                if (pageNumber > totalPages)
                    pageNumber = totalPages;

                var urlhelper = new UrlHelper(Request);

                var nextpageUrl = pageNumber==totalPages? "" :urlhelper.Link("GetExpenseGroup",new {
                    sortParameters = sortParameters,
                    status = status,
                    userId = userId,
                    fields = fields,
                    pageNumber = pageNumber+1,
                    pageSize= pageSize
                });


                var previousPageUrl=pageNumber==1 ? "" : urlhelper.Link("GetExpenseGroup", new
                {
                    sortParameters = sortParameters,
                    status = status,
                    userId = userId,
                    fields=fields,
                    pageNumber = pageNumber-1,
                    pageSize = pageSize
                });


                var paginationHeader = new
                {
                    currentPage=pageNumber,
                    totalPages=totalPages,
                    pageSize= pageSize,
                    nextPageUrl =nextpageUrl,
                    previousPageUrl=previousPageUrl,

                };

                HttpContext.Current.Response.Headers.Add("PaginationHeader", Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

                return Ok(result.Take(pageSize * pageNumber)
                                .Skip((pageNumber-1)*pageSize)
                                .ToList()
                                .Select(e => egFactory.CreateDatashapedObject(e, fieldList)));

                //return Ok(expenceRepository.GetExpenseGroups()
                //    .ApplySorting(sortParameters)
                //    .Where(x=> statusId.Value==-1 || statusId==null || x.ExpenseGroupStatusId==statusId)
                //    .Where(x=> string.IsNullOrEmpty(userId) || x.UserId.Equals(userId))
                //    .ToList()
                //    .Select(e => egFactory.CreateExpenseGroup(e)));
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [VersionRoute("api/GetExpenceGroup", 2, Name = "GetExpenseGroupV2")]
        public IHttpActionResult GetExpenceGroupV2(string sortParameters = "id", string status = "", string userId = ""
            , string fields = "", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageSize > maxPageSize)
                    pageSize = maxPageSize;

                int? statusId = -1;

                List<string> fieldList = fields.Split(',').ToList();
                bool areExpencesRequsted = fieldList.Any(x => x.Equals("expences"));


                if (!string.IsNullOrEmpty(status))
                    statusId = expenceRepository.GetExpenseGroupStatusses().Where(x => x.Description.Equals(status)).FirstOrDefault().Id;

                var result = (areExpencesRequsted ? expenceRepository.GetExpenseGroupsWithExpenses() : expenceRepository.GetExpenseGroups())
                    .ApplySorting(sortParameters)
                    .Where(x => statusId.Value == -1 || statusId == null || x.ExpenseGroupStatusId == statusId)
                    .Where(x => string.IsNullOrEmpty(userId) || x.UserId.Equals(userId))
                    .ToList();

                int totalCount = result.Count;
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


                if (pageNumber > totalPages)
                    pageNumber = totalPages;

                var urlhelper = new UrlHelper(Request);

                var nextpageUrl = pageNumber == totalPages ? "" : urlhelper.Link("GetExpenseGroup", new
                {
                    sortParameters = sortParameters,
                    status = status,
                    userId = userId,
                    fields = fields,
                    pageNumber = pageNumber + 1,
                    pageSize = pageSize
                });


                var previousPageUrl = pageNumber == 1 ? "" : urlhelper.Link("GetExpenseGroup", new
                {
                    sortParameters = sortParameters,
                    status = status,
                    userId = userId,
                    fields = fields,
                    pageNumber = pageNumber - 1,
                    pageSize = pageSize
                });


                var paginationHeader = new
                {
                    currentPage = pageNumber,
                    totalPages = totalPages,
                    pageSize = pageSize,
                    nextPageUrl = nextpageUrl,
                    previousPageUrl = previousPageUrl,

                };

                HttpContext.Current.Response.Headers.Add("PaginationHeader", Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

                return Ok(result.Take(pageSize * pageNumber)
                                .Skip((pageNumber - 1) * pageSize)
                                .ToList()
                                .Select(e => egFactory.CreateDatashapedObject(e, fieldList)));

                //return Ok(expenceRepository.GetExpenseGroups()
                //    .ApplySorting(sortParameters)
                //    .Where(x=> statusId.Value==-1 || statusId==null || x.ExpenseGroupStatusId==statusId)
                //    .Where(x=> string.IsNullOrEmpty(userId) || x.UserId.Equals(userId))
                //    .ToList()
                //    .Select(e => egFactory.CreateExpenseGroup(e)));
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }


        public IHttpActionResult GetExpenceGroup(int id)
        {
            try
            {
                var expenceGroup = expenceRepository.GetExpenseGroup(id);

                if (expenceGroup == null)
                    return NotFound();

                return Ok(egFactory.CreateExpenseGroup(expenceGroup));

            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult CreateExpenceGroup(ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                    return BadRequest();

                var result = expenceRepository.InsertExpenseGroup(egFactory.CreateExpenseGroup(expenseGroup));
                if (result.Status == RepositoryActionStatus.Created)
                {
                    return Created(Request.RequestUri + "/" + result.Entity.Id.ToString(), egFactory.CreateExpenseGroup(result.Entity));
                }

                return BadRequest();
            }
            catch (Exception ex)
            {

                return InternalServerError();
            }
        }

        [HttpPut]
        public IHttpActionResult UpdateExpenceGroup(ExpenseGroup ExpenseGroup)
        {

            try
            {
                if (ExpenseGroup == null)
                    return BadRequest();

                var result = expenceRepository.UpdateExpenseGroup(egFactory.CreateExpenseGroup(ExpenseGroup));

                if (result.Status == RepositoryActionStatus.NotFound)
                    return NotFound();

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    return Ok(result.Entity);
                }

                return BadRequest();

            }
            catch (Exception)
            {
                return InternalServerError();
            }

        }


        [HttpPatch]
        public IHttpActionResult PartialUpdate(int id, JsonPatchDocument<ExpenseTracker.DTO.ExpenseGroup> jpd)
        {
            try
            {
                var eg =  expenceRepository.GetExpenseGroup(id);
                if (eg == null)
                    return NotFound();

                var egDto = egFactory.CreateExpenseGroup(eg);
                jpd.ApplyTo(egDto);

               var result= expenceRepository.UpdateExpenseGroup(egFactory.CreateExpenseGroup(egDto));

                if (result.Status == RepositoryActionStatus.NotFound)
                    return NotFound();

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    return Ok(egFactory.CreateExpenseGroup(result.Entity));
                }

                return BadRequest();
            }
            catch (Exception)
            {
               return InternalServerError();
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteExpenceGroup(int id)
        {
            try
            {
                var result =expenceRepository.DeleteExpenseGroup(id);
                if (result.Status == RepositoryActionStatus.NotFound)
                    return NotFound();
                if (result.Status == RepositoryActionStatus.Deleted)
                    return StatusCode(HttpStatusCode.NoContent);

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }

}
