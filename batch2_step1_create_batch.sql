-- ========================================
-- Batch 2 - Step 1: Create Batch
-- ========================================

USE WitchTrialWT;
GO

PRINT '========================================';
PRINT 'Batch 2 - Step 1: Create Batch';
PRINT '========================================';
GO

-- Create Batch 2
DECLARE @islandId INT;
SET @islandId = 1;

IF NOT EXISTS (SELECT 1 FROM wt.Batch WHERE IslandID = @islandId AND BatchID = 2)
BEGIN
    INSERT INTO wt.Batch (IslandID, WitchCount)
    VALUES (@islandId, 0);
    
    PRINT 'Batch 2 created successfully';
    PRINT '   IslandID: ' + CAST(@islandId AS NVARCHAR);
    PRINT '   BatchID: ' + CAST(SCOPE_IDENTITY() AS NVARCHAR);
END
ELSE
BEGIN
    PRINT 'Batch 2 already exists';
END
GO

-- Verify batches
SELECT 
    b.BatchID,
    b.IslandID,
    i.Name AS IslandName,
    b.WitchCount
FROM wt.Batch b
JOIN wt.Island i ON b.IslandID = i.IslandID
ORDER BY b.BatchID;
GO

PRINT '';
PRINT '========================================';
PRINT 'Step 1 completed!';
PRINT '========================================';
GO
