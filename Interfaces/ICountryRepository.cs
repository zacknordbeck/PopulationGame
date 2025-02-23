using System.Collections.Generic;
using PopulationGame.Models;

namespace PopulationGame.Interfaces;

public interface ICountryRepository
{
    IEnumerable<Country> GetAllCountries();
    Country GetRandomCountry();
}

