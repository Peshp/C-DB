SELECT TOP 5
	c.CountryName, r.RiverName
FROM Rivers AS r
JOIN CountriesRivers AS cr ON r.Id = cr.RiverId
FULL JOIN Countries AS c ON c.CountryCode = cr.CountryCode
WHERE c.ContinentCode = 'AF'
ORDER BY c.CountryName