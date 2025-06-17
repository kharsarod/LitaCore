START TRANSACTION;

ALTER TABLE `character_items` ADD `ammo` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `holdingmodel` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `isfixed` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250503050227_litacore12.2', '8.0.13');

COMMIT;

