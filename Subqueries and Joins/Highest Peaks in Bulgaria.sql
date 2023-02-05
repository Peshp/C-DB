SELECT c.CountryCode, m.MountainRange, p.PeakName, p.Elevation
FROM Countries AS c
JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
	AND c.CountryName = 'Bulgaria'
JOIN Mountains AS m ON mc.MountainId = m.Id
JOIN Peaks AS p ON p.MountainId = m.Id
	AND p.Elevation > 2835
ORDER BY p.Elevation DESC