﻿using System;
using System.Collections.Generic;
using System.Linq;
using PayrollProcessor.Core.Domain.Features.Employees;
using PayrollProcessor.Data.Persistence.Infrastructure.Records;

namespace PayrollProcessor.Data.Persistence.Features.Employees
{
    public class EmployeeRecord : CosmosDBRecord
    {
        public string Department { get; set; } = "";
        public DateTimeOffset EmploymentStartedOn { get; set; }
        public string Email { get; set; } = "";
        public string EmailLower { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string FirstNameLower { get; set; } = "";
        public string LastName { get; set; } = "";
        public string LastNameLower { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Status { get; set; } = "";
        public string Title { get; set; } = "";

        public EmployeeRecord() => Type = nameof(EmployeeRecord);

        public static class Map
        {
            public static Employee ToEmployee(EmployeeRecord entity) =>
                new Employee(entity.Id)
                {
                    Department = entity.Department,
                    EmploymentStartedOn = entity.EmploymentStartedOn,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Phone = entity.Phone,
                    Email = entity.Email,
                    Status = entity.Status,
                    Title = entity.Title,
                    Version = entity.ETag
                };

            public static EmployeeDetail ToEmployeeDetails(EmployeeRecord entity, IEnumerable<EmployeePayrollRecord> payrolls) =>
                new EmployeeDetail(entity.Id)
                {
                    Department = entity.Department,
                    EmploymentStartedOn = entity.EmploymentStartedOn,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Phone = entity.Phone,
                    Email = entity.Email,
                    Status = entity.Status,
                    Title = entity.Title,
                    Version = entity.ETag,
                    Payrolls = payrolls.Select(EmployeePayrollRecord.Map.ToEmployeePayroll)
                };

            public static EmployeeRecord From(Employee employee)
            {
                var partitionId = employee.Id;

                return new EmployeeRecord
                {
                    Id = partitionId,
                    PartitionKey = partitionId.ToString(),
                    Department = employee.Department,
                    EmploymentStartedOn = employee.EmploymentStartedOn,
                    FirstName = employee.FirstName,
                    FirstNameLower = employee.FirstName.ToLowerInvariant(),
                    LastName = employee.LastName,
                    LastNameLower = employee.LastName.ToLowerInvariant(),
                    Email = employee.Email,
                    EmailLower = employee.Email.ToLowerInvariant(),
                    Phone = employee.Phone,
                    Status = employee.Status,
                    Title = employee.Title,
                };
            }

            public static EmployeeRecord From(EmployeeNew employee)
            {
                var partitionId = Guid.NewGuid();

                return new EmployeeRecord
                {
                    Id = partitionId,
                    PartitionKey = partitionId.ToString(),
                    Department = employee.Department,
                    EmploymentStartedOn = employee.EmploymentStartedOn,
                    FirstName = employee.FirstName,
                    FirstNameLower = employee.FirstName.ToLowerInvariant(),
                    LastName = employee.LastName,
                    LastNameLower = employee.LastName.ToLowerInvariant(),
                    Email = employee.Email,
                    EmailLower = employee.Email.ToLowerInvariant(),
                    Phone = employee.Phone,
                    Status = employee.Status,
                    Title = employee.Title
                };
            }
        }
    }
}
