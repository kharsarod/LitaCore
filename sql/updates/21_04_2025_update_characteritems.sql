START TRANSACTION;

ALTER TABLE `character_items` DROP COLUMN `isequipped`;

ALTER TABLE `character_items` ADD `equipmentslot` int NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250421024833_litacore12', '8.0.13');

COMMIT;

