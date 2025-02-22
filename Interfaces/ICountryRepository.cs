using System.Collections.Generic;
using PopulationGame.Models;

namespace PopulationGame.Interfaces;

public interface ICountryRepository
{
    IEnumerable<Country> GetAllCountries();
    Country GetRandomCountry();
    // Vid behov: Lägg till metoder för att hämta länder baserat på svårighetsgrad
}

