﻿--CREATE DATABASE Database;
--GO
--USE Database;
--CREATE TABLE Locations
--(
--	[ID] int IDENTITY(1,1),
--	[COORDINATES] sys.geography NOT NULL
--)
--INSERT INTO Locations (COORDINATES) VALUES 
--									(sys.geography::Point(54.9827317,82.8941761,4326)),
--									(sys.geography::Point(54.9888506591939,82.90938959343714,4326)),
--									(sys.geography::Point(54.98671243881107,82.90424518884213,4326));

--DROP TABLE Locations;
--SELECT *
--FROM Locations;