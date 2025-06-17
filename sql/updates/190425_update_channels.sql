START TRANSACTION;

ALTER TABLE `channels` ADD `channelid` tinyint unsigned NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250419023307_litacore11.1_channels', '8.0.13');

COMMIT;

