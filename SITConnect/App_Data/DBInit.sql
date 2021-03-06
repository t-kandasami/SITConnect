﻿exec sp_MSforeachtable "declare @name nvarchar(max); set @name = parsename('?', 1); exec sp_MSdropconstraints @name";
exec sp_MSforeachtable "drop table ?";


CREATE TABLE [dbo].[Account] (
    [account_id]                     INT            IDENTITY (1, 1) NOT NULL,
    [account_fname]                  NVARCHAR (30)  NULL,
    [account_lname]                  NVARCHAR (30)  NULL,
    [account_ccard]                  NVARCHAR (MAX) NULL,
    [account_email]                  NVARCHAR (100) NULL,
    [account_passwordHash]           NVARCHAR (MAX) NULL,
    [account_passwordHashOld1]       NVARCHAR (MAX) NULL,
    [account_passwordHashOld2]       NVARCHAR (MAX) NULL,
    [account_passwordSalt]           NVARCHAR (MAX) NULL,
    [account_dob]                    NVARCHAR (MAX) NULL,
    [account_iv]                     NVARCHAR (MAX) NULL,
    [account_key]                    NVARCHAR (MAX) NULL,
    [account_invalidAttempts]        INT            NULL,
    [account_lastInvalidAttemptTime] DATETIME       NULL,
    [account_passwordMinAge]         DATETIME       NULL,
    [account_passwordMaxAge]         DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([account_id] ASC),
    UNIQUE NONCLUSTERED ([account_email] ASC)
);


