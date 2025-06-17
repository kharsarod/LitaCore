START TRANSACTION;

ALTER TABLE `login_sessions` ADD `username` longtext CHARACTER SET utf8mb4 NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250418021012_litacore10.1', '8.0.13');

COMMIT;

