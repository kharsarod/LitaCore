START TRANSACTION;

ALTER TABLE `session_tokens` ADD `createdat` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE `session_tokens` ADD `expiresat` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250414042822_litacore05', '8.0.13');

COMMIT;

