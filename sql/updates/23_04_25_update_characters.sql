START TRANSACTION;

ALTER TABLE `characters` MODIFY COLUMN `dignity` float NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250423055515_litacore12.1', '8.0.13');

COMMIT;

