IF XACT_STATE() = 0
BEGIN
	RAISERROR('This migration must be run in a transaction', 18, 1)
END