﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Hubs;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;
        private readonly IMainHub mainHub;

        public EmployeeRepository(AppDbContext context, IMainHub mainHub)
        {
            this.context = context;
            this.mainHub = mainHub;
        }

        public IQueryable<Employee> Employees()
        {
            return context.Employees;
        }

        public IQueryable<Employee> EmployeeById(int id)
        {
            return context.Employees.Where(x => x.EmployeeId == id);
        }

        public async Task<bool> Create(Employee employee)
        {
            try
            {
                context.Employees.Add(employee);
                await context.SaveChangesAsync();

                await mainHub.NotifyAllClients();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> Update(Employee employee)
        {
            try
            {
                context.Employees.Update(employee);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> DeleteById(int id)
        {
            try
            {
                var employee = EmployeeById(id).FirstOrDefault();
                if (employee == null) return false;

                context.Employees.Remove(employee);
                await context.SaveChangesAsync();
                return true;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
    }
}
