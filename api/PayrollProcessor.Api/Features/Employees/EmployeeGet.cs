using System;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using PayrollProcessor.Core.Domain.Features.Employees;
using PayrollProcessor.Core.Domain.Intrastructure.Operations.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace PayrollProcessor.Api.Features.Employees
{

    public class EmployeeGet : BaseAsyncEndpoint<Guid, EmployeeDetail>
    {
        private readonly IQueryDispatcher dispatcher;

        public EmployeeGet(IQueryDispatcher dispatcher)
        {
            Guard.Against.Null(dispatcher, nameof(dispatcher));

            this.dispatcher = dispatcher;
        }

        [HttpGet("employees/{employeeId:Guid}"), MapToApiVersion("1")]
        [SwaggerOperation(
            Summary = "Gets a specific employee",
            Description = "Gets a specific employee specified by the route parameter with all payrolls",
            OperationId = "Employee.Get",
            Tags = new[] { "Employees" })
        ]
        public override Task<ActionResult<EmployeeDetail>> HandleAsync([FromRoute] Guid employeeId) =>
            dispatcher
                .Dispatch(new EmployeeDetailQuery(employeeId))
                .Match<EmployeeDetail, ActionResult<EmployeeDetail>>(
                    e => e,
                    () => NotFound(),
                    ex => new APIErrorResult(ex.Message));
    }
}
