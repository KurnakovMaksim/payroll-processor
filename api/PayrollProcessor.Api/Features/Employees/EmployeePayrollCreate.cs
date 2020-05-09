using System;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using PayrollProcessor.Core.Domain.Features.Employees;
using PayrollProcessor.Core.Domain.Intrastructure.Identifiers;
using PayrollProcessor.Core.Domain.Intrastructure.Operations.Commands;
using PayrollProcessor.Core.Domain.Intrastructure.Operations.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace PayrollProcessor.Api.Features.Employees
{
    public class EmployeePayrollCreate : BaseAsyncEndpoint<EmployeePayrollCreateRequest, EmployeePayroll>
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IEntityIdGenerator generator;

        public EmployeePayrollCreate(
            IQueryDispatcher queryDispatcher,
            ICommandDispatcher commandDispatcher,
            IEntityIdGenerator generator)
        {
            Guard.Against.Null(queryDispatcher, nameof(queryDispatcher));
            Guard.Against.Null(commandDispatcher, nameof(commandDispatcher));
            Guard.Against.Null(generator, nameof(generator));

            this.commandDispatcher = commandDispatcher;
            this.generator = generator;
            this.queryDispatcher = queryDispatcher;
        }

        [HttpPost("employees/payrolls"), MapToApiVersion("1")]
        [SwaggerOperation(
            Summary = "Creates a new employee payroll",
            Description = "Creates a new employee payroll for the specific employee specified by the route parameter",
            OperationId = "EmployeePayroll.Create",
            Tags = new[] { "Employees", "Payrolls" })
        ]
        public override Task<ActionResult<EmployeePayroll>> HandleAsync([FromBody] EmployeePayrollCreateRequest request) =>
            queryDispatcher
                .Dispatch(new EmployeeQuery(request.EmployeeId))
                .Bind(employee =>
                {
                    var newPayroll = new EmployeePayrollNew
                    {
                        CheckDate = request.CheckDate,
                        GrossPayroll = request.GrossPayroll,
                        PayrollPeriod = request.PayrollPeriod
                    };

                    var newPayrollId = generator.Generate();

                    var command = new EmployeePayrollCreateCommand(employee, newPayrollId, newPayroll);

                    return commandDispatcher.Dispatch(command);
                })
                .Match<EmployeePayroll, ActionResult<EmployeePayroll>>(
                    employeePayroll => employeePayroll,
                    () => NotFound($"Could not find employee [{request.EmployeeId}]"),
                    ex => new APIErrorResult(ex.Message));
    }

    public class EmployeePayrollCreateRequest
    {
        public Guid EmployeeId { get; set; }
        public DateTimeOffset CheckDate { get; set; }
        public decimal GrossPayroll { get; set; }
        public string PayrollPeriod { get; set; } = "";
    }
}
