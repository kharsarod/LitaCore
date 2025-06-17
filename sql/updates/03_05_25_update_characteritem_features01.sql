START TRANSACTION;

ALTER TABLE `character_items` ADD `closedefence` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `critluckrate` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `critrate` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `defdodge` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `distdefence` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `fairylevel` tinyint unsigned NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `fairymonsterremaining` int NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `hitrate` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `magicdefence` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `maxdmg` smallint NOT NULL DEFAULT 0;

ALTER TABLE `character_items` ADD `mindmg` smallint NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250503193648_litacore12.3', '8.0.13');

COMMIT;

