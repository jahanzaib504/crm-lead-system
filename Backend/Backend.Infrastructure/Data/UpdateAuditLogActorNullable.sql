IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.AuditLogs')
      AND name = N'PerformedByUserId'
      AND is_nullable = 0
)
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        IF OBJECT_ID(N'dbo.FK_AuditLogs_Users', N'F') IS NOT NULL
        BEGIN
            ALTER TABLE dbo.AuditLogs
            DROP CONSTRAINT FK_AuditLogs_Users;
        END;

        ALTER TABLE dbo.AuditLogs
        ALTER COLUMN PerformedByUserId INT NULL;

        ALTER TABLE dbo.AuditLogs
        ADD CONSTRAINT FK_AuditLogs_Users
            FOREIGN KEY (PerformedByUserId) REFERENCES dbo.Users(Id);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
