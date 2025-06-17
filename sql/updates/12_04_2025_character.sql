START TRANSACTION;

ALTER TABLE `characters` ADD `ispartnerautorelive` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `characters` ADD `ispetautorelive` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250412192140_litacore02', '8.0.13');

COMMIT;

