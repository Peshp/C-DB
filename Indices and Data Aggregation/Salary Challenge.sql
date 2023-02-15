

SELECT TOP 10 [FirstName], [LastName], [DepartmentID] 
FROM Employees AS e
WHERE e.[Salary] > (
					SELECT AVG(Salary) AS avg
					FROM Employees AS es					
					WHERE es.DepartmentID = e.DepartmentID
					
				)
ORDER BY e.DepartmentID