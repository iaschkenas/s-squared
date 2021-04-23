using Domain;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Repository
{
    public class Personnel
    {
        private static string _connectionString = string.Empty;

        public Personnel()
        {
            try
            {
                _connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all roles
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<KeyValuePair<string, string>>> GetRoles()
        {
            var l = new List<KeyValuePair<string, string>>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT RoleID, RoleName FROM Role ORDER BY RoleName"
                    };

                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var kvp = new KeyValuePair<string, string>(reader[0].ToString(),
                                                                       reader[1].ToString());

                            l.Add(kvp);
                        }
                    }
                }
            }
            catch
            {
                //TODO: Logging
                throw;
            }

            return l;
        }

        /// <summary>
        /// Gets all Managers
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<KeyValuePair<string, string>>> GetManagers()
        {
            var l = new List<KeyValuePair<string, string>>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.Text,
                        CommandText = @"SELECT m.ManagerID, e.FirstName + ' ' + e.LastName
                                        FROM Manager m
                                        INNER JOIN Employee e
                                            ON e.EmployeeID = m.EmployeeID
                                        ORDER BY 2"
                    };

                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var kvp = new KeyValuePair<string, string>(reader[0].ToString(),
                                                                       reader[1].ToString());

                            l.Add(kvp);
                        }
                    }
                }
            }
            catch
            {
                //TODO: Logging
                throw;
            }

            return l;
        }

        /// <summary>
        /// Gets employees that report to a specific manager
        /// </summary>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Employee>> GetEmployeesByManager(string managerId)
        {
            var l = new List<Employee>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT EmployeeIdentifier, FirstName, LastName FROM Employee WHERE ManagerID = @ManagerID"
                    };
                    cmd.Parameters.Add("@ManagerID", SqlDbType.Int).Value = managerId;

                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var employee = new Employee();
                            employee.EmployeeId = reader[0].ToString();
                            employee.FirstName = reader[1].ToString();
                            employee.LastName = reader[2].ToString();

                            l.Add(employee);
                        }
                    }
                }
            }
            catch
            {
                //TODO: Logging
                throw;
            }

            return l;
        }

        /// <summary>
        /// Inserts an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task<int> InsertEmployee(Employee employee)
        {
            int employeeId = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.Text,
                        CommandText = @"INSERT INTO Employee (EmployeeIdentifier, FirstName, LastName, ManagerID) VALUES 
                                                             (@EmployeeIdentifier, @FirstName, @LastName, @ManagerID);
	                                    SET @EmployeeID = SCOPE_IDENTITY();"
                    };

                    cmd.Parameters.Add("@EmployeeIdentifier", SqlDbType.VarChar).Value = employee.EmployeeId;
                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = employee.FirstName;
                    cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = employee.LastName;
                    cmd.Parameters.Add("@ManagerID", SqlDbType.Int).Value = employee.ManagerID;
                    SqlParameter p = new SqlParameter("@EmployeeID", employeeId);
                    p.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(p);

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    employeeId = (int)p.Value;
                }
            }
            catch
            {
                //TODO: Logging
                throw;
            }

            return employeeId;
        }

        /// <summary>
        /// Inserts roles for a specific employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task InsertEmployeeRoles(int employeeId, IEnumerable<int> roles)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    foreach (int roleId in roles)
                    {
                        SqlCommand cmd = new SqlCommand
                        {
                            Connection = con,
                            CommandType = CommandType.Text,
                            CommandText = @"INSERT INTO EmployeeRole (EmployeeID, RoleID) VALUES (@EmployeeID, @RoleID);"
                        };

                        cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = employeeId;
                        cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch
            {
                //TODO: Logging
                throw;
            }
        }

    }
}