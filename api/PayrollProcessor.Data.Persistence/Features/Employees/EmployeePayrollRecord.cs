using System;
using PayrollProcessor.Core.Domain.Features.Employees;
using PayrollProcessor.Data.Persistence.Infrastructure.Records;

namespace PayrollProcessor.Data.Persistence.Features.Employees;

/// <summary>
/// The datastore representation of a single payroll record for a given employee
/// </summary>
public class EmployeePayrollRecord : CosmosDBRecord
{
    public DateTimeOffset CheckDate { get; set; }
    public decimal GrossPayroll { get; set; }
    public string PayrollPeriod { get; set; } = string.Empty;

    public EmployeePayrollRecord() => Type = nameof(EmployeePayrollRecord);

    public static class Map
    {
        public static EmployeePayroll ToEmployeePayroll(EmployeePayrollRecord entity) =>
            new EmployeePayroll(entity.Id)
            {
                EmployeeId = Guid.Parse(entity.PartitionKey),
                CheckDate = entity.CheckDate,
                GrossPayroll = entity.GrossPayroll,
                PayrollPeriod = entity.PayrollPeriod,
                Version = entity.ETag,
            };

        public static EmployeePayrollRecord From(EmployeePayroll payroll) =>
            new EmployeePayrollRecord
            {
                Id = payroll.Id,
                PartitionKey = payroll.EmployeeId.ToString(),
                CheckDate = payroll.CheckDate,
                GrossPayroll = payroll.GrossPayroll,
                PayrollPeriod = payroll.PayrollPeriod,
                ETag = payroll.Version
            };

        public static EmployeePayrollRecord From(Employee employee, Guid newPayrollId, EmployeePayrollNew payroll) =>
            new EmployeePayrollRecord
            {
                Id = newPayrollId,
                PartitionKey = employee.Id.ToString(),
                CheckDate = payroll.CheckDate,
                GrossPayroll = payroll.GrossPayroll,
                PayrollPeriod = payroll.PayrollPeriod,
            };

        public static EmployeePayrollRecord Merge(EmployeePayrollUpdateCommand command)
        {
            var recordToUpdate = From(command.EntityToUpdate);

            recordToUpdate.CheckDate = command.CheckDate;
            recordToUpdate.GrossPayroll = command.GrossPayroll;
            recordToUpdate.PayrollPeriod = command.PayrollPeriod;

            return recordToUpdate;
        }
    }
}
