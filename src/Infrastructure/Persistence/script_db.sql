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

DELETE FROM tbEmployees WHERE id = '06a09af7-1c43-4f9c-87d5-4bf50255de59' RETURNING user_id;

SELECT * from tbEmployees;
SELECT id As Id, user_id As UserId, position As Position, subsidy As Subsidy FROM tbEmployees;

WITH updated AS 
(
    UPDATE tbEmployees
    SET
        position = CASE WHEN position IS DISTINCT FROM 'Trainer' THEN 'Trainer' ELSE position END,
        subsidy  = CASE WHEN subsidy  IS DISTINCT FROM 17000  THEN 17000  ELSE subsidy  END
    WHERE id = '951d3f4f-6736-470e-84ce-56cf65f0a8a4'
)
SELECT user_id
FROM tbEmployees
WHERE id = '951d3f4f-6736-470e-84ce-56cf65f0a8a4';
-- QUERIES