SELECT v.Name,
	COUNT(mv.MinionId) AS MinionsCount
FROM Villains AS v
LEFT JOIN MinionsVillains AS mv ON mv.VillainId = v.Id
JOIN Minions AS m ON m.Id = mv.MinionId
GROUP BY v.Name
HAVING COUNT(mv.MinionId) > 3