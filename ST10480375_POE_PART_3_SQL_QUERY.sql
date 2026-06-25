CREATE DATABASE ccp_chatbot;
GO

USE ccp_chatbot;
GO

CREATE TABLE tasks (
    id INT IDENTITY(1,1) PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description VARCHAR(500),
    reminder VARCHAR(255),
    is_completed BIT DEFAULT 0
);
GO

Select * from tasks;