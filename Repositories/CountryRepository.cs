using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PopulationGame.Interfaces;
using PopulationGame.Models;

namespace PopulationGame.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DapperContext _context;

        public CountryRepository(DapperContext context)
        {
            _context = context;
        }

        public IEnumerable<Country> GetAllCountries()
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                var sql = "SELECT CountryId, Name, Population FROM Countries";
                return connection.Query<Country>(sql).ToList();
            }
        }

        public Country GetRandomCountry()
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                // Använder NEWID() för att slumpmässigt hämta ett land, NEWID sätter ett slumpmässigt ID på raderna och vi hämtar TOP 1.
                var sql = "SELECT TOP 1 CountryId, Name, Population FROM Countries ORDER BY NEWID()";
                return connection.QueryFirstOrDefault<Country>(sql);
            }
        }
    }
}

