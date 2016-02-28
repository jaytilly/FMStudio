IF XACT_STATE() = 1
BEGIN
	RAISERROR('This migration cannot be run in a transaction', 18, 1)
END