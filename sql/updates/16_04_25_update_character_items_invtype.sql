START TRANSACTION;

ALTER TABLE `character_items` ADD `inventorytype` int NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250416193951_litacore09_characteritems_invtype', '8.0.13');

COMMIT;

