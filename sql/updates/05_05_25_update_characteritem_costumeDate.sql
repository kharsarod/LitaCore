START TRANSACTION;

ALTER TABLE `character_items` ADD `timeremaining` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250505003950_litacore12.4', '8.0.13');

COMMIT;

