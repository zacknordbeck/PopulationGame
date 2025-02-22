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
                var sql = "SELECT * FROM Countries"; //TODO: Skriv ut kolumnnamn
                return connection.Query<Country>(sql).ToList();
            }
        }

        public Country GetRandomCountry()
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                // Använder NEWID() för att slumpmässigt hämta ett land
                var sql = "SELECT TOP 1 * FROM Countries ORDER BY NEWID()"; //TODO: Skriv ut kolumnnamn
                return connection.QueryFirstOrDefault<Country>(sql);
            }
        }
    }
}

