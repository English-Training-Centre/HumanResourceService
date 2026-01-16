/*
powered by  : (c) 2025, Ramadan Ismael - All rights reserved!!
to          : ETC - Human Resources Service
*/

-- sudo -u postgres psql
-- CREATE DATABASE etc_db_human_resources_service;
-- GRANT ALL PRIVILEGES ON DATABASE etc_db_human_resources_service TO ramadan;

CREATE EXTENSION IF NOT EXISTS pgcrypto; -- auto uuid

-- EMPLOYEE
CREATE TABLE IF NOT EXISTS tbEmployees
(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID UNIQUE NOT NULL,
    position VARCHAR(25) NOT NULL,
    subsidy DECIMAL(10,2) NOT NULL CHECK(subsidy > 0.00)
);
CREATE INDEX idx_tbemployees_user_id ON tbEmployees(user_id);
-- EMPLOYEE


-- QUERIES
DROP TABLE IF EXISTS tbEmployees;
DROP INDEX IF EXISTS idx_tbemployees_user_id;

SELECT * from tbEmployees;
SELECT id As Id, user_id As UserId, position As Position, subsidy As Subsidy FROM tbEmployees;

-- QUERIES