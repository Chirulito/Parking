# CREATE TABLES

``` SQL
-- Authentication schema
GO
CREATE SCHEMA Auth;
GO

-- Roles table
CREATE TABLE Auth.Roles (
    id_role INT IDENTITY(1,1) PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL
);

-- Users table
CREATE TABLE Auth.Users (
    id_user INT IDENTITY(1,1) PRIMARY KEY,
    identification VARCHAR(9) NOT NULL UNIQUE,
    name VARCHAR(50) NOT NULL,
    email VARCHAR(50) NOT NULL UNIQUE,
    dob DATE NOT NULL,    
    password VARBINARY(255) NOT NULL,
    salt VARBINARY(255) NOT NULL
);

-- UsersRoles table
CREATE TABLE Auth.UsersRoles (
    id_user_role INT IDENTITY(1,1) PRIMARY KEY,    
    id_user INT NOT NULL,
    id_role INT NOT NULL,
    CONSTRAINT FK_UR_user FOREIGN KEY (id_user) 
        REFERENCES Auth.Users(id_user),  
    CONSTRAINT FK_UR_role FOREIGN KEY (id_role) 
        REFERENCES Auth.Roles(id_role),
    CONSTRAINT UQ_user_role UNIQUE (id_user, id_role)
);

-- Logs table
CREATE TABLE Auth.Logs (
    id_log INT IDENTITY(1,1) PRIMARY KEY,
    license_plate VARCHAR(20) NOT NULL,
    action VARCHAR(50) NOT NULL 
        CHECK (action IN ('entry', 'egress', 'denied', 'attempt')),
    description text,
    timestamp DATETIME DEFAULT GETDATE()
);

-- Parking schema
GO
CREATE SCHEMA Parking;
GO

-- Parking buildings
CREATE TABLE Parking.Building (
    id_building INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    maximum_capacity INT DEFAULT 0
);

-- Parking spots
CREATE TABLE Parking.Spots (
    id_spot INT IDENTITY(1,1) PRIMARY KEY,
    id_building INT NOT NULL,
    code VARCHAR(10) NOT NULL UNIQUE,
    type VARCHAR(20) 
        CHECK (type IN ('car', 'accommodation', 'motorcycle')),
    FOREIGN KEY (id_building) REFERENCES Parking.Building(id_building)
);

-- Vehicles
CREATE TABLE Parking.Vehicles (
    id_vehicle INT IDENTITY(1,1) PRIMARY KEY,
    id_user INT NOT NULL,
    license_plate VARCHAR(20) NOT NULL UNIQUE,
    brand VARCHAR(50),
    model VARCHAR(50),
    color VARCHAR(30),
    accommodation BIT,
    type VARCHAR(20) CHECK (type IN ('motorcycle', 'car')),
    FOREIGN KEY (id_user) REFERENCES Auth.Users(id_user)
);

-- Parking Occupancy
CREATE TABLE Parking.Occupancy (
    id_occupancy INT IDENTITY(1,1) PRIMARY KEY,
    id_spot INT NOT NULL,

    license_plate VARCHAR(20) NOT NULL,
    type VARCHAR(20) CHECK (type IN ('motorcycle', 'car')) NOT NULL,
    accommodation BIT NOT NULL,
    timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_spot) REFERENCES Parking.Spots(id_spot),

);
```

# INSERT TABLES
``` SQL
-- Roles
INSERT INTO Auth.Roles (role_name) VALUES
    ('System'),
    ('Staff'),
    ('Security'),
    ('Student');

-- Users
INSERT INTO Auth.Users (identification, name, email, dob, password, salt) VALUES
('111111111', 'System', 'system@service.com', '1900-01-01',
 0xA920F8B8C0E22E3360E3B946C204AAA831F41F66B8334F92D1242D51E2255E15,
 0x9ED084CFE7DC2FC54DE649550AB8649A),
('222222222', 'Staff', 'staff@service.com', '1900-01-01',
 0xA920F8B8C0E22E3360E3B946C204AAA831F41F66B8334F92D1242D51E2255E15,
 0x9ED084CFE7DC2FC54DE649550AB8649A),
('333333333', 'Security', 'security@service.com', '1900-01-01',
 0xA920F8B8C0E22E3360E3B946C204AAA831F41F66B8334F92D1242D51E2255E15,
 0x9ED084CFE7DC2FC54DE649550AB8649A),
('444444444', 'Student', 'student@service.com', '1900-01-01',
 0xA920F8B8C0E22E3360E3B946C204AAA831F41F66B8334F92D1242D51E2255E15,
 0x9ED084CFE7DC2FC54DE649550AB8649A),
('123456789', 'Student', 'person@email.com', '2000-01-01',
 0xA920F8B8C0E22E3360E3B946C204AAA831F41F66B8334F92D1242D51E2255E15,
 0x9ED084CFE7DC2FC54DE649550AB8649A),
('987654321', 'Student', 'human@email.com', '2000-01-01',
 0xA920F8B8C0E22E3360E3B946C204AAA831F41F66B8334F92D1242D51E2255E15,
 0x9ED084CFE7DC2FC54DE649550AB8649A);

-- UsersRoles
INSERT INTO Auth.UsersRoles (id_user, id_role)
VALUES
    (1, 1),
    (2, 2),
    (3, 3),
    (4, 4),
    (5, 4),
    (6, 4);

-- Parking buildings
INSERT INTO Parking.Building (name, maximum_capacity) VALUES
    ('Edificio A', 15),
    ('Edificio B', 15),
    ('Edificio C', 15);

-- Vehicles
INSERT INTO Parking.Vehicles (id_user, license_plate, brand, model, color, accommodation, type) VALUES
    (5, 'AAA111', 'Toyota', 'Corolla', 'Rojo', 0, 'car'),
    (6, 'BBB222', 'Yamaha', 'R15', 'Negro', 0, 'motorcycle');

-- Logs
INSERT INTO Auth.Logs (license_plate, action, description)
VALUES 
    ('CCC333', 'attempt', 'Example');

-- Insert Parking Spots
DECLARE @i INT = 1;
WHILE @i <= 10
BEGIN
    INSERT INTO Parking.Spots (id_building, code, type)
    VALUES (1, CONCAT('A', @i), 'car');
    SET @i += 1;
END

SET @i = 1;
WHILE @i <= 3
BEGIN
    INSERT INTO Parking.Spots (id_building, code, type)
    VALUES (1, CONCAT('B', @i), 'motorcycle');
    SET @i += 1;
END

SET @i = 1;
WHILE @i <= 2
BEGIN
    INSERT INTO Parking.Spots (id_building, code, type)
    VALUES (1, CONCAT('C', @i), 'accommodation');
    SET @i += 1;
END

DECLARE @i INT = 1;
WHILE @i <= 10
BEGIN
    INSERT INTO Parking.Spots (id_building, code, type)
    VALUES (2, CONCAT('D', @i), 'car');
    SET @i += 1;
END

SET @i = 1;
WHILE @i <= 3
BEGIN
    INSERT INTO Parking.Spots (id_building, code, type)
    VALUES (2, CONCAT('E', @i), 'motorcycle');
    SET @i += 1;
END

SET @i = 1;
WHILE @i <= 2
BEGIN
    INSERT INTO Parking.Spots (id_building, code, type)
    VALUES (2, CONCAT('F', @i), 'accommodation');
    SET @i += 1;
END

INSERT INTO Parking.Occupancy (id_spot, license_plate, type, accommodation)
VALUES (1, 'AAA111', 'car', 0);

INSERT INTO Parking.Occupancy (id_spot, license_plate, type, accommodation)
VALUES (11, 'BBB111', 'motorcycle', 0);
```

DROPS 
``` SQL

DROP TABLE IF EXISTS Parking.Occupancy;
DROP TABLE IF EXISTS Auth.Logs;
DROP TABLE IF EXISTS Auth.UsersRoles;
DROP TABLE IF EXISTS Parking.Vehicles;
DROP TABLE IF EXISTS Parking.Spots;
DROP TABLE IF EXISTS Auth.Users;
DROP TABLE IF EXISTS Auth.Roles;
DROP TABLE IF EXISTS Parking.Building;

DROP SCHEMA IF EXISTS Auth;
DROP SCHEMA IF EXISTS Parking;
```
