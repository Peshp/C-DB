USE [Geography]

SELECT [MountainRange], [PeakName], [Elevation]
	FROM [Mountains] AS m
LEFT JOIN [Peaks] AS p
	ON p.MountainId = m.Id
	WHERE [MountainRange] = 'Rila'
	ORDER BY [Elevation] DESC